using Chameleon.App.Licences.Attributes;

namespace Chameleon.App
{
    public class LicenseBaseDto
    {
        [LicenseKey]
        public string LicenseKey { get; set; }
    }
}
