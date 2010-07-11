using TinyRadius.Net.Packet;
using TinyRadius.Net.Util;

namespace TinyRadius.Net.Proxy
{

    ///<summary>
    ///This class stores information about a proxied packet.
    ///It contains two RadiusEndpoint objects representing the Radius client
    ///and server, the port number the proxied packet arrived
    ///at originally and the proxied packet itself.
    /// </summary>
    public class RadiusProxyConnection
    {
        /**
         * Creates a RadiusProxyConnection object.
         * @param radiusServer server endpoint
         * @param radiusClient client endpoint
         * @param port port the proxied packet arrived at originally 
         */

        public RadiusProxyConnection(RadiusEndpoint radiusServer, RadiusEndpoint radiusClient, RadiusPacket packet,
                                     int port)
        {
            RadiusServer = radiusServer;
            RadiusClient = radiusClient;
            Packet = packet;
            Port = port;
        }

        /**
         * Returns the Radius endpoint of the client.
         * @return endpoint
         */

        public RadiusEndpoint RadiusClient { get; private set; }

        /**
         * Returns the Radius endpoint of the server.
         * @return endpoint
         */

        public RadiusEndpoint RadiusServer { get; private set; }

        /**
         * Returns the proxied packet.
         * @return packet 
         */

        public RadiusPacket Packet { get; private set; }

        /**
         * Returns the port number the proxied packet arrived at
         * originally. 
         * @return port number
         */

        public int Port { get; private set; }
    }
}