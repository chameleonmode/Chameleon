namespace Chameleon.Interfaces.UserProfiles
{
    public static class HostConverter
    {
        private const string ChameleonModeHost = "proxy.chameleonmode.com";
        private const string PacketStreamHost = "proxy.packetstream.io";

        public static string GetHostForRequest(string host)
        {
            if (host.Contains(ChameleonModeHost))
            {
                return PacketStreamHost;
            }

            return host;
        }
    }
}
