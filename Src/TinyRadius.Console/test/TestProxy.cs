/**
 * $Id: TestProxy.java,v 1.1 2005/09/07 22:19:01 wuttke Exp $
 * Created on 07.09.2005
 * @author Matthias Wuttke
 * @version $Revision: 1.1 $
 */
namespace TinyRadius.test
{

    using java.net.InetAddress;
    using java.net.InetSocketAddress;
    using java.net.UnknownHostException;

    using TinyRadius.packet.AccountingRequest;
    using TinyRadius.packet.RadiusPacket;
    using TinyRadius.proxy.RadiusProxy;
    using TinyRadius.util.RadiusEndpoint;

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

        public RadiusEndpoint getProxyServer(RadiusPacket packet,
                RadiusEndpoint client) {
		// always proxy
		try {
			InetAddress address = InetAddress.getByAddress(new byte[]{127,0,0,1});
			int port = 10000;
			if (packet instanceof AccountingRequest)
				port = 10001;
			return new RadiusEndpoint(new InetSocketAddress(address, port), "testing123");
		} catch (UnknownHostException uhe) {
			uhe.printStackTrace();
			return null;
		}
	}

        public String getSharedSecret(InetSocketAddress client)
        {
            if (client.getPort() == 10000 || client.getPort() == 10001)
                return "testing123";
            else if (client.getAddress().getHostAddress().equals("127.0.0.1"))
                return "proxytest";
            else
                return null;
        }

        public String getUserPassword(String userName)
        {
            // not used because every request is proxied
            return null;
        }

        public static void main(String[] args)
        {
            new TestProxy().start(true, true, true);
        }

    }
}