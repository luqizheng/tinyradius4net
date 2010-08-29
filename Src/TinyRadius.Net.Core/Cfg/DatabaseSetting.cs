using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TinyRadius.Net.Cfg
{
    [DataContract]
    public class DatabaseSetting
    {
        public DatabaseSetting()
        {
            this.Connection = @"server=.\sqlexpress;database=nhibernateDemo;uid=sa;pwd=sa";
            PasswordSql = "select password from users where userid=@username";
        }
        [DataMember]
        public string Connection { get; set; }
        [DataMember]
        public string PasswordSql { get; set; }
    }
}
