namespace Chameleon.App.Services.License_Key.Dto
{
    public class LicenseStatusReponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public LicenseStatus Data { get; set; }
    }
}
