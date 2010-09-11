using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using log4net;
using TinyRadius.Net.Attributes;
using TinyRadius.Net.Packet;

namespace TinyRadius.Net.Util
{
    /// <summary>
    /// Implements a simple Radius server. This class must be subclassed to
    /// provide an implementation for getSharedSecret() and getUserPassword().
    /// If the server supports accounting, it must override
    /// accountingRequestReceived().
    /// </summary>
    public abstract class RadiusServer
    {
        /// <summary>
        /// Returns the shared secret used to communicate with the client with the
        /// passed IP address or null if the client is not allowed at this server.
        /// @param client IP address and port number of client
        /// @return shared secret or null
        /// </summary>
        protected readonly ILog Logger;

        private readonly List<ReceivedPacket> _receivedPackets = new List<ReceivedPacket>();
        private int _acctPort = 1813;
        private UdpClient _acctSocket;
        private int _authPort = 1812;
        private UdpClient _authSocket;
        private int _duplicateInterval = 30000; // 30 s
        private int _socketTimeout = 3000;

        protected RadiusServer()
        {
            this.Logger = LogManager.GetLogger(this.GetType());
        }
        /// <summary>
        /// Returns the auth port the server will listen on.
        /// @return auth port
        /// </summary>
        public int AuthPort
        {
            get { return _authPort; }
            set
            {
                if (value < 1 || value > 65535)
                    throw new ArgumentException("bad port number");
                _authPort = value;
                _authSocket = null;
            }
        }

        /// <summary>
        /// Sets the auth port the server will listen on.
        /// @param authPort auth port, 1-65535
        /// </summary>
        /// <summary>
        /// Returns the socket timeout (ms).
        /// @return socket timeout
        /// </summary>
        public virtual int SocketTimeout
        {
            get { return _socketTimeout; }
            set
            {
                if (value < 1)
                    throw new ArgumentException("socket tiemout must be positive");
                _socketTimeout = value;
                if (_authSocket != null)
                    _authSocket.Client.ReceiveTimeout = value;
                if (_acctSocket != null)
                    _acctSocket.Client.ReceiveTimeout = value;
            }
        }

        /// <summary>
        /// Sets the socket timeout.
        /// @param socketTimeout socket timeout, >0 ms
        /// @throws SocketException
        /// </summary>
        /// <summary>
        /// Sets the acct port the server will listen on.
        /// @param acctPort acct port 1-65535
        /// </summary>
        public int AcctPort
        {
            set
            {
                if (value < 1 || value > 65535)
                    throw new ArgumentOutOfRangeException("value", "bad port number");
                _acctPort = value;
                _acctSocket = null;
            }
            get { return _acctPort; }
        }


        /// <summary>
        /// Returns the duplicate interval in ms.
        /// A packet is discarded as a duplicate if in the duplicate interval
        /// there was another packet with the same identifier originating from the
        /// same address.
        /// @return duplicate interval (ms)
        /// </summary>
        public int DuplicateInterval
        {
            get { return _duplicateInterval; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value", "DuplicateInterval must be positive");
                _duplicateInterval = value;
            }
        }

        /// <summary>
        /// Returns the IP address the server listens on.
        /// Returns null if listening on the wildcard address.
        /// @return listen address or null
        /// </summary>
        public IPAddress ListenAuthIp { get; set; }

        public IPAddress ListenAccountIp { get; set; }

        public abstract String GetSharedSecret(IPEndPoint client);

        /// <summary>
        /// Returns the password of the passed user. Either this
        /// method or AccessRequestReceived() should be overriden.
        /// @param userName user name
        /// @return plain-text password or null if user unknown
        /// </summary>
        public abstract String GetUserPassword(String userName);

        /// <summary>
        /// Constructs an answer for an Access-Request packet. Either this
        /// method or isUserAuthenticated should be overriden.
        /// @param accessRequest Radius request packet
        /// @param client address of Radius client
        /// @return response packet or null if no packet shall be sent
        /// @exception RadiusException malformed request packet; if this
        /// exception is thrown, no answer will be sent
        /// </summary>
        public virtual RadiusPacket AccessRequestReceived(AccessRequest accessRequest, IPEndPoint client)
        {
            String plaintext = GetUserPassword(accessRequest.UserName);
            int type = RadiusPacket.AccessReject;
            if (plaintext != null && accessRequest.VerifyPassword(plaintext))
                type = RadiusPacket.AccessAccept;

            var answer = new RadiusPacket(type, accessRequest.Identifier);
            CopyProxyState(accessRequest, answer);
            return answer;
        }

        /// <summary>
        /// Constructs an answer for an Accounting-Request packet. This method
        /// should be overriden if accounting is supported.
        /// @param accountingRequest Radius request packet
        /// @param client address of Radius client
        /// @return response packet or null if no packet shall be sent
        /// @exception RadiusException malformed request packet; if this
        /// exception is thrown, no answer will be sent
        /// </summary>
        public RadiusPacket AccountingRequestReceived(AccountingRequest accountingRequest, IPEndPoint client)
        {
            var answer = new RadiusPacket(RadiusPacket.AccountingResponse, accountingRequest.Identifier);
            CopyProxyState(accountingRequest, answer);
            return answer;
        }

        /// <summary>
        /// Starts the Radius server.
        /// @param listenAuth open auth port?
        /// @param listenAcct open acct port?
        /// </summary>
        public virtual void Start(bool listenAuth, bool listenAcct)
        {
            if (listenAuth)
            {
                ThreadPool.QueueUserWorkItem(delegate
                                                 {
                                                     //setName("Radius Auth Listener");
                                                     try
                                                     {
                                                         Logger.Info("starting RadiusAuthListener on port " +
                                                                     AuthPort);
                                                         ListenAuth();
                                                         Logger.Info("RadiusAuthListener is being terminated");
                                                     }
                                                     catch (Exception e)
                                                     {
                                                         Logger.Fatal("auth thread stopped by exception", e);
                                                     }
                                                     finally
                                                     {
                                                         _authSocket.Close();
                                                         Logger.Debug("auth socket closed");
                                                     }
                                                 });
            }

            if (listenAcct)
            {
                ThreadPool.QueueUserWorkItem(delegate
                                                 {
                                                     //setName("Radius Acct Listener");
                                                     try
                                                     {
                                                         Logger.Info("starting RadiusAcctListener on port " +
                                                                     AcctPort);
                                                         ListenAcct();
                                                         Logger.Info("RadiusAcctListener is being terminated");
                                                     }
                                                     catch (Exception e)
                                                     {
                                                         Logger.Fatal("acct thread stopped by exception", e);
                                                     }
                                                     finally
                                                     {
                                                         _acctSocket.Close();
                                                         Logger.Debug("acct socket closed");
                                                     }
                                                 });
            }
        }

        /// <summary>
        /// Stops the server and closes the sockets.
        /// </summary>
        public virtual void Stop()
        {
            Logger.Info("stopping Radius server");
            if (_authSocket != null)
            {
                lock (_authSocket)
                {
                    _authSocket.Close();
                }
            }
            if (_acctSocket != null)
            {
                _acctSocket.Close();
            }
        }

        /// <summary>
        /// Sets the address the server listens on.
        /// Must be called before start().
        /// Defaults to null, meaning listen on every
        /// local address (wildcard address).
        /// @param listenAddress listen address or null
        /// </summary>
        /// <summary>
        /// Copies all Proxy-State attributes from the request
        /// packet to the response packet.
        /// @param request request packet
        /// @param answer response packet
        /// </summary>
        protected void CopyProxyState(RadiusPacket request, RadiusPacket answer)
        {
            IList<RadiusAttribute> proxyStateAttrs = request.GetAttributes(33);
            foreach (RadiusAttribute proxyStateAttr in proxyStateAttrs)
            {
                //RadiusAttribute proxyStateAttr = (RadiusAttribute)i.next();
                answer.AddAttribute(proxyStateAttr);
            }
        }

        /// <summary>
        /// Listens on the auth port (blocks the current thread).
        /// Returns when stop() is called.
        /// @throws SocketException
        /// @throws InterruptedException
        /// 
        /// </summary>
        protected void ListenAuth()
        {
            Listen(GetAuthSocket());
        }

        /// <summary>
        /// Listens on the acct port (blocks the current thread).
        /// Returns when stop() is called.
        /// @throws SocketException
        /// @throws InterruptedException
        /// </summary>
        protected void ListenAcct()
        {
            Listen(GetAcctSocket());
        }

        /// <summary>
        /// Listens on the passed socket, blocks until stop() is called.
        /// @param s socket to listen on
        /// </summary>
        protected void Listen(UdpClient s)
        {
            Logger.Info("about to call socket.receive()");
            while (true)
            {
                var point = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = s.Receive(ref point);
                Receive(data, s, point);
            }
        }

        private void Receive(byte[] data, UdpClient socket, IPEndPoint remoteAddress)
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug("receive buffer size = " + data.Length);

            EndPoint localAddress = socket.Client.LocalEndPoint;
            // check client
            String secret = GetSharedSecret(remoteAddress);
            if (secret == null)
            {
                if (Logger.IsInfoEnabled)
                    Logger.Info("ignoring packet from unknown client " + remoteAddress +
                                " received on local address " + localAddress);
            }

            // parse packet
            RadiusPacket request = MakeRadiusPacket(data, secret);
            if (Logger.IsInfoEnabled)
                Logger.Info("received packet from " + remoteAddress + " on local address " + localAddress + ": " +
                            request);

            // handle packet
            Logger.Debug("about to call RadiusServer.handlePacket()");
            RadiusPacket response = HandlePacket((IPEndPoint)localAddress, remoteAddress, request, secret);

            // send response
            if (response != null)
            {
                if (Logger.IsInfoEnabled)
                    Logger.Info("send response: " + response);
                byte[] packetOut = MakeDatagramPacket(response, secret, request);
                socket.Send(packetOut, packetOut.Length, remoteAddress);
            }
            else
                Logger.Info("no response sent");
        }

        /// <summary>
        /// Handles the received Radius packet and constructs a response. 
        /// @param localAddress local address the packet was received on
        /// @param remoteAddress remote address the packet was sent by
        /// @param request the packet
        /// @return response packet or null for no response
        /// @throws RadiusException
        /// </summary>
        protected virtual RadiusPacket HandlePacket(IPEndPoint localAddress,
                                                    IPEndPoint remoteAddress, RadiusPacket request, String sharedSecret)
        {
            RadiusPacket response = null;

            // check for duplicates
            if (!IsPacketDuplicate(request, remoteAddress))
            {
                if (localAddress.Port == AuthPort)
                {
                    // handle packets on auth port
                    if (typeof(AccessRequest).IsInstanceOfType(request))
                        response = AccessRequestReceived((AccessRequest)request, remoteAddress);
                    else
                        Logger.Error("unknown Radius packet type: " + request.Type);
                }
                else if (localAddress.Port == AcctPort)
                {
                    // handle packets on acct port
                    if (typeof(AccountingRequest).IsInstanceOfType(request))
                        response = AccountingRequestReceived((AccountingRequest)request, remoteAddress);
                    else
                        Logger.Error("unknown Radius packet type: " + request.Type);
                }
                else
                {
                    // ignore packet on unknown port
                }
            }
            else
                Logger.Info("ignore duplicate packet");

            return response;
        }

        /// <summary>
        /// Returns a socket bound to the auth port.
        /// @return socket
        /// @throws SocketException
        /// </summary>
        protected UdpClient GetAuthSocket()
        {
            if (_authSocket == null)
            {
                IPEndPoint ep = ListenAuthIp == null
                                    ? new IPEndPoint(IPAddress.Any, AuthPort)
                                    : new IPEndPoint(ListenAuthIp, AuthPort);
                Logger.Info("Create AuthSocket UdpClient with " + ep.Address + ":" + ep.Port);
                _authSocket = new UdpClient(ep);
            }
            return _authSocket;
        }

        /// <summary>
        /// Returns a socket bound to the acct port.
        /// @return socket
        /// @throws SocketException
        /// </summary>
        protected UdpClient GetAcctSocket()
        {
            if (_acctSocket == null)
            {
                IPEndPoint ep = ListenAccountIp == null
                                    ? new IPEndPoint(IPAddress.Any, AcctPort)
                                    : new IPEndPoint(ListenAccountIp, AcctPort);
                Logger.Info("Create AcctSocket UdpClient with" + ep.Address + ":" + ep.Port);
                _acctSocket = new UdpClient(ep);
            }
            return _acctSocket;
        }

        /// <summary>
        /// Creates a Radius response datagram packet from a RadiusPacket to be send. 
        /// @param packet RadiusPacket
        /// @param secret shared secret to encode packet
        /// @param request request packet
        /// @return new datagram packet
        /// @throws IOException
        /// </summary>
        protected Byte[] MakeDatagramPacket(RadiusPacket packet, String secret, RadiusPacket request)
        {
            var bos = new MemoryStream();
            try
            {
                packet.EncodeResponsePacket(bos, secret, request);
                return bos.ToArray();
            }
            finally
            {
                bos.Close();
                bos.Dispose();
            }
        }

        /// <summary>
        /// Creates a RadiusPacket for a Radius request from a received
        /// datagram packet.
        /// @param packet received datagram
        /// @return RadiusPacket object
        /// @exception RadiusException malformed packet
        /// @exception IOException communication error (after getRetryCount()
        /// retries)
        /// </summary>
        protected RadiusPacket MakeRadiusPacket(byte[] packet, String sharedSecret)
        {
            var @in = new MemoryStream(packet);
            return RadiusPacket.DecodeRequestPacket(@in, sharedSecret);
        }

        /// <summary>
        /// Checks whether the passed packet is a duplicate.
        /// A packet is duplicate if another packet with the same identifier
        /// has been sent from the same host in the last time. 
        /// @param packet packet in question
        /// @param address client address
        /// @return true if it is duplicate
        /// </summary>
        protected bool IsPacketDuplicate(RadiusPacket packet, IPEndPoint address)
        {
            //long now = System.currentTimeMillis();
            DateTime now = DateTime.Now;

            DateTime intervalStart = now - (new TimeSpan(0, 0, 0, 0, DuplicateInterval));

            byte[] authenticator = packet.Authenticator;

            lock (_receivedPackets)
            {
                for (int i = _receivedPackets.Count - 1; i >= 0; i--)
                {
                    ReceivedPacket p = _receivedPackets[i];
                    if (p.ReceiveTime < intervalStart)
                    {
                        _receivedPackets.RemoveAt(i);
                    }
                    else
                    {
                        if (p.Address.Equals(address) && p.PacketIdentifier == packet.Identifier)
                        {
                            return authenticator == null || p.Authenticator == null ||
                                   Equals(p.Authenticator, authenticator);
                        }
                    }
                }
            }

            // add packet to receive list
            var rp = new ReceivedPacket
                         {
                             Address = address,
                             PacketIdentifier = packet.Identifier,
                             ReceiveTime = now,
                             Authenticator = authenticator
                         };
            _receivedPackets.Add(rp);
            return false;
        }
    }

    /// <summary>
    /// This internal class represents a packet that has been received by 
    /// the server.
    /// </summary>
    internal class ReceivedPacket
    {
        /// <summary>
        /// The identifier of the packet.
        /// </summary>
        /// <summary>
        /// The address of the host who sent the packet.
        /// </summary>
        public IPEndPoint Address;

        /// <summary>
        /// Authenticator of the received packet.
        /// </summary>
        public byte[] Authenticator;

        /// <summary>
        /// 
        /// </summary>
        public int PacketIdentifier;

        /// <summary>
        /// The time the packet was received.
        /// </summary>
        public DateTime ReceiveTime;
    }
}