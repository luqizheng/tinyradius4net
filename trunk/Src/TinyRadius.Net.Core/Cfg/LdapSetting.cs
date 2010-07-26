using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TinyRadius.Net.Cfg
{
    [DataContract]
    public class LdapSetting
    {
        [DataMember]
        public string DomainName
        {
            get;
            set;
        }
        [DataMember]
        public string Path { get; set; }
    }
}
