using System.Net;

namespace TinyRadiusService.Cfg
{
    public class NasSetting
    {
        public NasSetting(string ip, string secretKey)
        {
            this.Ip = IPAddress.Parse(ip);
            SecretKey = secretKey;
        }

        public NasSetting()
        {
            
        }

        public IPAddress Ip { get; set; }
        public string SecretKey { get; set; }
    }
}
