using System;
using System.Data.SqlClient;
using System.Net;
using TinyRadius.Net.Packet;
using TinyRadiusConsole;

namespace TinyRadiusService
{
    public class RadiusServer : TinyRadius.Net.Util.RadiusServer
    {
        public override string GetSharedSecret(IPEndPoint client)
        {
            return "123";
            if (ServiceCfg.Instance.TinyConfig.NasSettings.ContainsKey(client.Address.ToString()))
            {
                return ServiceCfg.Instance.TinyConfig.NasSettings[client.Address.ToString()].SecretKey;
            }
            Logger.Error("Can't find shareKey with " + client.Address);
            return " ";
        }

        public override string GetUserPassword(string userName)
        {
            using (var conn = new SqlConnection("Data Source=SKY\\SQLEXPRESS;Initial Catalog=YiDong;Persist Security Info=True;User ID=hiee23;Password=hiee23"))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "select password from users where phone=@username";
                SqlParameter param = comm.CreateParameter();
                param.ParameterName = "@userName";
                param.Value = userName.Trim();
                comm.Parameters.Add(param);
                var result = comm.ExecuteScalar();
                if (result == null)
                {
                    this.Logger.InfoFormat("用户(账户{0})数据登录失败.", userName);
                    return "__Error__Password";
                }
                this.Logger.InfoFormat("用户(账户{0})数据登录成功.", userName);
                return result.ToString();
            }
            throw new ApplicationException("Please enable LDAP validation or database validation");
        }

        public override RadiusPacket AccessRequestReceived(AccessRequest accessRequest, IPEndPoint client)
        {
            /*if (ServiceCfg.Instance.TinyConfig.ValidateByLdap)
            {
                string struser = accessRequest.UserName;
                string strpwd = accessRequest.Password;
                string path = ServiceCfg.Instance.TinyConfig.LdapSetting.Path;

                int type = RadiusPacket.AccessReject;

                var auth = new LdapAuthentication(path);
                if (auth.IsAuthenticated(ServiceCfg.Instance.TinyConfig.LdapSetting.DomainName, struser, strpwd))
                {
                    type = RadiusPacket.AccessAccept;
                }


                if (type == RadiusPacket.AccessAccept)
                {
                    var answer = new RadiusPacket(type, accessRequest.Identifier);
                    CopyProxyState(accessRequest, answer);
                    return answer;
                }
            }*/

            string struser = accessRequest.UserName;
            string strpwd = accessRequest.Password;
            this.Logger.InfoFormat("通过Ldap检查用户,用户{0},密码{1}", struser, strpwd);
            if (!LdapAuthentication.IsAuthenticated(struser, strpwd))
            {
                this.Logger.InfoFormat("用户(账户{0},密码{1})Ldap登录失败,尝试本地数据库登陆", struser, strpwd);
                return base.AccessRequestReceived(accessRequest, client);
            }
            else
            {
                this.Logger.InfoFormat("用户(账户{0},密码{1})Ldap登录成功.", struser, strpwd);
                const int type = RadiusPacket.AccessAccept;
                var answer = new RadiusPacket(type, accessRequest.Identifier);
                CopyProxyState(accessRequest, answer);
                return answer;
            }
        }
    }
}