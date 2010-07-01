using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using TinyRadius.Net.Attributes;
using TinyRadius.Net.Packet;
using TinyRadius.Net.Util;

namespace TinyRadius.Net.packet
{
    /// <summary>
    /// This class represents a Radius packet of the type
    /// "Accounting-Request".
    /// </summary>
    public class AccountingRequest : RadiusPacket
    {
        /// <summary>
        /// Acct-Status-Type: Start
        /// </summary>
        public static readonly int ACCT_STATUS_TYPE_START = 1;

        /// <summary>
        /// Acct-Status-Type: Stop
        /// </summary>
        public static readonly int ACCT_STATUS_TYPE_STOP = 2;

        /// <summary>
        /// Acct-Status-Type: Interim Update/Alive
        /// </summary>
        public static readonly int ACCT_STATUS_TYPE_INTERIM_UPDATE = 3;

        /// <summary>
        /// Acct-Status-Type: Accounting-On
        /// </summary>
        public static readonly int ACCT_STATUS_TYPE_ACCOUNTING_ON = 7;

        /// <summary>
        /// Acct-Status-Type: Accounting-Off
        /// </summary>
        public static readonly int ACCT_STATUS_TYPE_ACCOUNTING_OFF = 8;

        /// <summary>
        /// Radius User-Name attribute type
        /// </summary>
        private static readonly int USER_NAME = 1;

        /// <summary>
        /// Radius Acct-Status-Type attribute type
        /// </summary>
        private static readonly int ACCT_STATUS_TYPE = 40;

        /// <summary>
        /// Constructs an Accounting-Request packet to be sent to a Radius server.
        /// @param userName user name
        /// @param acctStatusType ACCT_STATUS_TYPE_///
        /// </summary>
        public AccountingRequest(String userName, int acctStatusType)
            : base(AccountingRequest, GetNextPacketIdentifier())
        {
            UserName = userName;
            AcctStatusType = acctStatusType;
        }

        /// <summary>
        /// Constructs an empty Accounting-Request to be received by a
        /// Radius client.
        /// </summary>
        public AccountingRequest()
        {
        }

        /// <summary>
        /// Sets the User-Name attribute of this Accountnig-Request.
        /// @param userName user name to set
        /// </summary>
        public string UserName
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException("user name not set");
                if (value.Length == 0)
                    throw new ArgumentException("empty user name not allowed");

                RemoveAttributes(USER_NAME);
                AddAttribute(new StringAttribute(USER_NAME, value));
            }
            get
            {
                IList<RadiusAttribute> attrs = GetAttributes(USER_NAME);
                if (attrs.Count < 1 || attrs.Count > 1)
                    throw new NotImplementedException("exactly one User-Name attribute required");

                RadiusAttribute ra = attrs[0];
                return ra.Value;
            }
        }

        /// <summary>
        /// Sets the Acct-Status-Type attribute of this Accountnig-Request.
        /// @param acctStatusType ACCT_STATUS_TYPE_/// to set
        /// </summary>
        public int AcctStatusType
        {
            set
            {
                if (value < 1 || value > 15)
                    throw new ArgumentException("bad Acct-Status-Type");
                RemoveAttributes(ACCT_STATUS_TYPE);
                AddAttribute(new IntegerAttribute(ACCT_STATUS_TYPE, value));
            }
            get
            {
                RadiusAttribute ra = GetAttribute(ACCT_STATUS_TYPE);
                if (ra == null)
                    return -1;
                else
                    return ((IntegerAttribute)ra).ValueInt;
            }
        }

        /// <summary>
        /// Calculates the request authenticator as specified by RFC 2866.
        /// @see TinyRadius.packet.RadiusPacket#updateRequestAuthenticator(java.lang.String, int, byte[])
        /// </summary>
        protected override byte[] UpdateRequestAuthenticator(String sharedSecret, int packetLength, byte[] attributes)
        {
            var authenticator = new byte[16];
            for (int i = 0; i < 16; i++)
                authenticator[i] = 0;

            var bytes = new List<byte>
                            {
                                (byte) Type,
                                (byte) Identifier,
                                (byte) (packetLength >> 8),
                                (byte) (packetLength & 0xff)
                            };

            bytes.AddRange(authenticator);
            bytes.AddRange(attributes);
            bytes.AddRange(RadiusUtil.GetUtf8Bytes(sharedSecret));
            return MD5.Create().ComputeHash(bytes.ToArray());
        }

        /// <summary>
        /// Checks the received request authenticator as specified by RFC 2866.
        /// </summary>
        protected override void CheckRequestAuthenticator(String sharedSecret, int packetLength, byte[] attributes)
        {
            byte[] expectedAuthenticator = UpdateRequestAuthenticator(sharedSecret, packetLength, attributes);
            byte[] receivedAuth = Authenticator;
            for (int i = 0; i < 16; i++)
                if (expectedAuthenticator[i] != receivedAuth[i])
                    throw new RadiusException("request authenticator invalid");
        }
    }
}