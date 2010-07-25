using System.Net;
using TinyRadius.Net.Cfg;
using TinyRadius.Net.Util;

namespace TinyRadiusService
{
    internal class RadiusServer : TinyRadius.Net.Util.RadiusServer
    {
        public override string GetSharedSecret(IPEndPoint client)
        {
            //return ClientSets.Instance[client.Address.ToString()];
            var cfg = new Config("");
            return cfg.NasSettings[client.Address.ToString()].ToString();
        }

        public override string GetUserPassword(string userName)
        {
            switch (userName)
            {
                case "a":
                    return "a";
                default:
                    return "a";
            }
        }

        //public override RadiusPacket AccessRequestReceived(AccessRequest accessRequest, IPEndPoint client)
        //{
        //    string struser = accessRequest.UserName;
        //    string strpwd = accessRequest.Password;
        //    string str = "LDAP://yourserver/CN=username,CN=users,DC=yourdnsname,DC=CN ";
        //    DirectoryEntry de = new DirectoryEntry(str, "yourdnsname\\ " + struser, strpwd,
        //                                           AuthenticationTypes.ServerBind);

        //    int type = RadiusPacket.AccessReject;

        //    try
        //    {
        //        string guid = de.Guid.ToString();
        //        type = RadiusPacket.AccessAccept;
        //    }
        //    catch (System.Exception ex)
        //    {

        //    }

        //    var answer = new RadiusPacket(type, accessRequest.Identifier);
        //    CopyProxyState(accessRequest, answer);
        //    return answer;


        //}
    }
}