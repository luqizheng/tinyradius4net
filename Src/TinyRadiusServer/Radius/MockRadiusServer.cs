using System.Net;
using TinyRadius.Net.Util;

namespace TinyRadiusServer.Radius
{
    internal class MockRadiusServer : RadiusServer
    {
        public override string GetSharedSecret(IPEndPoint client)
        {
            return ClientSets.Instance[client.Address.ToString()];
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
    }
}