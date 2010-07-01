namespace TinyRadius.Net.packet
{



    using TinyRadius.Net.Util;
    using System;
    using System.Security.Cryptography;
    using TinyRadius.Net.Packet;

    /**
     * CoA packet. Thanks to Michael Krastev.
     * @author Michael Krastev <mkrastev@gmail.com>
     */
    public class CoaRequest : RadiusPacket
    {

        public CoaRequest()
            : base(COA_REQUEST, GetNextPacketIdentifier())
        {
        }

        /**
         * @see AccountingRequest#updateRequestAuthenticator(String, int, byte[])
         */
        protected byte[] updateRequestAuthenticator(String sharedSecret,
                int packetLength, byte[] attributes)
        {
            byte[] authenticator = new byte[16];
            /*for (int i = 0; i < 16; i++)
                authenticator[i] = 0;*/

            MD5 md5 = GetMd5Digest();
            md5.Initialize();



            /*MessageDigest md5 = getMd5Digest();
            md5.reset();
            md5.update((byte)getPacketType());
            md5.update((byte)getPacketIdentifier());
            md5.update((byte)(packetLength >> 8));
            md5.update((byte)(packetLength & 0xff));
            md5.update(authenticator, 0, authenticator.Length);
            md5.update(attributes, 0, attributes.Length);
            md5.update(RadiusUtil.getUtf8Bytes(sharedSecret));
            return md5.digest();*/
        }

    }
}