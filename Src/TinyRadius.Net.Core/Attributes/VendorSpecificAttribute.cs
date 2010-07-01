using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TinyRadius.Net.Dictionaries;
using TinyRadius.Net.Util;

namespace TinyRadius.Net.Attributes
{
    /// <summary>
    /// This class represents a "Vendor-Specific" attribute.
    /// </summary>
    public class VendorSpecificAttribute : RadiusAttribute
    {
        /// <summary>
        /// Radius attribute type code for Vendor-Specific
        /// </summary>
        public static readonly int VENDOR_SPECIFIC = 26;

        /// <summary>
        /// Sub attributes. Only set if isRawData == false.
        /// </summary>
        private List<RadiusAttribute> subAttributes = new List<RadiusAttribute>();

        /// <summary>
        /// Constructs an empty Vendor-Specific attribute that can be read from a
        /// Radius packet.
        /// </summary>
        public VendorSpecificAttribute()
        {
        }

        /// <summary>
        /// Constructs a new Vendor-Specific attribute to be sent.
        /// @param vendorId vendor ID of the sub-attributes
        /// </summary>
        public VendorSpecificAttribute(int vendorId)
        {
            Type = VENDOR_SPECIFIC;
            ChildVendorId = vendorId;
        }

        /// <summary>
        /// Gets or sets  the vendor ID of the child attributes.
        /// @param childVendorId
        /// </summary>
        public int ChildVendorId { get; set; }

        public override IWritableDictionary Dictionary
        {
            get { return base.Dictionary; }
            set
            {
                base.Dictionary = value;
                foreach (RadiusAttribute a in subAttributes)
                {
                    a.Dictionary = value;
                }
            }
        }

        /// <summary>
        /// Returns the list of sub-attributes.
        /// @return ArrayList of RadiusAttribute objects
        /// </summary>
        public List<RadiusAttribute> SubAttributes
        {
            get { return subAttributes; }
        }

        /// <summary>
        /// Adds a sub-attribute to this attribute.
        /// @param attribute sub-attribute to add
        /// </summary>
        public void AddSubAttribute(RadiusAttribute attribute)
        {
            if (attribute.VendorId != ChildVendorId)
                throw new ArgumentException(
                    "sub attribut has incorrect vendor ID");

            subAttributes.Add(attribute);
        }

        /// <summary>
        /// Adds a sub-attribute with the specified name to this attribute.
        /// @param name name of the sub-attribute
        /// @param value value of the sub-attribute
        /// @exception ArgumentException invalid sub-attribute name or value
        /// </summary>
        public void AddSubAttribute(String name, String value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("type name is empty");
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("value is empty");

            AttributeType type = this.Dictionary.GetAttributeTypeByName(name);
            if (type == null)
                throw new ArgumentException("unknown attribute type '"
                                            + name + "'");
            if (type.VendorId == -1)
                throw new ArgumentException("attribute type '" + name
                                            + "' is not a Vendor-Specific sub-attribute");
            if (type.VendorId != ChildVendorId)
                throw new ArgumentException("attribute type '" + name
                                            + "' does not belong to vendor ID " + ChildVendorId);

            RadiusAttribute attribute = CreateRadiusAttribute(Dictionary,
                                                              ChildVendorId, type.TypeCode);
            attribute.Value = value;
            AddSubAttribute(attribute);
        }

        /// <summary>
        /// Removes the specified sub-attribute from this attribute.
        /// @param attribute RadiusAttribute to remove
        /// </summary>
        public void RemoveSubAttribute(RadiusAttribute attribute)
        {
            if (!subAttributes.Remove(attribute))
                throw new ArgumentException("no such attribute");
        }

        /// <summary>
        /// Returns all sub-attributes of this attribut which have the given type.
        /// @param attributeType type of sub-attributes to get
        /// @return list of RadiusAttribute objects, does not return null
        /// </summary>
        public List<RadiusAttribute> GetSubAttributes(int attributeType)
        {
            if (attributeType < 1 || attributeType > 255)
                throw new ArgumentException(
                    "sub-attribute type out of bounds");

            var result = new List<RadiusAttribute>();
            foreach (RadiusAttribute a in subAttributes)
            {
                if (attributeType == a.Type)
                    result.Add(a);
            }
            return result;
        }

        /// <summary>
        /// Returns a sub-attribute of the given type which may only occur once in
        /// this attribute.
        /// @param type sub-attribute type
        /// @return RadiusAttribute object or null if there is no such sub-attribute
        /// @throws NotImplementedException if there are multiple occurences of the
        /// requested sub-attribute type
        /// </summary>
        public RadiusAttribute GetSubAttribute(int type)
        {
            List<RadiusAttribute> attrs = GetSubAttributes(type);
            if (attrs.Count > 1)
                throw new NotImplementedException(
                    "multiple sub-attributes of requested type " + type);
            else if (attrs.Count == 0)
                return null;
            else
                return attrs[0];
        }

        /// <summary>
        /// Returns a single sub-attribute of the given type name.
        /// @param type attribute type name
        /// @return RadiusAttribute object or null if there is no such attribute
        /// @throws NotImplementedException if the attribute occurs multiple times
        /// </summary>
        public RadiusAttribute GetSubAttribute(String type)
        {
            if (string.IsNullOrEmpty(type))
                throw new ArgumentException("type name is empty");


            AttributeType t = Dictionary.GetAttributeTypeByName(type);
            if (t == null)
                throw new ArgumentException("unknown attribute type name '"
                                            + type + "'");
            if (t.VendorId != ChildVendorId)
                throw new ArgumentException("vendor ID mismatch");

            return GetSubAttribute(t.TypeCode);
        }

        /// <summary>
        /// Returns the value of the Radius attribute of the given type or null if
        /// there is no such attribute.
        /// @param type attribute type name
        /// @return value of the attribute as a string or null if there is no such
        /// attribute
        /// @throws ArgumentException if the type name is unknown
        /// @throws NotImplementedException attribute occurs multiple times
        /// </summary>
        public String GetSubAttributeValue(String type)
        {
            RadiusAttribute attr = GetSubAttribute(type);
            if (attr == null)
                return null;
            else
                return attr.Value;
        }

        /// <summary>
        /// Renders this attribute as a byte array.
        /// @see TinyRadius.attribute.RadiusAttribute#writeAttribute()
        /// </summary>
        public override byte[] WriteAttribute()
        {
            // write vendor ID
            var bos = new MemoryStream(255);
            bos.WriteByte(Convert.ToByte(ChildVendorId >> 24 & 0x0ff));
            bos.WriteByte(Convert.ToByte(ChildVendorId >> 16 & 0x0ff));
            bos.WriteByte(Convert.ToByte(ChildVendorId >> 8 & 0x0ff));
            bos.WriteByte(Convert.ToByte(ChildVendorId & 0x0ff));

            // write sub-attributes
            try
            {
                foreach (RadiusAttribute a in subAttributes)
                {
                    byte[] c = a.WriteAttribute();
                    bos.Write(c, 0, c.Length);
                }
            }
            catch (IOException ioe)
            {
                // occurs never
                throw new NotImplementedException("error writing data", ioe);
            }

            // check data Length
            byte[] attrData = bos.ToArray();
            int len = attrData.Length;
            if (len > 253)
                throw new NotImplementedException("Vendor-Specific attribute too long: "
                                                  + len);

            // compose attribute
            var attr = new byte[len + 2];
            attr[0] = Convert.ToByte(VENDOR_SPECIFIC); // code
            attr[1] = (byte) (len + 2); // Length
            Array.Copy(attrData, 0, attr, 2, len);
            return attr;
        }

        /// <summary>
        /// Reads a Vendor-Specific attribute and decodes the internal sub-attribute
        /// structure.
        /// @see TinyRadius.attribute.RadiusAttribute#readAttribute(byte[], int,
        /// int)
        /// </summary>
        public override void ReadAttribute(byte[] data, int offset, int length)
        {
            // check Length
            if (length < 6)
                throw new RadiusException("Vendor-Specific attribute too short: "
                                          + length);

            int vsaCode = data[offset];
            int vsaLen = (data[offset + 1] & 0x000000ff) - 6;

            if (vsaCode != VENDOR_SPECIFIC)
                throw new RadiusException("not a Vendor-Specific attribute");

            // read vendor ID and vendor data
            /*
             * int vendorId = (data[offset + 2] << 24 | data[offset + 3] << 16 |
             * data[offset + 4] << 8 | ((int)data[offset + 5] & 0x000000ff));
             */
            int vendorId = (UnsignedByteToInt(data[offset + 2]) << 24
                            | UnsignedByteToInt(data[offset + 3]) << 16
                            | UnsignedByteToInt(data[offset + 4]) << 8 | UnsignedByteToInt(data[offset + 5]));
            ChildVendorId = vendorId;

            // validate sub-attribute structure
            int pos = 0;
            int count = 0;
            while (pos < vsaLen)
            {
                if (pos + 1 >= vsaLen)
                    throw new RadiusException("Vendor-Specific attribute malformed");
                // int vsaSubType = data[(offset + 6) + pos] & 0x0ff;
                int vsaSubLen = data[(offset + 6) + pos + 1] & 0x0ff;
                pos += vsaSubLen;
                count++;
            }
            if (pos != vsaLen)
                throw new RadiusException("Vendor-Specific attribute malformed");

            subAttributes = new List<RadiusAttribute>(count);
            pos = 0;
            while (pos < vsaLen)
            {
                int subtype = data[(offset + 6) + pos] & 0x0ff;
                int sublength = data[(offset + 6) + pos + 1] & 0x0ff;
                RadiusAttribute a = CreateRadiusAttribute(Dictionary,
                                                          vendorId, subtype);
                a.ReadAttribute(data, (offset + 6) + pos, sublength);
                subAttributes.Add(a);
                pos += sublength;
            }
        }

        private static int UnsignedByteToInt(byte b)
        {
            return b & 0xFF;
        }

        /// <summary>
        /// Returns a string representation for debugging.
        /// @see TinyRadius.attribute.RadiusAttribute#toString()
        /// </summary>
        public override String ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Vendor-Specific: ");
            int vendorId = ChildVendorId;
            String vendorName = Dictionary.GetVendorName(vendorId);
            if (vendorName != null)
            {
                sb.Append(vendorName)
                    .Append(" (")
                    .Append(vendorId)
                    .Append(")");
            }
            else
            {
                sb.Append("vendor ID ");
                sb.Append(vendorId);
            }
            foreach (RadiusAttribute attr in SubAttributes)
            {
                sb.Append("\n");
                sb.Append(attr.ToString());
            }
            return sb.ToString();
        }
    }
}