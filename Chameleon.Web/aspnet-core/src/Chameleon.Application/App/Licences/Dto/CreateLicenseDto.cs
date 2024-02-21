using Chameleon.App.Licences.Attributes;

namespace Chameleon.App
{
    public class CreateLicenseDto 
        : GenerateLicenseDto
    {
        [LicenseKey]
        public string LicenseKey { get; set; }
    }
}
