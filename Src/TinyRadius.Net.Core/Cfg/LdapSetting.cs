using System;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Runtime.Serialization;

namespace TinyRadius.Net.Cfg
{
    [DataContract]
    public class LdapSetting
    {
        [DataMember]
        public string Server { get; set; }

        [DataMember]
        public string Path { get; set; }

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
            var connection = new LdapConnection(Server);
            if (!AnonymousLogin)
            {
                connection.Credential = new NetworkCredential(CredentialUserName, CredentialPassword);
            }

            var searchRequestion = new SearchRequest(Path, "uid=" + username, SearchScope.Subtree);
            connection.SessionOptions.ProtocolVersion = 3;

            if (IsSsl)
            {
                connection.SessionOptions.SecureSocketLayer = true;
            }

            var searchResult = (SearchResponse)connection.SendRequest(searchRequestion);
            if (searchResult.Entries.Count == 0)
                return false;
            SearchResultEntry entry = searchResult.Entries[0];
            string dn = entry.DistinguishedName;

            connection.Credential = new NetworkCredential(dn, pwd);
            try
            {
                connection.Bind();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}