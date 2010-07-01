using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TinyRadius.Net.Attributes;
using TinyRadius.Net.Dictionaries;
using TinyRadius.Net.Net.JavaHelper;
using TinyRadius.Net.packet;
using TinyRadius.Net.Util;

namespace TinyRadius.Net.Packet
{
    /// <summary>
    ///  This class represents a Radius packet. Subclasses provide convenience methods
    ///  for special packet types.
    /// </summary>
    public class RadiusPacket
    {
        /// <summary>
        /// Maximum packet Length.
        ///</summary>
        public static readonly int MaxPacketLength = 4096;

        /// <summary>
        ///  Packet header Length.
        /// </summary>
        public static readonly int RadiusHeaderLength = 20;

        /// <summary>
        ///  Next packet identifier.
        /// </summary>
        private static int nextPacketId;

        /// <summary>
        ///  Random number generator.
        /// </summary>
        private static readonly Random random = new Random();

        /// <summary>
        ///  Attributes for this packet.
        /// </summary>
        private IList<RadiusAttribute> attributes = new List<RadiusAttribute>();

        /// <summary>
        ///  Authenticator for this Radius packet.
        /// </summary>
        private byte[] authenticator;

        /// <summary>
        ///  Dictionary to look up attribute names.
        /// </summary>
        private IWritableDictionary dictionary = DefaultDictionary.GetDefaultDictionary();

        /// <summary>
        ///  MD5 digest.
        /// </summary>
        private MD5 md5Digest;

        /// <summary>
        ///  Identifier of this packet.
        /// </summary>
        private int _identifier;

        /// <summary>
        ///  Type of this Radius packet.
        /// </summary>
        private int _type;

        /// <summary>
        ///  Builds a Radius packet without attributes. Retrieves
        ///  the next packet identifier.
        ///  @param type packet type 
        /// </summary>
        public RadiusPacket(int type)
            : this(type, GetNextPacketIdentifier(), new List<RadiusAttribute>())
        {
        }

        /// <summary>
        ///  Builds a Radius packet with the given type and identifier
        ///  and without attributes.
        ///  @param type packet type
        ///  @param identifier packet identifier
        /// </summary>
        public RadiusPacket(int type, int identifier) :
            this(type, identifier, new List<RadiusAttribute>())
        {
        }

        /// <summary>
        ///  Builds a Radius packet with the given type, identifier and
        ///  attributes. 
        ///  @param type packet type
        ///  @param identifier packet identifier
        ///  @param attributes list of RadiusAttribute objects
        /// </summary>
        public RadiusPacket(int type, int identifier, IList<RadiusAttribute> attributes)
        {
            Type = type;
            Identifier = identifier;
            Attributes = attributes;
        }

        /// <summary>
        ///  Builds an empty Radius packet.
        /// </summary>
        public RadiusPacket()
        {
        }

        /// <summary>
        ///  Returns the packet identifier for this Radius packet.
        ///  @return packet identifier
        /// </summary>
        public int Identifier
        {
            get { return _identifier; }
            set
            {
                if (value < 0 || value > 255)
                    throw new ArgumentException("packet identifier out of bounds");
                _identifier = value;
            }
        }

        /// <summary>
        ///  Returns the type of this Radius packet.
        ///  @return packet type
        /// </summary>
        public int Type
        {
            get { return _type; }
            set
            {
                if (value < 1 || value > 255)
                    throw new ArgumentException("packet type out of bounds");
                _type = value;
            }
        }

        private const int AccessRequest = 1;
        private const int AccountingRequest = 4;
        private const int AccessAccept = 2;
        private const int AccessReject = 3;
        private const int AccountingResponse = 5;
        /// <summary>
        ///  Returns the type name of this Radius packet.
        ///  @return name
        /// </summary>
        public string TypeName
        {
            get
            {
                switch (Type)
                {
                    case AccessRequest:
                        return "Access-Request";
                    case AccessAccept:
                        return "Access-Accept";
                    case AccessReject:
                        return "Access-Reject";
                    case AccountingRequest:
                        return "Accounting-Request";
                    case 5:
                        return "Accounting-Response";
                    case 6:
                        return "Accounting-Status";
                    case 7:
                        return "Password-Request";
                    case 8:
                        return "Password-Accept";
                    case 9:
                        return "Password-Reject";
                    case 10:
                        return "Accounting-Message";
                    case 11:
                        return "Access-Challenge";
                    case 12:
                        return "Status-Server";
                    case 13:
                        return "Status-Client";
                    // RFC 2882
                    case 40:
                        return "Disconnect-Request";
                    case 41:
                        return "Disconnect-ACK";
                    case 42:
                        return "Disconnect-NAK";
                    case 43:
                        return "CoA-Request";
                    case 44:
                        return "CoA-ACK";
                    case 45:
                        return "CoA-NAK";
                    case 46:
                        return "Status-Request";
                    case 47:
                        return "Status-Accept";
                    case 48:
                        return "Status-Reject";
                    case 255:
                        return "Reserved";
                    default:
                        return "Unknown (" + Type + ")";
                }
            }
        }

        /// <summary>
        ///  Sets the list of attributes for this Radius packet.
        ///  @param attributes list of RadiusAttribute objects
        /// </summary>
        public IList<RadiusAttribute> Attributes
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value", "attributes list is null");

                this.attributes = value;
            }
        }

        /// <summary>
        ///  Adds a Radius attribute to this packet. Can also be used
        ///  to add Vendor-Specific sub-attributes. If a attribute with
        ///  a vendor code != -1 is passed in, a VendorSpecificAttribute
        ///  is created for the sub-attribute.
        ///  @param attribute RadiusAttribute object
        /// </summary>
        public void AddAttribute(RadiusAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException("attribute", "attribute is null");
            attribute.Dictionary = Dictionary;
            if (attribute.VendorId == -1)
                attributes.Add(attribute);
            else
            {
                var vsa = new VendorSpecificAttribute(attribute.VendorId);
                vsa.AddSubAttribute(attribute);
                attributes.Add(vsa);
            }
        }

        /// <summary>
        ///  Adds a Radius attribute to this packet.
        ///  Uses AttributeTypes to lookup the type code and converts
        ///  the value.
        ///  Can also be used to add sub-attributes.
        ///  @param typeName name of the attribute, for example "NAS-Ip-Address"
        ///  @param value value of the attribute, for example "127.0.0.1"
        ///  @throws ArgumentException if type name is unknown
        /// </summary>
        public void AddAttribute(String typeName, String value)
        {
            if (string.IsNullOrEmpty(typeName))
                throw new ArgumentException("type name is empty");
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("value is empty");

            AttributeType type = this.Dictionary.GetAttributeTypeByName(typeName);
            if (type == null)
                throw new ArgumentException("unknown attribute type '" + typeName + "'");

            RadiusAttribute attribute = RadiusAttribute.CreateRadiusAttribute(Dictionary, type.VendorId,
                                                                              type.TypeCode);
            attribute.Value = value;
            AddAttribute(attribute);
        }

        /// <summary>
        ///  Removes the specified attribute from this packet.
        ///  @param attribute RadiusAttribute to remove
        /// </summary>
        public void RemoveAttribute(RadiusAttribute attribute)
        {
            if (attribute.VendorId == -1)
            {
                if (!attributes.Remove(attribute))
                    throw new ArgumentException("no such attribute");
            }
            else
            {
                // remove Vendor-Specific sub-attribute
                var vsas = GetVendorAttributes(attribute.VendorId);

                foreach (VendorSpecificAttribute vsa in vsas)
                {
                    List<RadiusAttribute> sas = vsa.SubAttributes;
                    if (sas.Contains(attribute))
                    {
                        vsa.RemoveSubAttribute(attribute);
                        if (sas.Count == 1)
                            // removed the last sub-attribute
                            // --> remove the whole Vendor-Specific attribute
                            RemoveAttribute(vsa);
                    }
                }
            }
        }

        /// <summary>
        ///  Removes all attributes from this packet which have got
        ///  the specified type.
        ///  @param type attribute type to remove
        /// </summary>
        public void RemoveAttributes(int type)
        {
            if (type < 1 || type > 255)
                throw new ArgumentException("attribute type out of bounds");

            var removedInt = new List<int>();
            for (int i = attributes.Count; i > 0; i--)
            {
                if (attributes[i].Type == type)
                    removedInt.Add(i);
            }
            foreach (var index in removedInt)
            {
                this.attributes.RemoveAt(index);
            }
        }

        /// <summary>
        ///  Removes the last occurence of the attribute of the given
        ///  type from the packet.
        ///  @param type attribute type code
        /// </summary>
        public void RemoveLastAttribute(int type)
        {
            var attrs = GetAttributes(type);
            if (attrs == null || attrs.Count == 0)
                return;

            var lastAttribute =
                attrs[attrs.Count - 1];
            RemoveAttribute(lastAttribute);
        }

        /// <summary>
        ///  Removes all sub-attributes of the given vendor and
        ///  type.
        ///  @param vendorId vendor ID
        ///  @param typeCode attribute type code
        /// </summary>
        public void RemoveAttributes(int vendorId, int typeCode)
        {
            if (vendorId == -1)
            {
                RemoveAttributes(typeCode);
                return;
            }
            List<Int32> removeVendorList = new List<int>();
            var vendorList = GetVendorAttributes(vendorId);
            var lengthOfVendor = GetVendorAttributes(vendorId).Count;
            for (int i = 0; i < lengthOfVendor; i++)
            {
                VendorSpecificAttribute vsa = vendorList[i] as VendorSpecificAttribute;
                if (vsa == null)
                    continue;
                var sas = vsa.SubAttributes;
                var removeId = new List<int>();

                for (int j = 0; j < sas.Count; j++)
                {
                    var attr = sas[j];
                    if (attr.Type == typeCode && attr.VendorId == vendorId)
                    {
                        removeId.Add(j);
                    }
                }
            }

        }

        /// <summary>
        ///  Returns all attributes of this packet of the given type.
        ///  Returns an empty list if there are no such attributes.
        ///  @param attributeType type of attributes to get 
        ///  @return list of RadiusAttribute objects, does not return null
        /// </summary>
        public IList<RadiusAttribute> GetAttributes(int attributeType)
        {
            if (attributeType < 1 || attributeType > 255)
                throw new ArgumentException("attribute type out of bounds");

            IList<RadiusAttribute> result = new List<RadiusAttribute>();
            foreach (RadiusAttribute a in attributes)
            {
                if (attributeType == a.Type)
                    result.Add(a);
            }
            return result;
        }

        /// <summary>
        ///  Returns all attributes of this packet that have got the
        ///  given type and belong to the given vendor ID.
        ///  Returns an empty list if there are no such attributes.
        ///  @param vendorId vendor ID
        ///  @param attributeType attribute type code
        ///  @return list of RadiusAttribute objects, never null
        /// </summary>
        public IList<RadiusAttribute> GetAttributes(int vendorId, int attributeType)
        {
            if (vendorId == -1)
                return GetAttributes(attributeType);

            IList<RadiusAttribute> vsas = GetVendorAttributes(vendorId);
            var result = from radius in vsas
                         where
                         radius.Type == attributeType && radius.VendorId == vendorId
                         select radius;
            /*for (Iterator i = vsas.iterator(); i.hasNext(); )
            {
                var vsa = (VendorSpecificAttribute)i.next();
                List sas = vsa.SubAttributes;
                for (Iterator j = sas.iterator(); j.hasNext(); )
                {
                    var attr = (RadiusAttribute)j.next();
                    if (attr.Type == attributeType &&
                        attr.VendorId == vendorId)
                        result.add(attr);
                }
            }*/

            return result.ToList<RadiusAttribute>();
        }

        /// <summary>
        ///  Returns a list of all attributes belonging to this Radius
        ///  packet.
        ///  @return List of RadiusAttribute objects
        /// </summary>
        public IList<RadiusAttribute> GetAttributes()
        {
            return attributes;
        }

        /// <summary>
        ///  Returns a Radius attribute of the given type which may only occur once
        ///  in the Radius packet.
        ///  @param type attribute type
        ///  @return RadiusAttribute object or null if there is no such attribute
        ///  @throws NotImplementedException if there are multiple occurences of the
        ///  requested attribute type
        /// </summary>
        public RadiusAttribute GetAttribute(int type)
        {
            var attrs = GetAttributes(type);
            if (attrs.Count > 1)
                throw new NotImplementedException("multiple attributes of requested type " + type);
            else if (attrs.Count == 0)
                return null;
            else
                return (RadiusAttribute)attrs[0];
        }

        /// <summary>
        ///  Returns a Radius attribute of the given type and vendor ID
        ///  which may only occur once in the Radius packet.
        ///  @param vendorId vendor ID
        ///  @param type attribute type
        ///  @return RadiusAttribute object or null if there is no such attribute
        ///  @throws NotImplementedException if there are multiple occurences of the
        ///  requested attribute type
        /// </summary>
        public RadiusAttribute GetAttribute(int vendorId, int type)
        {
            if (vendorId == -1)
                return GetAttribute(type);

            var attrs = GetAttributes(vendorId, type);
            if (attrs.Count > 1)
                throw new NotImplementedException("multiple attributes of requested type " + type);
            else if (attrs.Count == 0)
                return null;
            else
                return (RadiusAttribute)attrs[0];
        }

        /// <summary>
        ///  Returns a single Radius attribute of the given type name.
        ///  Also returns sub-attributes.
        ///  @param type attribute type name
        ///  @return RadiusAttribute object or null if there is no such attribute
        ///  @throws NotImplementedException if the attribute occurs multiple times
        /// </summary>
        public RadiusAttribute GetAttribute(String type)
        {
            if (string.IsNullOrEmpty(type))
                throw new ArgumentException("type name is empty");

            AttributeType t = dictionary.GetAttributeTypeByName(type);
            if (t == null)
                throw new ArgumentException("unknown attribute type name '" + type + "'");

            return GetAttribute(t.VendorId, t.TypeCode);
        }

        /// <summary>
        ///  Returns the value of the Radius attribute of the given type or
        ///  null if there is no such attribute.
        ///  Also returns sub-attributes.
        ///  @param type attribute type name
        ///  @return value of the attribute as a string or null if there
        ///  is no such attribute
        ///  @throws ArgumentException if the type name is unknown
        ///  @throws NotImplementedException attribute occurs multiple times
        /// </summary>
        public String GetAttributeValue(String type)
        {
            RadiusAttribute attr = GetAttribute(type);
            if (attr == null)
                return null;
            else
                return attr.Value;
        }

        /// <summary>
        ///  Returns the Vendor-Specific attribute(s) for the given vendor ID.
        ///  @param vendorId vendor ID of the attribute(s)
        ///  @return List with VendorSpecificAttribute objects, never null
        /// </summary>
        public IList<RadiusAttribute> GetVendorAttributes(int vendorId)
        {
            var result = new List<RadiusAttribute>();
            foreach (RadiusAttribute a in attributes)
            {
                if (typeof(VendorSpecificAttribute).IsInstanceOfType(a))
                {
                    var vsa = (VendorSpecificAttribute)a;
                    if (vsa.ChildVendorId == vendorId)
                        result.Add(vsa);
                }
            }
            return result;
        }

        /// <summary>
        ///  Returns the Vendor-Specific attribute for the given vendor ID.
        ///  If there is more than one Vendor-Specific
        ///  attribute with the given vendor ID, the first attribute found is
        ///  returned. If there is no such attribute, null is returned.
        ///  @param vendorId vendor ID of the attribute
        ///  @return the attribute or null if there is no such attribute
        ///  @deprecated use getVendorAttributes(int)
        ///  @see #getVendorAttributes(int)
        /// </summary>
        public VendorSpecificAttribute GetVendorAttribute(int vendorId)
        {
            foreach (VendorSpecificAttribute vsa in GetAttributes(VendorSpecificAttribute.VENDOR_SPECIFIC))
            {
                if (vsa.ChildVendorId == vendorId)
                    return vsa;
            }
            return null;
        }

        /// <summary>
        ///  Encodes this Radius packet and sends it to the specified output
        ///  stream.
        ///  @param out output stream to use
        ///  @param sharedSecret shared secret to be used to encode this packet
        ///  @exception IOException communication error
        /// </summary>
        public void EncodeRequestPacket(Stream outputStream, String sharedSecret)
        {
            encodePacket(outputStream, sharedSecret, null);
        }

        /// <summary>
        ///  Encodes this Radius response packet and sends it to the specified output
        ///  stream.
        ///  @param out output stream to use
        ///  @param sharedSecret shared secret to be used to encode this packet
        ///  @param request Radius request packet
        ///  @exception IOException communication error
        /// </summary>
        public void EncodeResponsePacket(Stream @out, String sharedSecret, RadiusPacket request)
        {
            if (request == null)
                throw new ArgumentNullException("request", "request cannot be null");
            encodePacket(@out, sharedSecret, request);
        }

        /// <summary>
        ///  Reads a Radius request packet from the given input stream and
        ///  creates an appropiate RadiusPacket descendant object.
        ///  Reads in all attributes and returns the object. 
        ///  Decodes the encrypted fields and attributes of the packet.
        ///  @param sharedSecret shared secret to be used to decode this packet
        ///  @return new RadiusPacket object
        ///  @exception IOException IO error
        ///  @exception RadiusException malformed packet
        /// </summary>
        public static RadiusPacket DecodeRequestPacket(Stream @in, String sharedSecret)
        {
            return DecodePacket(DefaultDictionary.GetDefaultDictionary(), @in, sharedSecret, null);
        }

        /// <summary>
        ///  Reads a Radius response packet from the given input stream and
        ///  creates an appropiate RadiusPacket descendant object.
        ///  Reads in all attributes and returns the object.
        ///  Checks the packet authenticator. 
        ///  @param sharedSecret shared secret to be used to decode this packet
        ///  @param request Radius request packet
        ///  @return new RadiusPacket object
        ///  @exception IOException IO error
        ///  @exception RadiusException malformed packet
        /// </summary>
        public static RadiusPacket decodeResponsePacket(Stream @in, String sharedSecret, RadiusPacket request)
        {
            if (request == null)
                throw new ArgumentNullException("request may not be null");
            return DecodePacket(DefaultDictionary.GetDefaultDictionary(), @in, sharedSecret, request);
        }

        /// <summary>
        ///  Reads a Radius request packet from the given input stream and
        ///  creates an appropiate RadiusPacket descendant object.
        ///  Reads in all attributes and returns the object. 
        ///  Decodes the encrypted fields and attributes of the packet.
        ///  @param dictionary dictionary to use for attributes
        ///  @param in InputStream to read packet from
        ///  @param sharedSecret shared secret to be used to decode this packet
        ///  @return new RadiusPacket object
        ///  @exception IOException IO error
        ///  @exception RadiusException malformed packet
        /// </summary>
        public static RadiusPacket DecodeRequestPacket(IWritableDictionary dictionary, Stream @in, String sharedSecret)
        {
            return DecodePacket(dictionary, @in, sharedSecret, null);
        }

        /// <summary>
        ///  Reads a Radius response packet from the given input stream and
        ///  creates an appropiate RadiusPacket descendant object.
        ///  Reads in all attributes and returns the object.
        ///  Checks the packet authenticator. 
        ///  @param dictionary dictionary to use for attributes
        ///  @param in InputStream to read packet from
        ///  @param sharedSecret shared secret to be used to decode this packet
        ///  @param request Radius request packet
        ///  @return new RadiusPacket object
        ///  @exception IOException IO error
        ///  @exception RadiusException malformed packet
        /// </summary>
        public static RadiusPacket decodeResponsePacket(IWritableDictionary dictionary, Stream @in,
                                                        String sharedSecret, RadiusPacket request)
        {
            if (request == null)
                throw new ArgumentNullException("request", "request may not be null");
            return DecodePacket(dictionary, @in, sharedSecret, request);
        }

        /// <summary>
        ///  Retrieves the next packet identifier to use and increments the static
        ///  storage.
        ///  @return the next packet identifier to use
        /// </summary>
        public static int GetNextPacketIdentifier()
        {


            nextPacketId++;
            if (nextPacketId > 255)
                nextPacketId = 0;
            return nextPacketId;

        }

        /// <summary>
        ///  Creates a RadiusPacket object. Depending on the passed type, the
        ///  appropiate successor is chosen. Sets the type, but does not touch
        ///  the packet identifier.
        ///  @param type packet type
        ///  @return RadiusPacket object
        /// </summary>
        public static RadiusPacket createRadiusPacket(int type)
        {
            RadiusPacket rp;
            switch (type)
            {
                case AccessRequest:
                    rp = new AccessRequest();
                    break;

                case AccountingRequest:
                    rp = new AccountingRequest();
                    break;
                case AccessAccept:
                case AccessReject:
                case AccountingResponse:
                default:
                    rp = new RadiusPacket();
                    break;
            }
            rp.Type = type;
            return rp;
        }

        /// <summary>
        ///  String representation of this packet, for debugging purposes.
        ///  @see java.lang.Object#toString()
        /// </summary>
        public override String ToString()
        {
            var s = new StringBuilder();
            s.Append(TypeName);
            s.Append(", ID ");
            s.Append(_identifier);
            //for (Iterator i = attributes.iterator(); i.hasNext(); )
            foreach (var attr in attributes)
            {
                //var attr = (RadiusAttribute)i.next();
                s.Append("\n").Append(attr.ToString());
            }
            return s.ToString();
        }

        /// <summary>
        ///  Returns the authenticator for this Radius packet.
        ///  For a Radius packet read from a stream, this will return the
        ///  authenticator sent by the server. For a new Radius packet to be sent,
        ///  this will return the authenticator created by the method
        ///  createAuthenticator() and will return null if no authenticator
        ///  has been created yet.
        ///  @return authenticator, 16 bytes
        /// </summary>
        public byte[] getAuthenticator()
        {
            return authenticator;
        }

        /// <summary>
        ///  Sets the authenticator to be used for this Radius packet.
        ///  This method should seldomly be used.
        ///  Authenticators are created and managed by this class internally.
        ///  @param authenticator authenticator
        /// </summary>
        public void setAuthenticator(byte[] authenticator)
        {
            this.authenticator = authenticator;
        }

        /// <summary>
        ///  Returns the dictionary this Radius packet uses.
        ///  @return Dictionary instance
        /// </summary>
        public IWritableDictionary Dictionary
        {
            get { return dictionary; }
            set
            {
                this.dictionary = value;
                foreach (var direct in attributes)
                {
                    direct.Dictionary = value;
                }
            }
        }

        /// <summary>
        ///  Encodes this Radius packet and sends it to the specified output
        ///  stream.
        ///  @param out output stream to use
        ///  @param sharedSecret shared secret to be used to encode this packet
        ///  @param request Radius request packet if this packet to be encoded
        ///  is a response packet, null if this packet is a request packet
        ///  @exception IOException communication error
        ///  @exception NotImplementedException if required packet data has not been set 
        /// </summary>
        protected void encodePacket(Stream outputStream, String sharedSecret, RadiusPacket request)
        {
            // check shared secret
            if (sharedSecret == null || sharedSecret.Length == 0)
                throw new NotImplementedException("no shared secret has been set");

            // check request authenticator
            if (request != null && request.getAuthenticator() == null)
                throw new NotImplementedException("request authenticator not set");

            // request packet authenticator
            if (request == null)
            {
                // first create authenticator, then encode attributes
                // (User-Password attribute needs the authenticator)
                authenticator = CreateRequestAuthenticator(sharedSecret);
                encodeRequestAttributes(sharedSecret);
            }

            byte[] attributes = GetAttributeBytes();
            int packetLength = RadiusHeaderLength + attributes.Length;
            if (packetLength > MaxPacketLength)
                throw new NotImplementedException("packet too long");

            // response packet authenticator
            if (request != null)
            {
                // after encoding attributes, create authenticator
                authenticator = createResponseAuthenticator(sharedSecret, packetLength, attributes,
                                                            request.getAuthenticator());
            }
            else
            {
                // update authenticator after encoding attributes
                authenticator = UpdateRequestAuthenticator(sharedSecret, packetLength, attributes);
            }
            outputStream.WriteByte(Convert.ToByte(Type));
            outputStream.WriteByte(Convert.ToByte(Identifier));
            outputStream.WriteByte(Convert.ToByte(packetLength));
            var authen = getAuthenticator();
            outputStream.Write(authen, 0, authen.Length);
            outputStream.Write(attributes, 0, attributes.Length);
            /*var dos = new MemoryStream(outputStream);
            dos.writeByte(Type);
            dos.writeByte(Identifier);
            dos.writeShort(packetLength);
            dos.write(getAuthenticator());
            dos.write(attributes);
            dos.flush();*/
        }

        /// <summary>
        ///  This method exists for subclasses to be overridden in order to
        ///  encode packet attributes like the User-Password attribute.
        ///  The method may use getAuthenticator() to get the request
        ///  authenticator.
        ///  @param sharedSecret
        /// </summary>
        protected void encodeRequestAttributes(String sharedSecret)
        {
        }

        /// <summary>
        ///  Creates a request authenticator for this packet. This request authenticator
        ///  is constructed as described in RFC 2865.
        ///  @param sharedSecret shared secret that secures the communication
        ///  with the other Radius server/client
        ///  @return request authenticator, 16 bytes
        /// </summary>
        protected byte[] CreateRequestAuthenticator(String sharedSecret)
        {
            byte[] secretBytes = RadiusUtil.GetUtf8Bytes(sharedSecret);

            var randomBytes = new byte[16];
            random.NextBytes(randomBytes);

            byte[] md5Bytes = new byte[secretBytes.Length + 16];

            /*code from java
            var md5 = GetMd5Digest();
            md5.reset();
            md5.update(secretBytes);
            md5.update(randomBytes);*/
            Array.Copy(secretBytes, 0, md5Bytes, 0, secretBytes.Length);
            Array.Copy(randomBytes, 0, md5Bytes, secretBytes.Length, 16);
            return MD5.Create().ComputeHash(md5Bytes);
        }

        /// <summary>
        ///  AccountingRequest overrides this
        ///  method to create a request authenticator as specified by RFC 2866.		 
        ///  @param sharedSecret shared secret
        ///  @param packetLength Length of the final Radius packet
        ///  @param attributes attribute data
        ///  @return new request authenticator
        /// </summary>
        protected byte[] UpdateRequestAuthenticator(String sharedSecret, int packetLength, byte[] attributes)
        {
            return authenticator;
        }

        /// <summary>
        ///  Creates an authenticator for a Radius response packet.
        ///  @param sharedSecret shared secret
        ///  @param packetLength Length of response packet
        ///  @param attributes encoded attributes of response packet
        ///  @param requestAuthenticator request packet authenticator
        ///  @return new 16 byte response authenticator
        /// </summary>
        protected byte[] createResponseAuthenticator(String sharedSecret, int packetLength, byte[] attributes,
                                                     byte[] requestAuthenticator)
        {
            //MessageDigest md5 = GetMd5Digest();
            //md5.reset();
            //md5.update((byte)Type);
            //md5.update((byte)Identifier);
            //md5.update((byte)(packetLength >> 8));
            //md5.update((byte)(packetLength & 0x0ff));
            //md5.update(requestAuthenticator, 0, requestAuthenticator.Length);
            //md5.update(attributes, 0, attributes.Length);
            //md5.update(RadiusUtil.getUtf8Bytes(sharedSecret));
            //return md5.digest();

            var bytes = new List<byte>
                          {
                              Convert.ToByte(Type),
                              Convert.ToByte(Identifier),
                              Convert.ToByte(packetLength >> 8),
                              Convert.ToByte(packetLength & 0x0ff)
                          };
            bytes.AddRange(requestAuthenticator);
            bytes.AddRange(RadiusUtil.GetUtf8Bytes(sharedSecret));

            return MD5.Create().ComputeHash(bytes.ToArray());
        }

        /// <summary>
        ///  Reads a Radius packet from the given input stream and
        ///  creates an appropiate RadiusPacket descendant object.
        ///  Reads in all attributes and returns the object. 
        ///  Decodes the encrypted fields and attributes of the packet.
        ///  @param dictionary dictionary to use for attributes
        ///  @param sharedSecret shared secret to be used to decode this packet
        ///  @param request Radius request packet if this is a response packet to be 
        ///  decoded, null if this is a request packet to be decoded
        ///  @return new RadiusPacket object
        ///  @exception IOException if an IO error occurred
        ///  @exception RadiusException if the Radius packet is malformed
        /// </summary>
        protected static RadiusPacket DecodePacket(IWritableDictionary dictionary, Stream inputStream, String sharedSecret,
                                                   RadiusPacket request)
        {
            // check shared secret
            if (string.IsNullOrEmpty(sharedSecret))
                throw new NotImplementedException("no shared secret has been set");

            // check request authenticator
            if (request != null && request.getAuthenticator() == null)
                throw new NotImplementedException("request authenticator not set");

            // read and check header
            int type = inputStream.ReadByte() & 0x0ff;
            int identifier = inputStream.ReadByte() & 0x0ff;
            int length = (inputStream.ReadByte() & 0x0ff) << 8 | (inputStream.ReadByte() & 0x0ff);

            if (request != null && request.Identifier != identifier)
                throw new RadiusException("bad packet: invalid packet identifier (request: " +
                                          request.Identifier + ", response: " + identifier);
            if (length < RadiusHeaderLength)
                throw new RadiusException("bad packet: packet too short (" + length + " bytes)");
            if (length > MaxPacketLength)
                throw new RadiusException("bad packet: packet too long (" + length + " bytes)");

            // read rest of packet
            var authenticator = new byte[16];
            var attributeData = new byte[length - RadiusHeaderLength];
            inputStream.Read(authenticator, 0, 16);
            inputStream.Read(attributeData, 0, attributeData.Length);

            // check and count attributes
            int pos = 0;
            int attributeCount = 0;
            while (pos < attributeData.Length)
            {
                if (pos + 1 >= attributeData.Length)
                    throw new RadiusException("bad packet: attribute Length mismatch");
                int attributeLength = attributeData[pos + 1] & 0x0ff;
                if (attributeLength < 2)
                    throw new RadiusException("bad packet: invalid attribute Length");
                pos += attributeLength;
                attributeCount++;
            }
            if (pos != attributeData.Length)
                throw new RadiusException("bad packet: attribute Length mismatch");

            // create RadiusPacket object; set properties
            RadiusPacket rp = createRadiusPacket(type);
            rp.Type = type;
            rp.Identifier = identifier;
            rp.authenticator = authenticator;

            // load attributes
            pos = 0;
            while (pos < attributeData.Length)
            {
                int attributeType = attributeData[pos] & 0x0ff;
                int attributeLength = attributeData[pos + 1] & 0x0ff;
                RadiusAttribute a = RadiusAttribute.CreateRadiusAttribute(dictionary, -1, attributeType);
                a.ReadAttribute(attributeData, pos, attributeLength);
                rp.AddAttribute(a);
                pos += attributeLength;
            }

            // request packet?
            if (request == null)
            {
                // decode attributes
                rp.DecodeRequestAttributes(sharedSecret);
                rp.CheckRequestAuthenticator(sharedSecret, length, attributeData);
            }
            else
            {
                // response packet: check authenticator
                rp.CheckResponseAuthenticator(sharedSecret, length, attributeData, request.getAuthenticator());
            }

            return rp;
        }

        /// <summary>
        ///  Checks the request authenticator against the supplied shared secret.
        ///  Overriden by AccountingRequest to handle special accounting request
        ///  authenticators. There is no way to check request authenticators for
        ///  authentication requests as they contain secret bytes.
        ///  @param sharedSecret shared secret
        ///  @param packetLength total Length of the packet
        ///  @param attributes request attribute data
        /// </summary>
        protected virtual void CheckRequestAuthenticator(String sharedSecret, int packetLength, byte[] attributes)
        {
        }

        /// <summary>
        ///  Can be overriden to decode encoded request attributes such as
        ///  User-Password. This method may use getAuthenticator() to get the
        ///  request authenticator.
        ///  @param sharedSecret
        /// </summary>
        protected virtual void DecodeRequestAttributes(String sharedSecret)
        {
        }

        /// <summary>
        ///  This method checks the authenticator of this Radius packet. This method
        ///  may be overriden to include special attributes in the authenticator check.
        ///  @param sharedSecret shared secret to be used to encrypt the authenticator
        ///  @param packetLength Length of the response packet
        ///  @param attributes attribute data of the response packet
        ///  @param requestAuthenticator 16 bytes authenticator of the request packet belonging
        ///  to this response packet
        /// </summary>
        protected virtual void CheckResponseAuthenticator(String sharedSecret, int packetLength, byte[] attributes,
                                                  byte[] requestAuthenticator)
        {
            byte[] authenticator = createResponseAuthenticator(sharedSecret, packetLength, attributes,
                                                               requestAuthenticator);
            byte[] receivedAuth = getAuthenticator();
            for (int i = 0; i < 16; i++)
                if (authenticator[i] != receivedAuth[i])
                    throw new RadiusException("response authenticator invalid");
        }

        /// <summary>
        ///  Returns a MD5 digest.
        ///  @return MessageDigest object
        /// </summary>
        protected virtual MD5 GetMd5Digest()
        {
            return md5Digest ?? (md5Digest = MD5.Create());
        }

        /// <summary>
        ///  Encodes the attributes of this Radius packet to a byte array.
        ///  @return byte array with encoded attributes
        ///  @throws IOException error writing data
        /// </summary>
        protected virtual byte[] GetAttributeBytes()
        {
            var bos = new MemoryStream(MaxPacketLength);
            try
            {
                foreach (var a in attributes)
                {
                    var bytes = a.WriteAttribute();
                    bos.Write(bytes, 0, bytes.Length);
                }
                bos.Flush();
                return bos.ToArray();
            }
            finally
            {
                bos.Close();
                bos.Dispose();
            }
        }
    }
}