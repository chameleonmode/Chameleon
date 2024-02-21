using Abp.Dependency;
using Chameleon.App.Entities;
using Chameleon.App.Services.License_Key.Dto;
using Chameleon.App.ValueObjects;
using System.Threading.Tasks;

//using LicenseKey = Chameleon.App.ValueObjects.LicenseKey;

namespace Chameleon.App.Services.License_Key
{
    public interface ILicenseKeyService
        : ITransientDependency
    {
        Task<bool> IsValidAsync(string licenseKey);
        Task<bool> IsValidAsync(LicenseKey licenseKey);
        Task<License> GetOrCreateAsync(string emailAddress, string licenseKeyValue);
        Task<LicenseType> GetTypeAsync(LicenseKey licenseKey);
    }
}
