using System;
using System.Data.SqlClient;
using System.Net;
using log4net.Config;
using Qi.Net;
using TinyRadius.Net.Attributes;
using TinyRadius.Net.Packet;
using TinyRadiusService.Cfg;

[assembly: XmlConfigurator(Watch = true)]

namespace TinyRadiusService
{
    public class RadiusServer : TinyRadius.Net.Util.RadiusServer
    {
        public override string GetSharedSecret(IPEndPoint client)
        {
            Logger.Debug("Client IP is " + client.Address.ToString());

            if (ServiceCfg.Instance.TinyConfig.NasSettings.ContainsKey(client.Address.ToString()))
            {
                return ServiceCfg.Instance.TinyConfig.NasSettings[client.Address.ToString()].SecretKey;
            }
            Logger.Error("Can't find shareKey with " + client.Address);
            return "zhgmcc@123";
        }

        public override string GetUserPassword(string userName)
        {
            using (var conn = new SqlConnection(ServiceCfg.Instance.TinyConfig.DatabaseSetting.Connection))
            {
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = ServiceCfg.Instance.TinyConfig.DatabaseSetting.PasswordSql;
                SqlParameter param = comm.CreateParameter();
                param.ParameterName = "@userName";
                param.Value = userName.Trim();
                comm.Parameters.Add(param);
                object result = comm.ExecuteScalar();
                if (result == null)
                {
                    Logger.InfoFormat("用户(账户{0})数据登录失败.", userName);
                    return null;
                }
                Logger.InfoFormat("用户(账户{0})数据登录成功.", userName);
                return result.ToString();
            }
        }


        public override RadiusPacket AccessRequestReceived(AccessRequest accessRequest, IPEndPoint client)
        {
            try
            {
                string ip = GetIP(accessRequest);
                string macAddr = GetMacAddress(ip);
                RadiusPacket answer;
                if (ServiceCfg.Instance.TinyConfig.ValidateByLdap)
                {
                    Logger.InfoFormat("尝试通过Ldap检查用户,账户:{0},密码:{1},Mac:{2},IP:{3}", accessRequest.UserName,
                                      accessRequest.Password,
                                      macAddr, ip);
                    if (ServiceCfg.Instance.TinyConfig.LdapSetting.IsAuthenticated(accessRequest.UserName,
                                                                                   accessRequest.Password))
                    {
                        Logger.InfoFormat("Ldap登录成功,账户:{0},密码:{1},Mac:{2},IP:{3}", accessRequest.UserName, accessRequest.Password, macAddr, ip);
                        Logger.InfoFormat("{0} login by Ldap success.", accessRequest.UserName);
                        answer = new RadiusPacket(RadiusPacket.AccessAccept, accessRequest.Identifier);
                        CopyProxyState(accessRequest, answer);
                        return answer;
                    }
                    Logger.InfoFormat("Ldap登录失败,账户:{0},密码:{1},Mac:{2},IP:{3}", accessRequest.UserName, accessRequest.Password,
                                      macAddr, ip);
                }

                if (ServiceCfg.Instance.TinyConfig.ValidateByDatabase)
                {
                    Logger.InfoFormat("通过本地数据库检查Mac地址,账户:{0},密码:{1},Mac:{2},IP:{3}", accessRequest.UserName,
                                      accessRequest.Password,
                                      macAddr, ip);
                    Logger.Debug("检查Mac地址");
                    if (!IsMacCorrect(accessRequest.UserName, macAddr))
                    {
                        Logger.InfoFormat("Mac地址不正确,账户:{0},密码:{1},Mac:{2},IP:{3}", accessRequest.UserName,
                                          accessRequest.Password, macAddr, ip);
                        answer = new RadiusPacket(RadiusPacket.AccessReject, accessRequest.Identifier);
                        CopyProxyState(accessRequest, answer);
                        return answer;
                    }
                    return base.AccessRequestReceived(accessRequest, client);
                }
                answer = new RadiusPacket(RadiusPacket.AccessReject, accessRequest.Identifier);
                CopyProxyState(accessRequest, answer);
                return answer;
            }
            catch (Exception ex)
            {
                Logger.Error("some error happend.", ex);
                return new RadiusPacket(RadiusPacket.AccessReject, accessRequest.Identifier);
            }
        }

        private bool IsMacCorrect(string userName, string mac)
        {
            Logger.DebugFormat("Connect db with {0} ", ServiceCfg.Instance.TinyConfig.DatabaseSetting.Connection);
            using (var conn = new SqlConnection(ServiceCfg.Instance.TinyConfig.DatabaseSetting.Connection))
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandText = ServiceCfg.Instance.TinyConfig.DatabaseSetting.MacSql;
                Logger.DebugFormat("通过SQL查找Mac,sql是{0}，用户{1},mac:{2}", comm.CommandText, userName, mac);

                var param = comm.CreateParameter();
                param.ParameterName = "@userName";
                param.Value = userName.Trim();
                comm.Parameters.Add(param);

                param = comm.CreateParameter();
                param.ParameterName = "@mac";
                param.Value = string.Format("%{0}%", mac);
                comm.Parameters.Add(param);
                //  param.Value = mac;
                int result = Convert.ToInt32(comm.ExecuteScalar());
                return result != 0;
            }
        }

        protected string GetIP(AccessRequest accessRequest)
        {
            foreach (RadiusAttribute attr in accessRequest.Attributes)
            {
                if (attr.Type == 31)
                {
                    return attr.Value;
                }
            }
            return null;
        }

        protected string GetMacAddress(string ip)
        {
            try
            {
                return IPAddress.Parse(ip).GetMac().Replace("-", ":");
            }
            catch (Exception ex)
            {
                Logger.Error(ip + " IP is Error", ex);
                throw ex;
            }
        }
    }
}