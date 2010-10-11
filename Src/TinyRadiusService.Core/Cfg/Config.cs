using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using log4net;

namespace TinyRadiusService.Cfg
{
    [DataContract]
    public class Config
    {
        private const string FileName = "TinyServerSetting.xml";
        private static readonly ILog Log = LogManager.GetLogger(typeof (Config));
        private readonly string _filePath;
        private Dictionary<string, NasSetting> _nasSettings = new Dictionary<string, NasSetting>();

        public Config(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }
            DatabaseSetting = new DatabaseSetting();
            LdapSetting = new LdapSetting();
            AuthListentIp = "127.0.0.1";
            AccountListentIp = "127.0.0.1";
            AcctPort = 1813;
            AuthPort = 1812;
            EnableAccount = true;
            EnableAuthentication = true;
            Log.Debug("try to find setting file's path " + path);
            _filePath = Path.Combine(path, FileName);
            Log.Debug("try to find setting file " + _filePath);
            if (File.Exists(_filePath))
            {
                Log.Debug("Read setting from filePath:" + _filePath);
                var mySerializer = new DataContractSerializer(typeof (Config));
                FileStream stream = File.OpenRead(_filePath);
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
            else
            {
                Log.Debug("setting file not find, use default value.");
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
        public Dictionary<string, NasSetting> NasSettings
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

            DatabaseSetting = config.DatabaseSetting ?? new DatabaseSetting();
            LdapSetting = config.LdapSetting ?? new LdapSetting();
        }

        public void Save()
        {
            if (String.IsNullOrEmpty(DatabaseSetting.Connection) && ValidateByDatabase)
                throw new ArgumentException("使用Database验证用户，但是链接字符串为空");
            if (String.IsNullOrEmpty(DatabaseSetting.PasswordSql) && ValidateByDatabase)
                throw new ArgumentException("使用Database验证用户，但是获取密码的SQL为空");

            if (String.IsNullOrEmpty(LdapSetting.SearchUserPath) && ValidateByLdap)
                throw new ArgumentException("使用Ldap验证用户，但是Ldap路径为空");

            if (!File.Exists(_filePath))
            {
                FileStream a = File.Create(_filePath);
                a.Dispose();
            }

            var mySerializer = new DataContractSerializer(typeof (Config));
            using (var myWriter = new FileStream(_filePath, FileMode.Create))
            {
                mySerializer.WriteObject(myWriter, this);
                myWriter.Close();
            }
        }
    }
}