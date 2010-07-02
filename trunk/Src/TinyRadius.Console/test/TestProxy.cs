/**
 * $Id: TestProxy.java,v 1.1 2005/09/07 22:19:01 wuttke Exp $
 * Created on 07.09.2005
 * @author Matthias Wuttke
 * @version $Revision: 1.1 $
 */
using System;
using System.Net;
using TinyRadius.Net.packet;
using TinyRadius.Net.Packet;
using TinyRadius.Net.Proxy;
using TinyRadius.Net.Util;

namespace TinyRadius.test
{

    /**
     * Test proxy server.
     * Listens on localhost:1812 and localhost:1813. Proxies every access request
     * to localhost:10000 and every accounting request to localhost:10001.
     * You can use TestClient to ask this TestProxy and TestServer
     * with the parameters 10000 and 10001 as the target server.
     * Uses "testing123" as the shared secret for the communication with the
     * target server (localhost:10000/localhost:10001) and "proxytest" as the
     * shared secret for the communication with connecting clients.
     */
    public class TestProxy : RadiusProxy
    {

        public override RadiusEndpoint GetProxyServer(RadiusPacket packet,
                RadiusEndpoint client)
        {
            // always proxy
            try
            {
                var address = IPAddress.Parse("127.0. 0. 1 ");
                int port = 10000;
                if (typeof(AccountingRequest).IsInstanceOfType(packet))
                    port = 10001;
                return new RadiusEndpoint(new IPEndPoint(address, port), "testing123");
            }
            catch
            {
                return null;
            }
        }


        public static void main(String[] args)
        {
            new TestProxy().Start(true, true, true);
        }

        public override string GetSharedSecret(IPEndPoint client)
        {
            if (client.Port == 10000 || client.Port == 10001)
                return "testing123";
            else if (client.Address.ToString() == "127.0.0.1")
                return "proxytest";
            else
                return null;
        }

        public override string GetUserPassword(string userName)
        {
            return null;
        }
    }
}