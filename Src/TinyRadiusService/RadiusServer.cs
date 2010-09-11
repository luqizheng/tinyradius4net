using System;
using System.Data.SqlClient;
using System.Net;
using TinyRadius.Net.Packet;

namespace TinyRadiusService
{
    public class RadiusServer : TinyRadius.Net.Util.RadiusServer
    {
        public override string GetSharedSecret(IPEndPoint client)
        {
            if (ServiceCfg.Instance.TinyConfig.NasSettings.ContainsKey(client.Address.ToString()))
            {
                return ServiceCfg.Instance.TinyConfig.NasSettings[client.Address.ToString()].SecretKey;
            }
            Logger.Error("Can't find shareKey with " + client.Address);
            return " ";
        }

        public override string GetUserPassword(string userName)
        {
            if (ServiceCfg.Instance.TinyConfig.ValidateByDatabase)
            {
                using (var conn = new SqlConnection(ServiceCfg.Instance.TinyConfig.DatabaseSetting.Connection))
                {
                    conn.Open();
                    SqlCommand comm = conn.CreateCommand();
                    comm.CommandText = ServiceCfg.Instance.TinyConfig.DatabaseSetting.PasswordSql;
                    SqlParameter param = comm.CreateParameter();
                    param.ParameterName = "@userName";
                    param.Value = userName;
                    comm.Parameters.Add(param);
                    comm.ExecuteScalar().ToString();
                }
            }
            throw new ApplicationException("Please enable LDAP validation or database validation");
        }

        public override RadiusPacket AccessRequestReceived(AccessRequest accessRequest, IPEndPoint client)
        {
            if (ServiceCfg.Instance.TinyConfig.ValidateByLdap)
            {
                int type = RadiusPacket.AccessReject;

                if (ServiceCfg.Instance.TinyConfig.LdapSetting.IsAuthenticated(accessRequest.UserName, accessRequest.Password))
                {
                    type = RadiusPacket.AccessAccept;
                }

                if (type == RadiusPacket.AccessAccept)
                {
                    var answer = new RadiusPacket(type, accessRequest.Identifier);
                    CopyProxyState(accessRequest, answer);
                    return answer;
                }
            }
            return base.AccessRequestReceived(accessRequest, client);
        }
    }
}