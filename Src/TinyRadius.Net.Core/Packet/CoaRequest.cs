using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using TinyRadius.Net.Packet;
using TinyRadius.Net.Util;

namespace TinyRadius.Net.packet
{
    /**
     * CoA packet. Thanks to Michael Krastev.
     * @author Michael Krastev <mkrastev@gmail.com>
     */

    public class CoaRequest : RadiusPacket
    {
        public CoaRequest()
            : base(CoaRequest, GetNextPacketIdentifier())
        {
        }

        /**
         * @see AccountingRequest#updateRequestAuthenticator(String, int, byte[])
         */

        protected override byte[] UpdateRequestAuthenticator(String sharedSecret,
                                                             int packetLength, byte[] attributes)
        {
            var authenticator = new byte[16];
            /*for (int i = 0; i < 16; i++)
                authenticator[i] = 0;*/


            var ms = new List<byte>
                         {
                             Convert.ToByte(Type),
                             Convert.ToByte(Identifier),
                             Convert.ToByte(packetLength >> 8),
                             Convert.ToByte(packetLength & 0xff)
                         };

            ms.AddRange(authenticator);
            ms.AddRange(attributes);
            ms.AddRange(RadiusUtil.GetUtf8Bytes(sharedSecret));

            return MD5.Create().ComputeHash(ms.ToArray());

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