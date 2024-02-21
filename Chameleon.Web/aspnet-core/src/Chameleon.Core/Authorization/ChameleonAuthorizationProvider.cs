using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace Chameleon.Authorization
{
    public class ChameleonAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
            context.CreatePermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
            context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
            context.CreatePermission(PermissionNames.Pages_Licences, L("Licences"));
            context.CreatePermission(PermissionNames.Pages_ProxyCredits, L("ProxyCredits"));
            context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);

            context.CreatePermission(PermissionNames.Pages_Users_Primary, L("PrimaryUser"));
            context.CreatePermission(PermissionNames.Pages_Users_Assistant, L("AssistantUser"));

            context.CreatePermission(PermissionNames.Pages_Outreach, L("Outreach"));
            context.CreatePermission(PermissionNames.Pages_Prospector, L("Prospector"));
            context.CreatePermission(PermissionNames.Pages_YouTube, L("YouTube"));
            context.CreatePermission(PermissionNames.Pages_YouTube_Config, L("YouTubeConfig"));
            context.CreatePermission(PermissionNames.Pages_RSS, L("RSS"));
            context.CreatePermission(PermissionNames.Pages_Curate, L("Curate"));
            context.CreatePermission(PermissionNames.Pages_Curate_Config, L("CurateConfig"));
            context.CreatePermission(PermissionNames.Pages_CreateProfiles, L("CreateProfiles"));
            context.CreatePermission(PermissionNames.Pages_DeleteProfiles, L("DeleteProfiles"));
            context.CreatePermission(PermissionNames.Pages_Proxy, L("Proxy"));
            context.CreatePermission(PermissionNames.Pages_Proxy_Config, L("ProxyConfig"));
            context.CreatePermission(PermissionNames.Pages_ProxyCredit, L("ProxyCredit"));
            context.CreatePermission(PermissionNames.Pages_ImportExport, L("ImportExport"));
            context.CreatePermission(PermissionNames.Pages_AssistantUsers, L("AssistantUsers"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, ChameleonConsts.LocalizationSourceName);
        }
    }
}
