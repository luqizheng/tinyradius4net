using TinyRadius.Net.Attributes;

namespace TinyRadius.Net.Proxy
{
    using TinyRadius.Net.Packet;
    using TinyRadius.Net.Util;
    using System.Threading;
    using TinyRadius.Net.JavaHelper;
    using TinyRadius.Net.Net.JavaHelper;
    using System;
    using System.Collections;
    using log4net;

    /**
     * This class implements a Radius Proxy that receives Radius packets
     * and forwards them to a Radius server.
     * You have to override the method getRadiusProxyConnection() which
     * identifies the Radius Proxy connection a Radius packet belongs to.
     */
    public abstract class RadiusProxy : RadiusServer
    {

        /**
         * Starts the Radius Proxy. Listens on the Proxy port.
         */
        public void start(bool listenAuth, bool listenAcct, bool listenProxy)
        {
            base.Start(listenAuth, listenAcct);
            if (listenProxy)
            {
                ThreadPool.QueueUserWorkItem(delegate(object state)
                {
                    setName("Radius Proxy Listener");
                    try
                    {
                        logger.Info("starting RadiusProxyListener on port " + getProxyPort());
                        Listen(getProxySocket());
                    }
                    catch (Exception e)
                    {
                        e.printStackTrace();
                    }
                });
                /*new Thread() {
                    public void run() {
                        setName("Radius Proxy Listener");
                        try {
                            logger.Info("starting RadiusProxyListener on port " + getProxyPort());
                            listen(getProxySocket());
                        } catch(Exception e) {
                            e.printStackTrace();
                        }
                    }
                }.start();	*/
            }
        }

        /**
         * Stops the Proxy and closes the socket.
         */
        public void stop()
        {
            logger.Info("stopping Radius Proxy");
            if (proxySocket != null)
                proxySocket.close();
            super.stop();
        }

        /**
         * This method must be implemented to return a RadiusEndpoint
         * if the given packet is to be proxied. The endpoint represents the
         * Radius server the packet should be proxied to.
         * @param packet the packet in question
         * @param client the client endpoint the packet originated from
         * (containing the address, port number and shared secret)
         * @return a RadiusEndpoint or null if the packet should not be
         * proxied
         */
        public abstract RadiusEndpoint getProxyServer(RadiusPacket packet, RadiusEndpoint client);

        /**
         * Returns the Proxy port this server listens to.
         * Defaults to 1814.
         * @return Proxy port
         */
        public int getProxyPort()
        {
            return proxyPort;
        }

        /**
         * Sets the Proxy port this server listens to.
         * Please call before start().
         * @param proxyPort Proxy port
         */
        public void setProxyPort(int proxyPort)
        {
            this.proxyPort = proxyPort;
            this.proxySocket = null;
        }

        /**
         * Sets the socket timeout.
         * @param socketTimeout socket timeout, >0 ms
         * @throws SocketException
         */
        public void setSocketTimeout(int socketTimeout)
        {
            base.setSocketTimeout(socketTimeout);
            if (proxySocket != null)
                proxySocket.setSoTimeout(socketTimeout);
        }

        /**
         * Returns a socket bound to the Proxy port.
         * @return socket
         * @throws SocketException
         */
        protected DatagramSocket getProxySocket()
        {
            if (proxySocket == null)
            {
                if (getListenAddress() == null)
                    proxySocket = new DatagramSocket(getProxyPort());
                else
                    proxySocket = new DatagramSocket(getProxyPort(), getListenAddress());
                proxySocket.setSoTimeout(getSocketTimeout());
            }
            return proxySocket;
        }

        /**
         * Handles packets coming in on the Proxy port. Decides whether
         * packets coming in on Auth/Acct ports should be proxied.
         */
        protected RadiusPacket handlePacket(InetSocketAddress localAddress, InetSocketAddress remoteAddress, RadiusPacket request, String sharedSecret)
        {
            // handle incoming Proxy packet
            if (localAddress.getPort() == getProxyPort())
            {
                proxyPacketReceived(request, remoteAddress);
                return null;
            }

            // handle auth/acct packet
            RadiusEndpoint radiusClient = new RadiusEndpoint(remoteAddress, sharedSecret);
            RadiusEndpoint radiusServer = getProxyServer(request, radiusClient);
            if (radiusServer != null)
            {
                // Proxy incoming packet to other radius server
                RadiusProxyConnection proxyConnection = new RadiusProxyConnection(radiusServer, radiusClient, request, localAddress.getPort());
                logger.Info("Proxy packet to " + proxyConnection);
                proxyPacket(request, proxyConnection);
                return null;
            }
            else
                // normal processing
                return super.handlePacket(localAddress, remoteAddress, request, sharedSecret);
        }

        /**
         * Sends an answer to a proxied packet back to the original host.
         * Retrieves the RadiusProxyConnection object from the cache employing
         * the Proxy-State attribute.
         * @param packet packet to be sent back
         * @param remote the server the packet arrived from
         * @throws IOException
         */
        protected void proxyPacketReceived(RadiusPacket packet, InetSocketAddress remote)
        {
            // retrieve my Proxy-State attribute (the last)
            ArrayList proxyStates = packet.GetAttributes(33);
            if (proxyStates == null || proxyStates.size() == 0)
                throw new RadiusException("Proxy packet without Proxy-State attribute");
            RadiusAttribute proxyState = (RadiusAttribute)proxyStates.get(proxyStates.size() - 1);

            // retrieve Proxy connection from cache 
            String state = new String(proxyState.Data);
            RadiusProxyConnection proxyConnection = (RadiusProxyConnection)proxyConnections.remove(state);
            if (proxyConnection == null)
            {
                logger.warn("received packet on Proxy port without saved Proxy connection - duplicate?");
                return;
            }

            // retrieve client
            RadiusEndpoint client = proxyConnection.getRadiusClient();
            if (logger.IsInfoEnabled())
            {
                logger.Info("received Proxy packet: " + packet);
                logger.Info("forward packet to " + client.getEndpointAddress().toString() + " with secret " + client.getSharedSecret());
            }

            // remove only own Proxy-State (last attribute)
            packet.RemoveLastAttribute(33);

            // re-encode answer packet with authenticator of the original packet
            RadiusPacket answer = new RadiusPacket(packet.Type, packet.Identifier, packet.GetAttributes());
            DatagramPacket datagram = makeDatagramPacket(answer, client.getSharedSecret(), client.getEndpointAddress().getAddress(), client.getEndpointAddress().getPort(), proxyConnection.getPacket());

            // send back using correct socket
            DatagramSocket socket;
            if (proxyConnection.getPort() == getAuthPort())
                socket = getAuthSocket();
            else
                socket = getAcctSocket();
            socket.send(datagram);
        }

        /**
         * Proxies the given packet to the server given in the Proxy connection.
         * Stores the Proxy connection object in the cache with a key that
         * is added to the packet in the "Proxy-State" attribute.
         * @param packet the packet to Proxy
         * @param proxyCon the RadiusProxyConnection for this packet
         * @throws IOException
         */
        protected void proxyPacket(RadiusPacket packet, RadiusProxyConnection proxyConnection)
        {
            lock (typeof(RadiusProxy))
            {
                // add Proxy-State attribute
                proxyIndex++;
                String proxyIndexStr = Integer.toString(proxyIndex);
                packet.AddAttribute(new RadiusAttribute(33, proxyIndexStr.getBytes()));

                // store RadiusProxyConnection object
                proxyConnections.put(proxyIndexStr, proxyConnection);
            }

            // get server address
            InetAddress serverAddress = proxyConnection.getRadiusServer().getEndpointAddress().getAddress();
            int serverPort = proxyConnection.getRadiusServer().getEndpointAddress().getPort();
            String serverSecret = proxyConnection.getRadiusServer().getSharedSecret();

            // save request authenticator (will be calculated new)
            byte[] auth = packet.Authenticator;

            // encode new packet (with new authenticator)
            ByteArrayOutputStream bos = new ByteArrayOutputStream();
            packet.EncodeRequestPacket(bos, serverSecret);
            byte[] data = bos.toByteArray();
            DatagramPacket datagram = new DatagramPacket(data, data.Length, serverAddress, serverPort);

            // restore original authenticator
            packet.Authenticator = auth;

            // send packet
            DatagramSocket proxySocket = getProxySocket();
            proxySocket.send(datagram);
        }

        /**
         * Index for Proxy-State attribute.
         */
        private int proxyIndex = 1;

        /**
         * Cache for Radius Proxy connections belonging to sent packets
         * without a received response.
         * Key: Proxy Index (String), Value: RadiusProxyConnection
         */
        private Hashtable proxyConnections = new Hashtable();

        private int proxyPort = 1814;
        private DatagramSocket proxySocket = null;
        private static ILog logger = log4net.LogManager.GetLogger(typeof(RadiusProxy));

    }
}