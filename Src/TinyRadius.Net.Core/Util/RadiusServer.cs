using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using log4net;
using TinyRadius.Net.Attributes;
using TinyRadius.Net.JavaHelper;
using TinyRadius.Net.Net.JavaHelper;
using TinyRadius.Net.packet;
using TinyRadius.Net.Packet;

namespace TinyRadius.Net.Util
{
    /**
     * Implements a simple Radius server. This class must be subclassed to
     * provide an implementation for getSharedSecret() and getUserPassword().
     * If the server supports accounting, it must override
     * accountingRequestReceived().
     */

    public abstract class RadiusServer
    {
        /**
         * Returns the shared secret used to communicate with the client with the
         * passed IP address or null if the client is not allowed at this server.
         * @param client IP address and port number of client
         * @return shared secret or null
         */
        private static readonly ILog logger = LogManager.GetLogger(typeof (RadiusServer));
        private readonly ArrayList receivedPackets = new ArrayList();
        private int acctPort = 1813;
        private DatagramSocket acctSocket;
        private int authPort = 1812;
        private DatagramSocket authSocket;
        private bool closing;
        private long duplicateInterval = 30000; // 30 s
        private InetAddress listenAddress;
        private int socketTimeout = 3000;
        public abstract String GetSharedSecret(InetSocketAddress client);

        /**
         * Returns the password of the passed user. Either this
         * method or accessRequestReceived() should be overriden.
         * @param userName user name
         * @return plain-text password or null if user unknown
         */
        public abstract String GetUserPassword(String userName);

        /**
         * Constructs an answer for an Access-Request packet. Either this
         * method or isUserAuthenticated should be overriden.
         * @param accessRequest Radius request packet
         * @param client address of Radius client
         * @return response packet or null if no packet shall be sent
         * @exception RadiusException malformed request packet; if this
         * exception is thrown, no answer will be sent
         */

        public RadiusPacket accessRequestReceived(AccessRequest accessRequest, IPAddress client)
        {
            String plaintext = GetUserPassword(accessRequest.UserName);
            int type = RadiusPacket.AccessReject;
            if (plaintext != null && accessRequest.VerifyPassword(plaintext))
                type = RadiusPacket.AccessAccept;

            var answer = new RadiusPacket(type, accessRequest.Identifier);
            copyProxyState(accessRequest, answer);
            return answer;
        }

        /**
         * Constructs an answer for an Accounting-Request packet. This method
         * should be overriden if accounting is supported.
         * @param accountingRequest Radius request packet
         * @param client address of Radius client
         * @return response packet or null if no packet shall be sent
         * @exception RadiusException malformed request packet; if this
         * exception is thrown, no answer will be sent
         */

        public RadiusPacket AccountingRequestReceived(AccountingRequest accountingRequest, InetSocketAddress client)
        {
            var answer = new RadiusPacket(RadiusPacket.AccountingResponse, accountingRequest.Identifier);
            copyProxyState(accountingRequest, answer);
            return answer;
        }

        /**
         * Starts the Radius server.
         * @param listenAuth open auth port?
         * @param listenAcct open acct port?
         */

        public void Start(bool listenAuth, bool listenAcct)
        {
            if (listenAuth)
            {
                ThreadPool.QueueUserWorkItem(delegate
                                                 {
                                                     setName("Radius Auth Listener");
                                                     try
                                                     {
                                                         logger.Info("starting RadiusAuthListener on port " +
                                                                     getAuthPort());
                                                         listenAuth();
                                                         logger.Info("RadiusAuthListener is being terminated");
                                                     }
                                                     catch (Exception e)
                                                     {
                                                         logger.Fatal("auth thread stopped by exception", e);
                                                     }
                                                     finally
                                                     {
                                                         authSocket.close();
                                                         logger.Debug("auth socket closed");
                                                     }
                                                 });
                /*new Thread() {
                    public void run() {
                        setName("Radius Auth Listener");
                        try {
                            logger.Info("starting RadiusAuthListener on port " + getAuthPort());
                            listenAuth();
                            logger.Info("RadiusAuthListener is being terminated");
                        } catch(Exception e) {
                            e.printStackTrace();
                            logger.fatal("auth thread stopped by exception", e);
                        } finally {
                            authSocket.close();
                            logger.debug("auth socket closed");
                        }
                    }
                }.start();*/
            }

            if (listenAcct)
            {
                ThreadPool.QueueUserWorkItem(delegate
                                                 {
                                                     setName("Radius Acct Listener");
                                                     try
                                                     {
                                                         logger.Info("starting RadiusAcctListener on port " +
                                                                     getAcctPort());
                                                         listenAcct();
                                                         logger.Info("RadiusAcctListener is being terminated");
                                                     }
                                                     catch (Exception e)
                                                     {
                                                         logger.Fatal("acct thread stopped by exception", e);
                                                     }
                                                     finally
                                                     {
                                                         acctSocket.close();
                                                         logger.Debug("acct socket closed");
                                                     }
                                                 });
                /*new Thread() {
                    public void run() {
                        setName("Radius Acct Listener");
                        try {
                            logger.Info("starting RadiusAcctListener on port " + getAcctPort());
                            listenAcct();
                            logger.Info("RadiusAcctListener is being terminated");
                        } catch(Exception e) {
                            e.printStackTrace();
                            logger.fatal("acct thread stopped by exception", e);
                        } finally {
                            acctSocket.close();
                            logger.debug("acct socket closed");
                        }
                    }
                }.start();*/
            }
        }

        /**
         * Stops the server and closes the sockets.
         */

        public void stop()
        {
            logger.Info("stopping Radius server");
            closing = true;
            if (authSocket != null)
                authSocket.close();
            if (acctSocket != null)
                acctSocket.close();
        }

        /**
         * Returns the auth port the server will listen on.
         * @return auth port
         */

        public int getAuthPort()
        {
            return authPort;
        }

        /**
         * Sets the auth port the server will listen on.
         * @param authPort auth port, 1-65535
         */

        public void setAuthPort(int authPort)
        {
            if (authPort < 1 || authPort > 65535)
                throw new ArgumentException("bad port number");
            this.authPort = authPort;
            authSocket = null;
        }

        /**
         * Returns the socket timeout (ms).
         * @return socket timeout
         */

        public int getSocketTimeout()
        {
            return socketTimeout;
        }

        /**
         * Sets the socket timeout.
         * @param socketTimeout socket timeout, >0 ms
         * @throws SocketException
         */

        public void setSocketTimeout(int socketTimeout)
        {
            if (socketTimeout < 1)
                throw new ArgumentException("socket tiemout must be positive");
            this.socketTimeout = socketTimeout;
            if (authSocket != null)
                authSocket.setSoTimeout(socketTimeout);
            if (acctSocket != null)
                acctSocket.setSoTimeout(socketTimeout);
        }

        /**
         * Sets the acct port the server will listen on.
         * @param acctPort acct port 1-65535
         */

        public void setAcctPort(int acctPort)
        {
            if (acctPort < 1 || acctPort > 65535)
                throw new ArgumentException("bad port number");
            this.acctPort = acctPort;
            acctSocket = null;
        }

        /**
         * Returns the acct port the server will listen on.
         * @return acct port
         */

        public int getAcctPort()
        {
            return acctPort;
        }

        /**
         * Returns the duplicate interval in ms.
         * A packet is discarded as a duplicate if in the duplicate interval
         * there was another packet with the same identifier originating from the
         * same address.
         * @return duplicate interval (ms)
         */

        public long getDuplicateInterval()
        {
            return duplicateInterval;
        }

        /**
         * Sets the duplicate interval in ms.
         * A packet is discarded as a duplicate if in the duplicate interval
         * there was another packet with the same identifier originating from the
         * same address.
         * @param duplicateInterval duplicate interval (ms), >0
         */

        public void setDuplicateInterval(long duplicateInterval)
        {
            if (duplicateInterval <= 0)
                throw new ArgumentException("duplicate interval must be positive");
            this.duplicateInterval = duplicateInterval;
        }

        /**
         * Returns the IP address the server listens on.
         * Returns null if listening on the wildcard address.
         * @return listen address or null
         */

        public InetAddress getListenAddress()
        {
            return listenAddress;
        }

        /**
         * Sets the address the server listens on.
         * Must be called before start().
         * Defaults to null, meaning listen on every
         * local address (wildcard address).
         * @param listenAddress listen address or null
         */

        public void setListenAddress(InetAddress listenAddress)
        {
            this.listenAddress = listenAddress;
        }

        /**
         * Copies all Proxy-State attributes from the request
         * packet to the response packet.
         * @param request request packet
         * @param answer response packet
         */

        protected void copyProxyState(RadiusPacket request, RadiusPacket answer)
        {
            IList<RadiusAttribute> proxyStateAttrs = request.GetAttributes(33);
            //for (Iterator i = proxyStateAttrs.iterator(); i.hasNext(); )
            foreach (RadiusAttribute proxyStateAttr in proxyStateAttrs)
            {
                //RadiusAttribute proxyStateAttr = (RadiusAttribute)i.next();
                answer.AddAttribute(proxyStateAttr);
            }
        }

        /**
         * Listens on the auth port (blocks the current thread).
         * Returns when stop() is called.
         * @throws SocketException
         * @throws InterruptedException
         * 
         */

        protected void ListenAuth()
        {
            Listen(getAuthSocket());
        }

        /**
         * Listens on the acct port (blocks the current thread).
         * Returns when stop() is called.
         * @throws SocketException
         * @throws InterruptedException
         */

        protected void ListenAcct()
        {
            Listen(getAcctSocket());
        }

        /**
         * Listens on the passed socket, blocks until stop() is called.
         * @param s socket to listen on
         */

        protected void Listen(DatagramSocket s)
        {
            var packetIn = new DatagramPacket
                (new byte[RadiusPacket.MaxPacketLength], RadiusPacket.MaxPacketLength);
            while (true)
            {
                try
                {
                    // receive packet
                    try
                    {
                        logger.trace("about to call socket.receive()");
                        s.receive(packetIn);
                        if (logger.isDebugEnabled())
                            logger.debug("receive buffer size = " + s.getReceiveBufferSize());
                    }
                    catch (SocketException se)
                    {
                        if (closing)
                        {
                            // end thread
                            logger.Info("got closing signal - end listen thread");
                            return;
                        }
                        else
                        {
                            // retry s.receive()
                            logger.Error("SocketException during s.receive() -> retry", se);
                            continue;
                        }
                    }

                    // check client
                    var localAddress = (InetSocketAddress) s.getLocalSocketAddress();
                    var remoteAddress = new InetSocketAddress(packetIn.getAddress(), packetIn.getPort());
                    String secret = GetSharedSecret(remoteAddress);
                    if (secret == null)
                    {
                        if (logger.IsInfoEnabled())
                            logger.Info("ignoring packet from unknown client " + remoteAddress +
                                        " received on local address " + localAddress);
                        continue;
                    }

                    // parse packet
                    RadiusPacket request = makeRadiusPacket(packetIn, secret);
                    if (logger.IsInfoEnabled())
                        logger.Info("received packet from " + remoteAddress + " on local address " + localAddress + ": " +
                                    request);

                    // handle packet
                    logger.trace("about to call RadiusServer.handlePacket()");
                    RadiusPacket response = handlePacket(localAddress, remoteAddress, request, secret);

                    // send response
                    if (response != null)
                    {
                        if (logger.IsInfoEnabled())
                            logger.Info("send response: " + response);
                        DatagramPacket packetOut = makeDatagramPacket(response, secret, remoteAddress.getAddress(),
                                                                      packetIn.getPort(), request);
                        s.send(packetOut);
                    }
                    else
                        logger.Info("no response sent");
                }
                catch (SocketTimeoutException ste)
                {
                    // this is expected behaviour
                    logger.trace("normal socket timeout");
                }
                catch (IOException ioe)
                {
                    // error while reading/writing socket
                    logger.Error("communication error", ioe);
                }
                catch (RadiusException re)
                {
                    // malformed packet
                    logger.Error("malformed Radius packet", re);
                }
            }
        }

        /**
         * Handles the received Radius packet and constructs a response. 
         * @param localAddress local address the packet was received on
         * @param remoteAddress remote address the packet was sent by
         * @param request the packet
         * @return response packet or null for no response
         * @throws RadiusException
         */

        protected RadiusPacket handlePacket(InetSocketAddress localAddress,
                                            InetSocketAddress remoteAddress, RadiusPacket request, String sharedSecret)
        {
            RadiusPacket response = null;

            // check for duplicates
            if (!isPacketDuplicate(request, remoteAddress))
            {
                if (localAddress.getPort() == getAuthPort())
                {
                    // handle packets on auth port
                    if (GetType(AccessRequest).IsInstanceOfType(request))
                        response = accessRequestReceived((AccessRequest) request, remoteAddress);
                    else
                        logger.Error("unknown Radius packet type: " + request.Type);
                }
                else if (localAddress.getPort() == getAcctPort())
                {
                    // handle packets on acct port
                    if (typeof (AccountingRequest).IsInstanceOfType(request))
                        response = AccountingRequestReceived((AccountingRequest) request, remoteAddress);
                    else
                        logger.Error("unknown Radius packet type: " + request.Type);
                }
                else
                {
                    // ignore packet on unknown port
                }
            }
            else
                logger.Info("ignore duplicate packet");

            return response;
        }

        /**
         * Returns a socket bound to the auth port.
         * @return socket
         * @throws SocketException
         */

        protected DatagramSocket getAuthSocket()
        {
            if (authSocket == null)
            {
                if (getListenAddress() == null)
                    authSocket = new DatagramSocket(getAuthPort());
                else
                    authSocket = new DatagramSocket(getAuthPort(), getListenAddress());
                authSocket.setSoTimeout(getSocketTimeout());
            }
            return authSocket;
        }

        /**
         * Returns a socket bound to the acct port.
         * @return socket
         * @throws SocketException
         */

        protected DatagramSocket getAcctSocket()
        {
            if (acctSocket == null)
            {
                if (getListenAddress() == null)
                    acctSocket = new DatagramSocket(getAcctPort());
                else
                    acctSocket = new DatagramSocket(getAcctPort(), getListenAddress());
                acctSocket.setSoTimeout(getSocketTimeout());
            }
            return acctSocket;
        }

        /**
         * Creates a Radius response datagram packet from a RadiusPacket to be send. 
         * @param packet RadiusPacket
         * @param secret shared secret to encode packet
         * @param address where to send the packet
         * @param port destination port
         * @param request request packet
         * @return new datagram packet
         * @throws IOException
         */

        protected DatagramPacket makeDatagramPacket(RadiusPacket packet, String secret, InetAddress address, int port,
                                                    RadiusPacket request)
        {
            var bos = new ByteArrayOutputStream();
            packet.EncodeResponsePacket(bos, secret, request);
            byte[] data = bos.toByteArray();
            var datagram = new DatagramPacket(data, data.Length, address, port);
            return datagram;
        }

        /**
         * Creates a RadiusPacket for a Radius request from a received
         * datagram packet.
         * @param packet received datagram
         * @return RadiusPacket object
         * @exception RadiusException malformed packet
         * @exception IOException communication error (after getRetryCount()
         * retries)
         */

        protected RadiusPacket makeRadiusPacket(DatagramPacket packet, String sharedSecret)
        {
            var @in = new ByteArrayInputStream(packet.getData());
            return RadiusPacket.DecodeRequestPacket(@in, sharedSecret);
        }

        /**
         * Checks whether the passed packet is a duplicate.
         * A packet is duplicate if another packet with the same identifier
         * has been sent from the same host in the last time. 
         * @param packet packet in question
         * @param address client address
         * @return true if it is duplicate
         */

        protected bool isPacketDuplicate(RadiusPacket packet, InetSocketAddress address)
        {
            long now = System.currentTimeMillis();
            long intervalStart = now - getDuplicateInterval();

            byte[] authenticator = packet.Authenticator;

            lock (receivedPackets)
            {
                for (Iterator i = receivedPackets.iterator(); i.hasNext();)
                {
                    var p = (ReceivedPacket) i.next();
                    if (p.receiveTime < intervalStart)
                    {
                        // packet is older than duplicate interval
                        i.remove();
                    }
                    else
                    {
                        if (p.address.equals(address) && p.packetIdentifier == packet.Identifier)
                        {
                            if (authenticator != null && p.authenticator != null)
                            {
                                // packet is duplicate if stored authenticator is equal
                                // to the packet authenticator
                                return Arrays.equals(p.authenticator, authenticator);
                            }
                            else
                            {
                                // should not happen, packet is duplicate
                                return true;
                            }
                        }
                    }
                }

                // add packet to receive list
                var rp = new ReceivedPacket();
                rp.address = address;
                rp.packetIdentifier = packet.Identifier;
                rp.receiveTime = now;
                rp.authenticator = authenticator;
                receivedPackets.add(rp);
            }

            return false;
        }
    }

    /**
     * This internal class represents a packet that has been received by 
     * the server.
     */

    internal class ReceivedPacket
    {
        /**
         * The identifier of the packet.
         */

        /**
         * The address of the host who sent the packet.
         */
        public InetSocketAddress address;

        /**
         * Authenticator of the received packet.
         */
        public byte[] authenticator;
        public int packetIdentifier;

        /**
         * The time the packet was received.
         */
        public long receiveTime;
    }
}