using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using log4net;
using TinyRadius.Net.Attributes;
using TinyRadius.Net.Util;

namespace TinyRadius.Net.Packet
{
    /// <summary>
    ///This class represents an Access-Request Radius packet.
    /// </summary>
    public class AccessRequest : RadiusPacket
    {
        /// <summary>
        ///Radius type code for Radius attribute User-Name
        /// </summary>
        private const int USER_NAME = 1;

        /// <summary>
        ///Radius attribute type for User-Password attribute.
        /// </summary>
        private const int USER_PASSWORD = 2;

        /// <summary>
        ///Radius attribute type for CHAP-Password attribute.
        /// </summary>
        private const int CHAP_PASSWORD = 3;

        /// <summary>
        ///Radius attribute type for CHAP-Challenge attribute.
        /// </summary>
        private const int CHAP_CHALLENGE = 60;

        
        private static readonly Random random = new Random();

        /// <summary>
        ///Logger for logging information about malformed packets
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(typeof(AccessRequest));

        
        private byte[] chapChallenge;
        private byte[] chapPassword;
        private String password;

        /// <summary>
        ///Constructs an empty Access-Request packet.
        /// </summary>
        public AccessRequest()
        {
        }

        /// <summary>
        ///Constructs an Access-Request packet, sets the
        ///code, identifier and adds an User-Name and an
        ///User-Password attribute (PAP).
        ///@param userName user name
        ///@param userPassword user password
        /// </summary>
        public AccessRequest(String userName, String userPassword)
            : base(AccessRequest, GetNextPacketIdentifier())
        {
            UserName = userName;
            Password = userPassword;
        }

        /// <summary>
        ///Sets the User-Name attribute of this Access-Request.
        ///@param userName user name to set
        /// </summary>
        public string UserName
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value", "user name not set");
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
                return (ra).Value;
            }
        }

        /// <summary>
        ///Sets the plain-text user password.
        ///@param userPassword user password to set
        /// </summary>
        public string Password
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("password is empty");
                password = value;
            }
        }

        /// <summary>
        ///Returns the protocol used for encrypting the passphrase.
        ///@return AUTH_PAP or AUTH_CHAP
        /// </summary>
        public AuthenticationType AuthProtocol
        {
            get;
            set;
        }

        /// <summary>
        ///Retrieves the plain-text user password.
        ///Returns null for CHAP - use verifyPassword().
        ///@see #verifyPassword(String)
        ///@return user password
        /// </summary>
        public String getUserPassword()
        {
            return password;
        }

        /// <summary>
        ///Verifies that the passed plain-text password matches the password
        ///(hash) send with this Access-Request packet. Works with both PAP
        ///and CHAP.
        ///@param plaintext
        ///@return true if the password is valid, false otherwise
        /// </summary>
        public bool VerifyPassword(String plaintext)
        {
            if (string.IsNullOrEmpty(plaintext))
                throw new ArgumentException("password is empty");
            if (AuthProtocol==AuthenticationType.chap)
                return VerifyChapPassword(plaintext);
            else
                return getUserPassword().Equals(plaintext);
        }

        /// <summary>
        ///Decrypts the User-Password attribute.
        ///@see TinyRadius.packet.RadiusPacket#decodeRequestAttributes(java.lang.String)
        /// </summary>
        protected override void DecodeRequestAttributes(String sharedSecret)
        {
            // detect auth protocol 
            RadiusAttribute userPassword = GetAttribute(USER_PASSWORD);
            RadiusAttribute chapPassword = GetAttribute(CHAP_PASSWORD);
            RadiusAttribute chapChallenge = GetAttribute(CHAP_CHALLENGE);

            if (userPassword != null)
            {
                AuthProtocol = AuthenticationType.pap;
                password = DecodePapPassword(userPassword.Data, RadiusUtil.GetUtf8Bytes(sharedSecret));
                // copy truncated data
                userPassword.Data = RadiusUtil.GetUtf8Bytes(password);
            }
            else if (chapPassword != null && chapChallenge != null)
            {
                AuthProtocol = AuthenticationType.chap;
                this.chapPassword = chapPassword.Data;
                this.chapChallenge = chapChallenge.Data;
            }
            else
                throw new RadiusException("Access-Request: User-Password or CHAP-Password/CHAP-Challenge missing");
        }

        /// <summary>
        ///Sets and encrypts the User-Password attribute.
        ///@see TinyRadius.packet.RadiusPacket#encodeRequestAttributes(java.lang.String)
        /// </summary>
        protected override void EncodeRequestAttributes(String sharedSecret)
        {
            if (string.IsNullOrEmpty(password))
                return;
            // ok for proxied packets whose CHAP password is already encrypted
            //throw new NotImplementedException("no password set");

            if (AuthProtocol.Equals(AuthenticationType.pap))
            {
                byte[] pass = EncodePapPassword(RadiusUtil.GetUtf8Bytes(password), RadiusUtil.GetUtf8Bytes(sharedSecret));
                RemoveAttributes(USER_PASSWORD);
                AddAttribute(new RadiusAttribute(USER_PASSWORD, pass));
            }
            else if (AuthProtocol.Equals(AuthenticationType.chap))
            {
                byte[] challenge = CreateChapChallenge();
                byte[] pass = EncodeChapPassword(password, challenge);
                RemoveAttributes(CHAP_PASSWORD);
                RemoveAttributes(CHAP_CHALLENGE);
                AddAttribute(new RadiusAttribute(CHAP_PASSWORD, pass));
                AddAttribute(new RadiusAttribute(CHAP_CHALLENGE, challenge));
            }
        }


        /// <summary>
        ///This method encodes the plaintext user password according to RFC 2865.
        ///@param userPass the password to encrypt
        ///@param sharedSecret shared secret
        ///@return the byte array containing the encrypted password
        /// </summary>
        private byte[] EncodePapPassword(byte[] userPass, byte[] sharedSecret)
        {
            // the password must be a multiple of 16 bytes and less than or equal
            // to 128 bytes. If it isn't a multiple of 16 bytes fill it out with zeroes
            // to make it a multiple of 16 bytes. If it is greater than 128 bytes
            // truncate it at 128.
            byte[] userPassBytes = null;
            if (userPass.Length > 128)
            {
                userPassBytes = new byte[128];
                Array.Copy(userPass, 0, userPassBytes, 0, 128);
            }
            else
            {
                userPassBytes = userPass;
            }

            // declare the byte array to hold the readonly product
            byte[] encryptedPass = null;
            if (userPassBytes.Length < 128)
            {
                if (userPassBytes.Length % 16 == 0)
                {
                    // tt is already a multiple of 16 bytes
                    encryptedPass = new byte[userPassBytes.Length];
                }
                else
                {
                    // make it a multiple of 16 bytes
                    encryptedPass = new byte[((userPassBytes.Length / 16) * 16) + 16];
                }
            }
            else
            {
                // the encrypted password must be between 16 and 128 bytes
                encryptedPass = new byte[128];
            }

            // copy the userPass into the encrypted pass and then fill it out with zeroes
            Array.Copy(userPassBytes, 0, encryptedPass, 0, userPassBytes.Length);
            for (int i = userPassBytes.Length; i < encryptedPass.Length; i++)
            {
                encryptedPass[i] = 0;
            }

            // digest shared secret and authenticator

            MD5 md5 = MD5.Create();
            var lastBlock = new byte[16];
            var byteAry = new List<byte>();
            for (int i = 0; i < encryptedPass.Length; i += 16)
            {
                byteAry.Clear();
                byteAry.AddRange(sharedSecret);
                byteAry.AddRange(i == 0 ? Authenticator : lastBlock);
                byte[] bn = md5.ComputeHash(byteAry.ToArray());

                Array.Copy(encryptedPass, i, lastBlock, 0, 16);

                // perform the XOR as specified by RFC 2865.
                for (int j = 0; j < 16; j++)
                    encryptedPass[i + j] = (byte)(bn[j] ^ encryptedPass[i + j]);
            }

            return encryptedPass;
        }

        /// <summary>
        ///Decodes the passed encrypted password and returns the clear-text form.
        ///@param encryptedPass encrypted password
        ///@param sharedSecret shared secret
        ///@return decrypted password
        /// </summary>
        private String DecodePapPassword(byte[] encryptedPass, byte[] sharedSecret)
        {
            if (encryptedPass == null || encryptedPass.Length < 16)
            {
                // PAP passwords require at least 16 bytes
                logger.Warn("invalid Radius packet: User-Password attribute with malformed PAP password, Length = " +
                            (encryptedPass != null ? encryptedPass.Length : 0) + ", but Length must be greater than 15");
                throw new RadiusException("malformed User-Password attribute");
            }

            var byteAry = new List<byte>();
            var lastBlock = new byte[16];
            MD5 md5 = MD5.Create();
            for (int i = 0; i < encryptedPass.Length; i += 16)
            {
                byteAry.Clear();
                byteAry.AddRange(sharedSecret);
                byteAry.AddRange(i == 0 ? Authenticator : lastBlock);
                byte[] bn = md5.ComputeHash(byteAry.ToArray());

                Array.Copy(encryptedPass, i, lastBlock, 0, 16);

                // perform the XOR as specified by RFC 2865.
                for (int j = 0; j < 16; j++)
                    encryptedPass[i + j] = (byte)(bn[j] ^ encryptedPass[i + j]);
            }

            // remove trailing zeros
            int len = encryptedPass.Length;
            while (len > 0 && encryptedPass[len - 1] == 0)
                len--;
            var passtrunc = new byte[len];
            Array.Copy(encryptedPass, 0, passtrunc, 0, len);

            // convert to string
            return RadiusUtil.GetStringFromUtf8(passtrunc);
        }

        /// <summary>
        ///Creates a random CHAP challenge using a secure random algorithm.
        ///@return 16 byte CHAP challenge
        /// </summary>
        private byte[] CreateChapChallenge()
        {
            var challenge = new byte[16];
            random.NextBytes(challenge);
            return challenge;
        }

        /// <summary>
        ///Encodes a plain-text password using the given CHAP challenge.
        ///@param plaintext plain-text password
        ///@param chapChallenge CHAP challenge
        ///@return CHAP-encoded password
        /// </summary>
        private byte[] EncodeChapPassword(String plaintext, byte[] chapChallenge)
        {
            // see RFC 2865 section 2.2
            var chapIdentifier = (byte)random.Next(256);
            var chapPassword = new byte[17];
            chapPassword[0] = chapIdentifier;

            MD5 md5 = MD5.Create();
            var aryList = new List<byte>();
            aryList.Add(chapIdentifier);
            aryList.AddRange(RadiusUtil.GetUtf8Bytes(plaintext));
            byte[] chapHash = md5.ComputeHash(chapChallenge);

            Array.Copy(chapHash, 0, chapPassword, 1, 16);
            return chapPassword;
        }

        /// <summary>
        ///Verifies a CHAP password against the given plaintext password.
        ///@return plain-text password
        /// </summary>
        private bool VerifyChapPassword(String plaintext)
        {
            if (string.IsNullOrEmpty(plaintext))
                throw new ArgumentException("plaintext must not be empty");
            if (chapChallenge == null || chapChallenge.Length != 16)
                throw new RadiusException("CHAP challenge must be 16 bytes");
            if (chapPassword == null || chapPassword.Length != 17)
                throw new RadiusException("CHAP password must be 17 bytes");

            byte chapIdentifier = chapPassword[0];

            var byteAry = new List<byte> { chapIdentifier };
            byteAry.AddRange(RadiusUtil.GetUtf8Bytes(plaintext));
            byte[] chapHash = MD5.Create().ComputeHash(chapChallenge);

            // compar
            for (int i = 0; i < 16; i++)
                if (chapHash[i] != chapPassword[i + 1])
                    return false;
            return true;
        }
    }
}