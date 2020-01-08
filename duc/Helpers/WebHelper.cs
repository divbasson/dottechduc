using System.Net;
using System.Net.NetworkInformation;

namespace duc.Helpers
{
    public static class WebHelper
    {
        public static string GetExternalIp(string proxyAddress = "")
        {
            var client = new WebClient();

            if (!string.IsNullOrEmpty(proxyAddress))
            {
                WebProxy wp = new WebProxy(proxyAddress);
                client.Proxy = wp;
            }

            string externalip = client.DownloadString("http://ipinfo.io/ip").Replace("\n", "");
            return externalip;
        }

        public static string GetDomainIp(string domainName, int timeout = 1024)
        {
            Ping pingSender = new Ping();

            PingReply reply = pingSender.Send(domainName, timeout);
            if (!string.IsNullOrEmpty(reply.Address.ToString()))
            {
                return reply.Address.ToString();
            }
            else
            {
                return reply.Status.ToString();
            }
        }
    }
}