using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using log4net;
using TinyRadius.Net.Attributes;
using TinyRadius.Net.Packet;
using TinyRadius.Net.Util;

namespace TinyRadius.Net.Proxy
{
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

        private static readonly ILog logger = LogManager.GetLogger(typeof (RadiusProxy));
        private readonly Hashtable proxyConnections = new Hashtable();
        private int proxyIndex = 1;
        private int proxyPort = 1814;
        private UdpClient proxySocket;

        public int ProxyPort
        {
            get { return proxyPort; }
            set
            {
                proxyPort = value;
                proxySocket = null;
            }
        }

        /**
         * Sets the Proxy port this server listens to.
         * Please call before start().
         * @param proxyPort Proxy port
         */

        /**
         * Sets the socket timeout.
         * @param socketTimeout socket timeout, >0 ms
         * @throws SocketException
         */

        public override int SocketTimeout
        {
            get { return base.SocketTimeout; }
            set
            {
                base.SocketTimeout = value;
                /*if (proxySocket != null)
                    proxySocket.ReceiveTimeout = SocketTimeout;*/
            }
        }

        public void Start(bool listenAuth, bool listenAcct, bool listenProxy)
        {
            base.Start(listenAuth, listenAcct);
            if (listenProxy)
            {
                ThreadPool.QueueUserWorkItem(delegate
                                                 {
                                                     //setName("Radius Proxy Listener");
                                                     try
                                                     {
                                                         logger.Info("starting RadiusProxyListener on port " +
                                                                     ProxyPort);
                                                         Listen(getProxySocket());
                                                     }
                                                     catch (Exception e)
                                                     {
                                                         //e.printStackTrace();
                                                         logger.Fatal(e);
                                                         throw e;
                                                     }
                                                 });
            }
        }

        /**
         * Stops the Proxy and closes the socket.
         */

        public override void Stop()
        {
            logger.Info("stopping Radius Proxy");
            if (proxySocket != null)
                proxySocket.Close();

            ;
            base.Stop();
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
        public abstract RadiusEndpoint GetProxyServer(RadiusPacket packet, RadiusEndpoint client);

        /**
         * Returns the Proxy port this server listens to.
         * Defaults to 1814.
         * @return Proxy port
         */


        /**
         * Returns a socket bound to the Proxy port.
         * @return socket
         * @throws SocketException
         */

        protected UdpClient getProxySocket()
        {
            if (proxySocket == null)
            {
                IPEndPoint ep = ListenAddress == null
                                    ? new IPEndPoint(IPAddress.Any, ProxyPort)
                                    : new IPEndPoint(ListenAddress, ProxyPort);
                proxySocket = new UdpClient(ep);
            }
            return proxySocket;
        }

        /**
         * Handles packets coming in on the Proxy port. Decides whether
         * packets coming in on Auth/Acct ports should be proxied.
         */

        protected override RadiusPacket HandlePacket(IPEndPoint localAddress, IPEndPoint remoteAddress,
                                                     RadiusPacket request, String sharedSecret)
        {
            // handle incoming Proxy packet
            if (localAddress.Port == ProxyPort)
            {
                proxyPacketReceived(request, remoteAddress);
                return null;
            }

            // handle auth/acct packet
            var radiusClient = new RadiusEndpoint(remoteAddress, sharedSecret);
            RadiusEndpoint radiusServer = GetProxyServer(request, radiusClient);
            if (radiusServer != null)
            {
                // Proxy incoming packet to other radius server
                var proxyConnection = new RadiusProxyConnection(radiusServer, radiusClient, request,
                                                                localAddress.Port);
                logger.Info("Proxy packet to " + proxyConnection);
                proxyPacket(request, proxyConnection);
                return null;
            }
            else
                // normal processing
                return base.HandlePacket(localAddress, remoteAddress, request, sharedSecret);
        }

        /**
         * Sends an answer to a proxied packet back to the original host.
         * Retrieves the RadiusProxyConnection object from the cache employing
         * the Proxy-State attribute.
         * @param packet packet to be sent back
         * @param remote the server the packet arrived from
         * @throws IOException
         */

        protected void proxyPacketReceived(RadiusPacket packet, IPEndPoint remote)
        {
            // retrieve my Proxy-State attribute (the last)
            IList<RadiusAttribute> proxyStates = packet.GetAttributes(33);
            if (proxyStates == null || proxyStates.Count == 0)
                throw new RadiusException("Proxy packet without Proxy-State attribute");
            RadiusAttribute proxyState = proxyStates[proxyStates.Count - 1];

            // retrieve Proxy connection from cache 
            string state = BitConverter.ToString(proxyState.Data);
            var proxyConnection = (RadiusProxyConnection) proxyConnections[state];
            proxyConnections.Remove(state);
            if (proxyConnection == null)
            {
                logger.Warn("received packet on Proxy port without saved Proxy connection - duplicate?");
                return;
            }

            // retrieve client
            RadiusEndpoint client = proxyConnection.RadiusClient;
            if (logger.IsInfoEnabled)
            {
                logger.Info("received Proxy packet: " + packet);
                logger.Info("forward packet to " + client.EndpointAddress + " with secret " +
                            client.SharedSecret);
            }

            // remove only own Proxy-State (last attribute)
            packet.RemoveLastAttribute(33);

            // re-encode answer packet with authenticator of the original packet
            var answer = new RadiusPacket(packet.Type, packet.Identifier, packet.Attributes);
            byte[] datagram = MakeDatagramPacket(answer, client.SharedSecret, proxyConnection.Packet);

            // send back using correct socket
            UdpClient socket = proxyConnection.Port == AuthPort ? GetAuthSocket() : GetAcctSocket();
            socket.Send(datagram, datagram.Length, remote);
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
            lock (typeof (RadiusProxy))
            {
                // add Proxy-State attribute
                proxyIndex++;
                String proxyIndexStr = proxyIndex.ToString();
                packet.AddAttribute(new RadiusAttribute(33, Encoding.UTF8.GetBytes(proxyIndexStr)));

                // store RadiusProxyConnection object
                proxyConnections.Add(proxyIndexStr, proxyConnection);
            }

            // get server address
            //IPAddress serverAddress = proxyConnection.getRadiusServer().EndpointAddress.Address;
            //int serverPort = proxyConnection.getRadiusServer().EndpointAddress.Port;
            String serverSecret = proxyConnection.RadiusServer.SharedSecret;

            // save request authenticator (will be calculated new)
            byte[] auth = packet.Authenticator;

            // encode new packet (with new authenticator)
            var bos = new MemoryStream();
            packet.EncodeRequestPacket(bos, serverSecret);
            byte[] data = bos.ToArray();
            bos.Dispose();


            //var datagram = new DatagramPacket(data, data.Length, serverAddress, serverPort);

            // restore original authenticator
            packet.Authenticator = auth;

            // send packet
            //Socket proxySocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.IP);
            proxySocket.Send(data, data.Length);
            //proxySocket.send(datagram);
        }

        /**
         * Index for Proxy-State attribute.
         */
    }
}