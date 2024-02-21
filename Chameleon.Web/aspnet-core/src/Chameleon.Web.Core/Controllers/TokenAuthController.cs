using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Abp.UI;
using Chameleon.Authentication.External;
using Chameleon.Authentication.JwtBearer;
using Chameleon.Authorization;
using Chameleon.Authorization.Users;
using Chameleon.Models.TokenAuth;
using Chameleon.MultiTenancy;
using Abp.Domain.Uow;
using Abp.Domain.Repositories;
using Chameleon.App.Entities;
using Chameleon.App.ValueObjects;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Chameleon.Authorization.Roles;
using System.Transactions;
//using NUglify.Helpers;
using Microsoft.EntityFrameworkCore;
using Chameleon.App.Services.License_Key.Dto;
using Chameleon.App.Services.License_Key;

namespace Chameleon.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TokenAuthController : ChameleonControllerBase
    {
        private readonly LogInManager _logInManager;
        private readonly UserManager _userManager;
        private readonly IRepository<License> _licenseRepository;
        private readonly ITenantCache _tenantCache;
        private readonly AbpLoginResultTypeHelper _abpLoginResultTypeHelper;
        private readonly TokenAuthConfiguration _configuration;
        private readonly IExternalAuthConfiguration _externalAuthConfiguration;
        private readonly IExternalAuthManager _externalAuthManager;
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly ILicenseKeyService _licenseKeyService;
        private readonly IRepository<AssistantLicense> _assistantLicenseRepository;
        private readonly IPermissionManager _permissionManager;
        private readonly RoleManager _roleManager;

        public TokenAuthController(
            LogInManager logInManager,
            UserManager userManager,
            IRepository<License> licenseRepository,
            ITenantCache tenantCache,
            AbpLoginResultTypeHelper abpLoginResultTypeHelper,
            TokenAuthConfiguration configuration,
            IExternalAuthConfiguration externalAuthConfiguration,
            IExternalAuthManager externalAuthManager,
            UserRegistrationManager userRegistrationManager,
            ILicenseKeyService licenseKeyService,
            IRepository<AssistantLicense> assistantLicenseRepository,
            IPermissionManager permissionManager,
            RoleManager roleManager)
        {
            _logInManager = logInManager;
            _userManager = userManager;
            _licenseRepository = licenseRepository;
            _tenantCache = tenantCache;
            _abpLoginResultTypeHelper = abpLoginResultTypeHelper;
            _configuration = configuration;
            _externalAuthConfiguration = externalAuthConfiguration;
            _externalAuthManager = externalAuthManager;
            _userRegistrationManager = userRegistrationManager;
            _licenseKeyService = licenseKeyService;
            _assistantLicenseRepository = assistantLicenseRepository;
            _permissionManager = permissionManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<AuthenticateResultModel> Authenticate([FromBody] AuthenticateModel model)
        {
            var resultModel = await AuthenticateInternal(model);
            await InitializePermissions(resultModel);
            return resultModel;
        }
        [HttpGet]
        public async Task<IsLicActiveResultModel> IsLicenseActive(string key)
        {
            var license = await _licenseRepository
                .FirstOrDefaultAsync(l => l.LicenseKeyValue == key);

            return new IsLicActiveResultModel { isActive = license?.IsActive ?? false };
        }

        private async Task<AuthenticateResultModel> AuthenticateInternal(AuthenticateModel model)
        {
            AuthenticateResultModel resultModel = null;

            if (model.Password.StartsWith("KEY"))
            {
                resultModel = await AssistantAuthenticateOrNull(model);
            }
            else
            {
                resultModel = await LicenseAuthenticateOrNull(model);
            }

            if (resultModel == null)
            {
                return await AuthenticateDefault(model);
            }

            return resultModel;
        }

        private async Task<AuthenticateResultModel> AuthenticateDefault(AuthenticateModel model)
        {
            var loginResult = await GetLoginResultAsync(
                model.UserNameOrEmailAddress,
                model.Password,
                GetTenancyNameOrNull()
            );

            var accessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity));
            var refreshToken = GenerateRefreshToken();

            loginResult.User.RefreshToken = refreshToken;
            loginResult.User.RefreshTokenExpiryTime = DateTime.Now.AddDays((int)_configuration.RefreshTokenExpiration.TotalDays);

            var resultModel = new AuthenticateResultModel
            {
                AccessToken = accessToken,
                EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                ExpireInSeconds = (long)_configuration.Expiration.TotalSeconds,
                UserId = loginResult.User.Id,
                CreatorUserId = loginResult.User.CreatorUserId,
                RefreshToken = refreshToken,
                TookGuidedTour = loginResult.User.TookGuidedTour
            };

            return resultModel;
        }

        private async Task<AuthenticateResultModel> LicenseAuthenticateOrNull(AuthenticateModel model)
        {
            if (!LicenseKey.IsValid(model.Password))
            {
                return null;
            }

            try
            {
                return await LicenseAuthenticate(new LicenseAuthenticateModel
                {
                    EmailAddress = model.UserNameOrEmailAddress,
                    LicenseKey = model.Password
                });
            }
            catch (AbpAuthorizationException)
            {
                throw;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<AuthenticateResultModel> AssistantAuthenticateOrNull(AuthenticateModel model)
        {
            if (!AssistantLicenseKey.IsValid(model.Password))
            {
                return null;
            }

            try
            {
                return await AssistantAuthenticate(new LicenseAuthenticateModel
                {
                    EmailAddress = model.UserNameOrEmailAddress,
                    LicenseKey = model.Password
                });
            }
            catch (AbpAuthorizationException)
            {
                throw;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpGet]
        public List<ExternalLoginProviderInfoModel> GetExternalAuthenticationProviders()
        {
            return ObjectMapper.Map<List<ExternalLoginProviderInfoModel>>(_externalAuthConfiguration.Providers);
        }

        [HttpPost]
        public async Task<ExternalAuthenticateResultModel> ExternalAuthenticate([FromBody] ExternalAuthenticateModel model)
        {
            var externalUser = await GetExternalUserInfo(model);

            var loginResult = await _logInManager.LoginAsync(new UserLoginInfo(model.AuthProvider, model.ProviderKey, model.AuthProvider), GetTenancyNameOrNull());

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    {
                        var accessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity));
                        return new ExternalAuthenticateResultModel
                        {
                            AccessToken = accessToken,
                            EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                            ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds
                        };
                    }
                case AbpLoginResultType.UnknownExternalLogin:
                    {
                        var newUser = await RegisterExternalUserAsync(externalUser);
                        if (!newUser.IsActive)
                        {
                            return new ExternalAuthenticateResultModel
                            {
                                WaitingForActivation = true
                            };
                        }

                        // Try to login again with newly registered user!
                        loginResult = await _logInManager.LoginAsync(new UserLoginInfo(model.AuthProvider, model.ProviderKey, model.AuthProvider), GetTenancyNameOrNull());
                        if (loginResult.Result != AbpLoginResultType.Success)
                        {
                            throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                                loginResult.Result,
                                model.ProviderKey,
                                GetTenancyNameOrNull()
                            );
                        }

                        return new ExternalAuthenticateResultModel
                        {
                            AccessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity)),
                            ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds
                        };
                    }
                default:
                    {
                        throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                            loginResult.Result,
                            model.ProviderKey,
                            GetTenancyNameOrNull()
                        );
                    }
            }
        }

        private async Task<User> RegisterExternalUserAsync(ExternalAuthUserInfo externalUser)
        {
            var user = await _userRegistrationManager.RegisterAsync(
                externalUser.Name,
                externalUser.Surname,
                externalUser.EmailAddress,
                externalUser.EmailAddress,
                Authorization.Users.User.CreateRandomPassword(),
                true
            );

            user.Logins = new List<UserLogin>
            {
                new UserLogin
                {
                    LoginProvider = externalUser.Provider,
                    ProviderKey = externalUser.ProviderKey,
                    TenantId = user.TenantId
                }
            };

            await CurrentUnitOfWork.SaveChangesAsync();

            return user;
        }

        private async Task<ExternalAuthUserInfo> GetExternalUserInfo(ExternalAuthenticateModel model)
        {
            var userInfo = await _externalAuthManager.GetUserInfo(model.AuthProvider, model.ProviderAccessCode);
            if (userInfo.ProviderKey != model.ProviderKey)
            {
                throw new UserFriendlyException(L("CouldNotValidateExternalUser"));
            }

            return userInfo;
        }

        private string GetTenancyNameOrNull()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return _tenantCache.GetOrNull(AbpSession.TenantId.Value)?.TenancyName;
        }

        private async Task<AbpLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
        {
            var loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    return loginResult;
                default:
                    throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult.Result, usernameOrEmailAddress, tenancyName);
            }
        }

        private string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
        {
            var now = DateTime.UtcNow;

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(expiration ?? _configuration.Expiration),
                signingCredentials: _configuration.SigningCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        private static List<Claim> CreateJwtClaims(ClaimsIdentity identity)
        {
            var claims = identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == ClaimTypes.NameIdentifier);

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            });

            return claims;
        }

        private static string GetEncryptedAccessToken(string accessToken)
        {
            return SimpleStringCipher.Instance.Encrypt(accessToken, AppConsts.DefaultPassPhrase);
        }

        private async Task<AuthenticateResultModel> LicenseAuthenticate(LicenseAuthenticateModel model)
        {
            using (UnitOfWorkManager.Current.DisableFilter(
                AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant
                ))
            {
                return await LicenseAuthenticateInternal(model);
            }
        }

        private async Task<AuthenticateResultModel> LicenseAuthenticateInternal(LicenseAuthenticateModel model)
        {
            async Task ActivateLicense(License activeLic)
            {
                var userLics = await _licenseRepository.GetAllListAsync(l => l.UserId == activeLic.UserId);
                userLics.ForEach(l => l.IsActive = false);
                activeLic.IsActive = true;
            }

            if (!await _licenseKeyService.IsValidAsync(model.LicenseKey))
            {
                throw new AbpAuthorizationException();
            }

            var license = await _licenseKeyService.GetOrCreateAsync(
                model.EmailAddress,
                model.LicenseKey
                );
            await ActivateLicense(license);

            var licenseUser = _userManager.GetUserById(license.UserId);
            var licenseType = await _licenseKeyService.GetTypeAsync(license.LicenseKey);

            if (!string.Equals(licenseUser.EmailAddress, model.EmailAddress,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new AbpAuthorizationException();
            }

            var tenancyName = _tenantCache.GetOrNull(licenseUser.TenantId.GetValueOrDefault())?.TenancyName ?? License.DefaultTenantName;

            var loginResult = await GetLoginResultAsync(
                model.EmailAddress,
                tenancyName
            );

            var accessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity));
            var refreshToken = GenerateRefreshToken();

            licenseUser.RefreshToken = refreshToken;
            licenseUser.RefreshTokenExpiryTime = DateTime.Now.AddDays((int)_configuration.RefreshTokenExpiration.TotalDays);

            var resultModel = new AuthenticateResultModel
            {
                AccessToken = accessToken,
                EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                ExpireInSeconds = (long)_configuration.Expiration.TotalSeconds,
                UserId = loginResult.User.Id,
                CreatorUserId = loginResult.User.CreatorUserId,
                LicenseLimits = new LicenseLimits(licenseType),
                RefreshToken = refreshToken,
                TookGuidedTour = licenseUser.TookGuidedTour
            };
            return resultModel;
        }

        private async Task<AuthenticateResultModel> AssistantAuthenticate(LicenseAuthenticateModel input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(
                AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant
                ))
            {
                return await AssistantAuthenticateInternal(input);
            }
        }

        private async Task<AuthenticateResultModel> AssistantAuthenticateInternal(LicenseAuthenticateModel model)
        {
            var license = _assistantLicenseRepository
                .GetAll()
                .Where(entity => entity.LicenseKeyValue == model.LicenseKey)
                .FirstOrDefault();

            if (license == null)
            {
                throw new AbpAuthorizationException();
            }

            var primaryUserLicense = _licenseRepository
                .GetAll()
                .Where(entity => entity.UserId == license.PrimaryUserId)
                .FirstOrDefault();

            await VerifyPrimaryUserLicenceAsync(primaryUserLicense);

            var licenseUser = _userManager.GetUserById(license.UserId);
            var primaryUserLicenseType = await _licenseKeyService.GetTypeAsync(primaryUserLicense.LicenseKey);

            if (!string.Equals(licenseUser.EmailAddress, model.EmailAddress,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new AbpAuthorizationException();
            }

            var tenancyName = _tenantCache.GetOrNull(licenseUser.TenantId.GetValueOrDefault())?.TenancyName ?? License.DefaultTenantName;

            var loginResult = await GetLoginResultAsync(
                model.EmailAddress,
                tenancyName
            );

            var accessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity));
            var refreshToken = GenerateRefreshToken();

            licenseUser.RefreshToken = refreshToken;
            licenseUser.RefreshTokenExpiryTime = DateTime.Now.AddDays((int)_configuration.RefreshTokenExpiration.TotalDays);

            var resultModel = new AuthenticateResultModel
            {
                AccessToken = accessToken,
                EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                ExpireInSeconds = (long)_configuration.Expiration.TotalSeconds,
                UserId = loginResult.User.Id,
                CreatorUserId = loginResult.User.CreatorUserId,
                LicenseLimits = new LicenseLimits(primaryUserLicenseType),
                RefreshToken = refreshToken,
                TookGuidedTour = licenseUser.TookGuidedTour,
                CanCreateProfiles = license.CanCreateProfiles
            };

            return resultModel;
        }

        private async Task VerifyPrimaryUserLicenceAsync(License license)
        {
            try
            {
                if (license == null)
                {
                    throw new AbpAuthorizationException();
                }

                if (!await _licenseKeyService.IsValidAsync(license.LicenseKey))
                {
                    throw new AbpAuthorizationException();
                }
            }
            catch (Exception)
            {
                throw new AbpAuthorizationException();
            }
        }

        private async Task<IList<Permission>> GetAssistantPermissionsForAssign(LicenseLimits limits)
        {
            var permissionNames = new List<string>();

            if (limits.HasOutreach)
            {
                permissionNames.Add(PermissionNames.Pages_Outreach);
            }

            if(limits.HasYouTube)
            {
                permissionNames.Add(PermissionNames.Pages_YouTube);
            }

            if(limits.HasWordPress)
            {
                permissionNames.Add(PermissionNames.Pages_Curate);
            }

            if(limits.ContentDiscoveryLimits.HasProspector)
            {
                permissionNames.Add(PermissionNames.Pages_Prospector);
            }

            if (limits.ContentDiscoveryLimits.MaxRssCount > 0)
            {
                permissionNames.Add(PermissionNames.Pages_RSS);
            }

            permissionNames.Add(PermissionNames.Pages_CreateProfiles);
            permissionNames.Add(PermissionNames.Pages_DeleteProfiles);

            var permissions = new List<Permission>();
            var allPermissions = _permissionManager.GetAllPermissions(true);

            foreach (var permissionName in permissionNames)
            {
                var permission = allPermissions
                    .Single(p => p.Name == permissionName);

                permissions.Add(permission);
            }

            var role = await _roleManager.GetRoleByNameAsync(StaticRoleNames.Tenants.AssistantUser);
            var rolePermissions = await _roleManager.GetGrantedPermissionsAsync(role.Id);

            foreach(var rolePermission in rolePermissions)
            {
                permissions.Add(rolePermission);
            }

            return permissions;
        }

        private async Task InitializePermissions(AuthenticateResultModel model)
        {
            async Task<IEnumerable<Permission>> GetAddSetsPerms(long userId) 
            {
                User _user = await _userManager.GetUserByIdAsync(userId);
                return (await _userManager.GetGrantedPermissionsAsync(_user))
                        .Where(gp =>
                        gp.Name == PermissionNames.Pages_Proxy_Config ||
                        gp.Name == PermissionNames.Pages_Curate_Config ||
                        gp.Name == PermissionNames.Pages_YouTube_Config);
            }

            User user = null;

            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                user = await _userManager.GetUserByIdAsync(model.UserId);
            }

            using (UnitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                IReadOnlyCollection<Permission> grantedPermissions;

                if (user.CreatorUserId.HasValue)
                {
                    using (var unitOfWork = UnitOfWorkManager.Begin(TransactionScopeOption.Suppress))
                    {
                        var permissionsForAssign = await GetAssistantPermissionsForAssign(model.LicenseLimits);
                        await _userManager.SetGrantedPermissionsAsync(user, permissionsForAssign);
                        await unitOfWork.CompleteAsync();
                    }

                    grantedPermissions = (await _userManager.GetGrantedPermissionsAsync(user))
                        .Union(await GetAddSetsPerms(user.CreatorUserId.Value))
                        .DistinctBy(gp => gp.Name)
                        .ToList()
                        .AsReadOnly();
                }
                else grantedPermissions = await _userManager.GetGrantedPermissionsAsync(user);

                model.Permissions = grantedPermissions
                    .Select(x => x.Name)
                    .ToArray();
            }
        }

        private async Task<AbpLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string tenancyName)
        {
            var loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, tenancyName);
            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    return loginResult;
                default:
                    throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                        loginResult.Result, usernameOrEmailAddress, tenancyName);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RefreshToken([FromBody] TokenModel token)
        {
            if (token == null)
            {
                return BadRequest("Invalid client request");
            }

            string accessToken = token.AccessToken;
            string refreshToken = token.RefreshToken;

            var principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            {
                return BadRequest("Invalid access token");
            }

            string username = principal.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest("Invalid refresh token");
            }

            var newAccessToken = CreateAccessToken(principal.Claims.ToList());
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return new ObjectResult(new
            {
                NewAccessToken = newAccessToken,
                NewRefreshToken = newRefreshToken,
                ExpireInSeconds = (long)_configuration.Expiration.TotalSeconds
            });
        }

        [HttpPost]
        public async Task<IActionResult> Revoke(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return BadRequest("Invalid user name");

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> RevokeAll()
        {
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                user.RefreshToken = null;
                await _userManager.UpdateAsync(user);
            }

            return NoContent();
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _configuration.SecurityKey,
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}