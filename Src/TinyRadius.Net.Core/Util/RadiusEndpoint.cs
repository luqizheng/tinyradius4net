using System;
using System.Net;

namespace TinyRadius.Net.Util
{
    /// <summary>
    /// This class stores information about a Radius endpoint.
    /// This includes the address of the remote endpoint and the shared secret
    /// used for securing the communication.
    /// </summary>
    public class RadiusEndpoint
    {
        /// <summary>
        /// Constructs a RadiusEndpoint object.
        /// @param remoteAddress remote address (ip and port number)
        /// @param sharedSecret shared secret
        /// </summary>
        public RadiusEndpoint(string hostName, int port, string shareSecret)
        {
            IPAddress ipAddress = Dns.GetHostEntry(hostName).AddressList[0];
            EndpointAddress = new IPEndPoint(ipAddress, port);
            SharedSecret = shareSecret;
        }

        public RadiusEndpoint(IPEndPoint remoteAddress, String sharedSecret)
        {
            EndpointAddress = remoteAddress;
            SharedSecret = sharedSecret;
        }

        /// <summary>
        /// Returns the remote address.
        /// @return remote address
        /// </summary>
        public IPEndPoint EndpointAddress { get; private set; }

        /// <summary>
        /// Returns the shared secret.
        /// @return shared secret
        /// </summary>
        public string SharedSecret { get; private set; }
    }
}