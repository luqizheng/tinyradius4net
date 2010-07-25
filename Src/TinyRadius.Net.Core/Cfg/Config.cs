using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace TinyRadius.Net.Cfg
{
    [DataContract]
    public class Config
    {
        private const string fileName = "TinyServerSetting.xml";
        public static readonly Config Instance = new Config();
        private Dictionary<string, string> _nasSettings = new Dictionary<string, string>();

        public Config()
        {
            AuthListentIp = "192.168.1.123";
            AccountListentIp = "192.168.1.123";
            AcctPort = 1813;
            AuthPort = 1812;
            if (File.Exists(fileName))
            {
                var mySerializer = new DataContractSerializer(typeof (Config));
                FileStream stream = File.OpenRead(fileName);
                try
                {
                    var config = (Config) mySerializer.ReadObject(stream);
                    InitBy(config);
                }
                finally
                {
                    stream.Close();
                }
            }
        }

        [DataMember]
        public string AuthListentIp { get; set; }

        [DataMember]
        public int AuthPort { get; set; }

        [DataMember]
        public bool EnableAuthentication { get; set; }

        [DataMember]
        public string AccountListentIp { get; set; }

        [DataMember]
        public int AcctPort { get; set; }

        [DataMember]
        public bool EnableAccount { get; set; }

        [DataMember]
        public Dictionary<string, string> NasSettings
        {
            get { return _nasSettings; }
            set { _nasSettings = value; }
        }

        private void InitBy(Config config)
        {
            AccountListentIp = config.AccountListentIp;
            AcctPort = config.AcctPort;
            EnableAccount = config.EnableAccount;

            AuthListentIp = config.AuthListentIp;
            AuthPort = config.AuthPort;
            EnableAuthentication = config.EnableAuthentication;

            _nasSettings = config._nasSettings;
        }

        public void Save()
        {
            var mySerializer = new DataContractSerializer(typeof (Config));

            var myWriter = new FileStream(fileName, FileMode.Create);
            mySerializer.WriteObject(myWriter, this);
            myWriter.Close();
        }
    }
}