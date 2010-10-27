using System;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Runtime.Serialization;
using log4net;

namespace TinyRadiusService.Cfg
{
    [DataContract]
    public class LdapSetting
    {
        public LdapSetting()
        {
            Server = "10.243.1.123";
            CredentialUserName = "cn=Directory Manager";
            SearchUserPath = "dc=gmcc,dc=net";
            CredentialPassword = "";
        }

        [DataMember]
        public string Server { get; set; }

        [DataMember]
        public string SearchUserPath { get; set; }

        [DataMember]
        public string CredentialUserName { get; set; }

        [DataMember]
        public string CredentialPassword { get; set; }

        [DataMember]
        public bool IsSsl { get; set; }

        [DataMember]
        public bool AnonymousLogin { get; set; }

        public bool IsAuthenticated(string username, string pwd)
        {
            ILog log = LogManager.GetLogger(GetType());
            try
            {
                log.InfoFormat("连接Ldap服务器,server是{0}", Server);
                var connection = new LdapConnection(Server)
                                     {
                                         AuthType = AuthType.Basic
                                     };
                connection.SessionOptions.ProtocolVersion = 3;

                if (!AnonymousLogin)
                {
                    log.InfoFormat("使用Credential账户是{0},密码是{1}", CredentialUserName, CredentialPassword);
                    connection.Credential = new NetworkCredential(CredentialUserName, CredentialPassword ?? "");
                }

                if (IsSsl)
                {
                    log.Info("使用SSL连接");
                    connection.SessionOptions.SecureSocketLayer = true;
                }
                
                log.DebugFormat("创建SearchRequest,distinguishedName是{0},filter是{1}", SearchUserPath, "uid=" + username);
                var searchRequestion = new SearchRequest(SearchUserPath, "uid=" + username, SearchScope.Subtree);

                var searchResult = (SearchResponse)connection.SendRequest(searchRequestion, new TimeSpan(0, 0, 0, 30));
                if (searchResult.Entries.Count == 0)
                {
                    log.InfoFormat("无法通过找到用户.distinguishedName是{0},filter是{1}", SearchUserPath, "uid=" + username);
                    return false;
                }
                SearchResultEntry entry = searchResult.Entries[0];
                string dn = entry.DistinguishedName;
                log.InfoFormat("DN是{0}", dn);

                connection.Credential = new NetworkCredential(dn, pwd);


                connection.Bind();
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                return false;
            }
        }
    }
}