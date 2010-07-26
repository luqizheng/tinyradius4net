using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Win32;

namespace TinyRadius.Net.Cfg
{
    [DataContract]
    public class Config
    {
        private const string FileName = "TinyServerSetting.xml";
        private Dictionary<string, string> _nasSettings = new Dictionary<string, string>();
        private readonly string _filePath;
        public Config(string path)
        {
            this.DatabaseSetting = new DatabaseSetting();
            LdapSetting = new LdapSetting();
            AuthListentIp = "192.168.1.123";
            AccountListentIp = "192.168.1.123";
            AcctPort = 1813;
            AuthPort = 1812;
            EnableAccount = true;
            EnableAuthentication = true;
            _filePath = Path.Combine(path, FileName);
            if (File.Exists(_filePath))
            {
                var mySerializer = new DataContractSerializer(typeof(Config));
                FileStream stream = File.OpenRead(FileName);
                try
                {
                    var config = (Config)mySerializer.ReadObject(stream);
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


        [DataMember]
        public bool ValidateByDatabase { get; set; }
        [DataMember]
        public bool ValidateByLdap { get; set; }
        [DataMember]
        public DatabaseSetting DatabaseSetting { get; set; }
        [DataMember]
        public LdapSetting LdapSetting { get; set; }

        private void InitBy(Config config)
        {
            AccountListentIp = config.AccountListentIp;
            AcctPort = config.AcctPort;
            EnableAccount = config.EnableAccount;

            AuthListentIp = config.AuthListentIp;
            AuthPort = config.AuthPort;
            EnableAuthentication = config.EnableAuthentication;

            _nasSettings = config._nasSettings;

            ValidateByDatabase = config.ValidateByDatabase;
            ValidateByLdap = config.ValidateByLdap;

            DatabaseSetting = config.DatabaseSetting;
            LdapSetting = config.LdapSetting;
        }

        public void Save()
        {
            if (String.IsNullOrEmpty(DatabaseSetting.Connection) && ValidateByDatabase)
                throw new ArgumentException("使用Database验证用户，但是链接字符串为空");
            if (String.IsNullOrEmpty(DatabaseSetting.PasswordSql) && ValidateByDatabase)
                throw new ArgumentException("使用Database验证用户，但是获取密码的SQL为空");

            if (String.IsNullOrEmpty(this.LdapSetting.Path) && ValidateByLdap)
                throw new ArgumentException("使用Ldap验证用户，但是Ldap路径为空");


            var mySerializer = new DataContractSerializer(typeof(Config));
            var myWriter = new FileStream(_filePath, FileMode.Create);
            mySerializer.WriteObject(myWriter, this);
            myWriter.Close();
        }
    }
}