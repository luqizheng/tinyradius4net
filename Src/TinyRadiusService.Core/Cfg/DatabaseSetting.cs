using System;
using System.Runtime.Serialization;

namespace TinyRadiusService.Cfg
{
    [DataContract]
    public class DatabaseSetting
    {
        public DatabaseSetting()
        {
            Connection = @"Data Source=.\\SQLEXPRESS;Initial Catalog=YiDong;Persist Security Info=True;User ID=sa;Password=sa";
            PasswordSql = "select password from users where phone=@username";
            MacSql = "SELECT count(*) FROM TrustMAC where MacAddress like @mac and userphone=@userName and TrustDelete='存在'";
        }

        [DataMember]
        public string Connection { get; set; }

        [DataMember]
        public string PasswordSql { get; set; }
        [DataMember]
        public string MacSql
        {
            get; set;
        }
    }
}