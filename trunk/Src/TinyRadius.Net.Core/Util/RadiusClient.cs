using System;
using System.IO;
using System.Net.Sockets;
using log4net;
using TinyRadius.Net.packet;
using TinyRadius.Net.Packet;

namespace TinyRadius.Net.Util
{
    /**
     * This object represents a simple Radius client which communicates with
     * a specified Radius server. You can use a single instance of this object
     * to authenticate or account different users with the same Radius server
     * as long as you authenticate/account one user after the other. This object
     * is thread safe, but only opens a single socket so operations using this
     * socket are synchronized to avoid confusion with the mapping of request
     * and result packets.
     */

    public class RadiusClient
    {
        /**
         * Creates a new Radius client object for a special Radius server.
         * @param hostName host name of the Radius server
         * @param sharedSecret shared secret used to secure the communication
         */

        private static readonly ILog logger = LogManager.GetLogger(typeof (RadiusClient));
        private int _acctPort = 1813;
        private int _authPort = 1812;
        private String _hostName;
        private int retryCount = 3;
        private String sharedSecret;
        private Socket socket;
        private int socketTimeout = 3000;

        public RadiusClient(String hostName, String sharedSecret)
        {
            HostName = hostName;
            SetSharedSecret(sharedSecret);
        }

        /**
         * Constructs a Radius client for the given Radius endpoint.
         * @param client Radius endpoint
         */

        public RadiusClient(RadiusEndpoint client)
        {
        }

        public int AuthPort
        {
            set
            {
                if (value < 1 || value > 65535)
                    throw new ArgumentException("bad port number");
                _authPort = value;
            }
        }

        /**
         * Returns the host name of the Radius server.
         * @return host name
         */

        public string HostName
        {
            get { return _hostName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("host name must not be empty");
                _hostName = value;
            }
        }

        public int SocketTimeout
        {
            set
            {
                if (value < 1)
                    throw new ArgumentException("socket tiemout must be positive");
                socketTimeout = value;
                if (socket != null)
                    socket.ReceiveTimeout = value;
            }
        }

        /**
         * Sets the Radius server accounting port.
         * @param acctPort acct port, 1-65535
         */

        public int AcctPort
        {
            set
            {
                if (value < 1 || value > 65535)
                    throw new ArgumentException("bad port number");
                _acctPort = value;
            }
            get { return _acctPort; }
        }

        /**
         * Authenticates a user.
         * @param userName user name
         * @param password password
         * @return true if authentication is successful, false otherwise
         * @exception RadiusException malformed packet
         * @exception IOException communication error (after getRetryCount()
         * retries)
         */

        public bool Authenticate(String userName, String password)
        {
            lock (this)
            {
                var request = new AccessRequest(userName, password);
                RadiusPacket response = Authenticate(request);
                return response.Type == RadiusPacket.AccessAccept;
            }
        }

        /**
         * Sends an Access-Request packet and receives a response
         * packet.
         * @param request request packet
         * @return Radius response packet
         * @exception RadiusException malformed packet
         * @exception IOException communication error (after getRetryCount()
         * retries)
         */

        public RadiusPacket Authenticate(AccessRequest request)
        {
            lock (this)
            {
                if (logger.IsInfoEnabled)
                    logger.Info("send Access-Request packet: " + request);

                RadiusPacket response = Communicate(request, GetAuthPort());
                if (logger.IsInfoEnabled)
                    logger.Info("received packet: " + response);

                return response;
            }
        }

        /**
         * Sends an Accounting-Request packet and receives a response
         * packet.
         * @param request request packet
         * @return Radius response packet
         * @exception RadiusException malformed packet
         * @exception IOException communication error (after getRetryCount()
         * retries)
         */

        public RadiusPacket Account(AccountingRequest request)
        {
            lock (this)
            {
                if (logger.IsInfoEnabled)
                    logger.Info("send Accounting-Request packet: " + request);

                RadiusPacket response = Communicate(request, AcctPort);
                if (logger.IsInfoEnabled)
                    logger.Info("received packet: " + response);

                return response;
            }
        }

        /**
         * Closes the socket of this client.
         */

        public void Close()
        {
            if (socket != null)
                socket.Close();
        }

        /**
         * Returns the Radius server auth port.
         * @return auth port
         */

        public int GetAuthPort()
        {
            return _authPort;
        }
   

        /**
         * Returns the retry count for failed transmissions.
         * @return retry count
         */
        public int GetRetryCount()
        {
            return retryCount;
        }

        /**
         * Sets the retry count for failed transmissions.
         * @param retryCount retry count, >0
         */

        public void SetRetryCount(int retryCount)
        {
            if (retryCount < 1)
                throw new ArgumentException("retry count must be positive");
            this.retryCount = retryCount;
        }

        /**
         * Returns the secret shared between server and client.
         * @return shared secret
         */

        public String GetSharedSecret()
        {
            return sharedSecret;
        }

        /**
         * Sets the secret shared between server and client.
         * @param sharedSecret shared secret
         */

        public void SetSharedSecret(String sharedSecret)
        {
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentException("shared secret must not be empty");
            this.sharedSecret = sharedSecret;
        }

        /**
         * Returns the socket timeout.
         * @return socket timeout, ms
         */

        public int GetSocketTimeout()
        {
            return socketTimeout;
        }

        /**
         * Sets the socket timeout
         * @param socketTimeout timeout, ms, >0
         * @throws SocketException
         */

        /**
         * Sends a Radius packet to the server and awaits an answer.
         * @param request packet to be sent
         * @param port server port number
         * @return response Radius packet
         * @exception RadiusException malformed packet
         * @exception IOException communication error (after getRetryCount()
         * retries)
         */

        public RadiusPacket Communicate(RadiusPacket request, int port)
        {
            var packetIn = new byte[RadiusPacket.MaxPacketLength];
            byte[] packetOut = MakeDatagramPacket(request);

            Socket socket = Socket;
            for (int i = 1; i <= GetRetryCount(); i++)
            {
                try
                {
                    socket.Send(packetOut);
                    socket.Receive(packetIn);
                    return MakeRadiusPacket(packetIn, request);
                }
                catch (IOException ioex)
                {
                    if (i == GetRetryCount())
                    {
                        if (logger.IsErrorEnabled)
                        {
                            logger.Error("communication failure, no more retries", ioex);
                        }
                        throw ioex;
                    }
                    if (logger.IsInfoEnabled)
                        logger.Info("communication failure, retry " + i);
                    // TODO increase Acct-Delay-Time by getSocketTimeout()/1000
                    // this changes the packet authenticator and requires packetOut to be
                    // calculated again (call makeDatagramPacket)
                }
            }

            return null;
        }

        /**
         * Sends the specified packet to the specified Radius server endpoint.
         * @param remoteServer Radius endpoint consisting of server address,
         * port number and shared secret
         * @param request Radius packet to be sent 
         * @return received response packet
         * @throws RadiusException malformed packet
         * @throws IOException error while communication
         */

        public static RadiusPacket Communicate(RadiusEndpoint remoteServer, RadiusPacket request)
        {
            var rc = new RadiusClient(remoteServer);
            return rc.Communicate(request, remoteServer.EndpointAddress.Port);
        }

        /**
         * Returns the socket used for the server communication. It is
         * bound to an arbitrary free local port number.
         * @return local socket
         * @throws SocketException
         */

        protected Socket Socket
        {
            get
            {
                if (socket == null)
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP)
                                 {ReceiveTimeout = GetSocketTimeout()};
                }
                return socket;
            }
        }

        /**
         * Creates a datagram packet from a RadiusPacket to be send. 
         * @param packet RadiusPacket
         * @param port destination port number
         * @return new datagram packet
         * @throws IOException
         */

        protected byte[] MakeDatagramPacket(RadiusPacket packet)
        {
            var bos = new MemoryStream();
            packet.EncodeRequestPacket(bos, GetSharedSecret());
            byte[] data = bos.ToArray();
            bos.Dispose();
            return data;
        }

        /**
         * Creates a RadiusPacket from a received datagram packet.
         * @param packet received datagram
         * @param request Radius request packet
         * @return RadiusPacket object
         */

        protected RadiusPacket MakeRadiusPacket(byte[] data, RadiusPacket request)
        {
            var memoryStream = new MemoryStream(data);
            try
            {
                return RadiusPacket.DecodeResponsePacket(memoryStream, GetSharedSecret(), request);
            }
            finally
            {
                memoryStream.Dispose();
            }
        }
    }
}