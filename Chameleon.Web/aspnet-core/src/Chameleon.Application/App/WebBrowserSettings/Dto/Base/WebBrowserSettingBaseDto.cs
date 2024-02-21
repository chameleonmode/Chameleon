namespace Chameleon.App
{
    public class WebBrowserSettingBaseDto
    {
        public bool WebRTC { get; set; }
        public bool WebGL { get; set; }
        public bool Tracking { get; set; }
        public bool Flash { get; set; }
        public decimal Canvas { get; set; }
        public int? UserAgentId { get; set; }
    }
}
