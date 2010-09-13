using System;
using System.DirectoryServices.Protocols;
using System.Net;

namespace TinyRadiusConsole
{
    public static class LdapAuthentication
    {
        //private string _path;
        //private string _filterAttribute;
        //private ILog log;
        //public LdapAuthentication(string path)
        //{
        //    _path = path;
        //    log = LogManager.GetLogger(this.GetType());
        //}

        public static bool IsAuthenticated(string username, string pwd)
        {
            //string domainAndUsername = (String.IsNullOrEmpty(domain) ? "" : @"\") + username;
            try
            {
                var credential = new NetworkCredential("cn=Directory Manager", "");
                var entry = new LdapConnection("10.243.1.123")
                                {
                                    AuthType = AuthType.Basic,
                                    Credential = credential
                                };
                entry.SessionOptions.ProtocolVersion = 3;
                entry.Bind();
                var searchRequest = new SearchRequest("dc=gmcc,dc=net", "uid=" + username, SearchScope.Subtree);
                var a = (SearchResponse)entry.SendRequest(searchRequest, new TimeSpan(0, 0, 0, 30));
                if (a.Entries.Count == 0)
                    return false;
                try
                {
                    var newC = new NetworkCredential(a.Entries[0].DistinguishedName, pwd);
                    entry.Credential = newC;
                    entry.Bind();
                }
                catch
                {
                    return false;
                }
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception("Error authenticating user. " + ex.Message);
            }
        }

        //public string GetGroups()
        //{
        //    DirectorySearcher search = new DirectorySearcher(_path);
        //    search.Filter = "(cn=" + _filterAttribute + ")";
        //    search.PropertiesToLoad.Add("memberOf");
        //    StringBuilder groupNames = new StringBuilder();

        //    try
        //    {
        //        SearchResult result = search.FindOne();
        //        int propertyCount = result.Properties["memberOf"].Count;
        //        string dn;
        //        int equalsIndex, commaIndex;

        //        for (int propertyCounter = 0; propertyCounter < propertyCount; propertyCounter++)
        //        {
        //            dn = (string)result.Properties["memberOf"][propertyCounter];
        //            equalsIndex = dn.IndexOf("=", 1);
        //            commaIndex = dn.IndexOf(",", 1);
        //            if (-1 == equalsIndex)
        //            {
        //                return null;
        //            }
        //            groupNames.Append(dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1));
        //            groupNames.Append("|");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error obtaining group names. " + ex.Message);
        //    }
        //    return groupNames.ToString();
        //}
    }
}
