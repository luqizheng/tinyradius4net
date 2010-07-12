/**
 * $Id: TestServer.java,v 1.6 2006/02/17 18:14:54 wuttke Exp $
 * Created on 08.04.2005
 * @author Matthias Wuttke
 * @version $Revision: 1.6 $
 */
using System;
using System.Net;
using System.Threading;
using TinyRadius.Net.Packet;
using TinyRadius.Net.Util;

namespace TinyRadius.Test
{
    /**
     * Test server which terminates after 30 s.
     * Knows only the client "localhost" with secret "testing123" and
     * the user "mw" with the password "test".
     */

    public class TestServer
    {
        public static void main(String[] args)
        {
            var server = new FaileServer();
            /* {
                 // Authorize localhost/testing123
                 public String getSharedSecret(InetSocketAddress client) {
                     if (client.getAddress().getHostAddress().equals("127.0.0.1"))
                         return "testing123";
                     else
                         return null;
                 }
			
                 // Authenticate mw
                 public String getUserPassword(String userName) {
                     if (userName.equals("mw"))
                         return "test";
                     else
                         return null;
                 }
			
                 // Adds an attribute to the Access-Accept packet
                 public RadiusPacket accessRequestReceived(AccessRequest accessRequest, InetSocketAddress client) 
                 {
                
                     Console.WriteLine("Received Access-Request:\n" + accessRequest);
                     RadiusPacket packet = super.accessRequestReceived(accessRequest, client);
                     if (packet.getPacketType() == RadiusPacket.ACCESS_ACCEPT)
                         packet.addAttribute("Reply-Message", "Welcome " + accessRequest.getUserName() + "!");
                     if (packet == null)
                         Console.WriteLine("Ignore packet.");
                     else
                         Console.WriteLine("Answer:\n" + packet);
                     return packet;
                 }
             };*/
            if (args.Length >= 1)
                server.AuthPort = Convert.ToInt32(args[0]);
            if (args.Length >= 2)
                server.AuthPort = Convert.ToInt32(args[1]);

            server.Start(true, true);

            System.Console.WriteLine("Server started.");

            Thread.Sleep(1000*60*30);
            System.Console.WriteLine("Stop server");
            server.Stop();
        }

        #region Nested type: FaileServer

        public class FaileServer : RadiusServer
        {
            public override string GetSharedSecret(IPEndPoint client)
            {
                if (client.Address.Equals(IPAddress.Parse("127.0.0.1")))
                    return "testing123";
                else
                    return null;
            }

            public override string GetUserPassword(string userName)
            {
                if (userName.Equals("mw"))
                    return "test";
                else
                    return null;
            }

            public override RadiusPacket AccessRequestReceived(AccessRequest accessRequest, IPEndPoint client)
            {
                System.Console.WriteLine("Received Access-Request:\n" + accessRequest);
                RadiusPacket packet = base.AccessRequestReceived(accessRequest, client);
                if (packet.Type == RadiusPacket.AccessAccept)
                    packet.AddAttribute("Reply-Message", "Welcome " + accessRequest.UserName + "!");
                if (packet == null)
                    System.Console.WriteLine("Ignore packet.");
                else
                    System.Console.WriteLine("Answer:\n" + packet);
                return packet;
            }
        }

        #endregion
    }
}