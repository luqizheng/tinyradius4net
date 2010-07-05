using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using TinyRadius.Net.Util;

namespace TinyRadiusServer.Radius
{
    class MockRadiusServer : RadiusServer
    {
        public override string GetSharedSecret(IPEndPoint client)
        {
            return "123";
            //return ClientSets.Instance[client.Address.ToString()];
        }

        public override string GetUserPassword(string userName)
        {
            switch (userName)
            {
                case "a":
                    return "A";
                default:
                    return "A";

            }

        }
    }
}
