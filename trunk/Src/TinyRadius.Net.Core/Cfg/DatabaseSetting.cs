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
        [DataMember]
        public string Connection { get; set; }
        [DataMember]
        public string PasswordSql { get; set; }
    }
}
