/**
 * $Id: TestClient.java,v 1.4 2006/02/17 18:14:54 wuttke Exp $
 * Created on 08.04.2005
 * @author Matthias Wuttke
 * @version $Revision: 1.4 $
 */
using TinyRadius.Net.packet;
using TinyRadius.Net.Packet;
using TinyRadius.Net.Util;

namespace TinyRadius.test
{


    using System;

    /**
     * Simple Radius command-line client.
     */
    public class TestClient
    {

        /**
         * Radius command line client.
         * <br/>Usage: TestClient <i>hostName sharedSecret userName password</i>
         * @param args arguments
         * @throws Exception
         */
        public static void main(String[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Usage: TestClient hostName sharedSecret userName password");
                return;
            }

            String host = args[0];
            String shared = args[1];
            String user = args[2];
            String pass = args[3];

            RadiusClient rc = new RadiusClient(host, shared);

            // 1. Send Access-Request
            AccessRequest ar = new AccessRequest(user, pass);
            ar.AuthProtocol = AccessRequest.AUTH_PAP; // or AUTH_CHAP
            ar.AddAttribute("NAS-Identifier", "this.is.my.nas-identifier.de");
            ar.AddAttribute("NAS-IP-Address", "192.168.0.100");
            ar.AddAttribute("Service-Type", "Login-User");
            ar.AddAttribute("WISPr-Redirection-URL", "http://www.sourceforge.net/");
            ar.AddAttribute("WISPr-Location-ID", "net.sourceforge.ap1");

            Console.WriteLine("Packet before it is sent\n" + ar + "\n");
            RadiusPacket response = rc.authenticate(ar);
            Console.WriteLine("Packet after it was sent\n" + ar + "\n");
            Console.WriteLine("Response\n" + response + "\n");

            // 2. Send Accounting-Request
            AccountingRequest acc = new AccountingRequest("mw", AccountingRequest.ACCT_STATUS_TYPE_START);
            acc.AddAttribute("Acct-Session-Id", "1234567890");
            acc.AddAttribute("NAS-Identifier", "this.is.my.nas-identifier.de");
            acc.AddAttribute("NAS-Port", "0");

            Console.WriteLine(acc + "\n");
            response = rc.Account(acc);
            Console.WriteLine("Response: " + response);

            rc.Close();
        }

    }
}