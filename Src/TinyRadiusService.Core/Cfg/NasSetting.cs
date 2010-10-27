using System.Net;
using System.Runtime.Serialization;

namespace TinyRadiusService.Cfg
{
    [DataContract]
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
        [DataMember]
        public IPAddress Ip { get; set; }
        [DataMember]
        public string SecretKey { get; set; }
    }
}
