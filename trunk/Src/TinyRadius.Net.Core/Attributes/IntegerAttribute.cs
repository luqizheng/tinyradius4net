using System;
using TinyRadius.Net.Dictionaries;
using TinyRadius.Net.Util;

namespace TinyRadius.Net.Attributes
{
    /// <summary>
    ///  This class represents a Radius attribute which only
    ///  contains a 32 bit integer.
    /// </summary>

    public class IntegerAttribute : RadiusAttribute
    {
        /// <summary>
        ///  Constructs an empty integer attribute.
        /// </summary>

        public IntegerAttribute()
        {
        }

        /// <summary>
        ///  Constructs an integer attribute with the given value.
        ///  @param type attribute type
        ///  @param value attribute value
        /// </summary>

        public IntegerAttribute(int type, int value)
        {
            Type = type;
            ValueInt = value;
        }

        /// <summary>
        ///  Returns the string value of this attribute.
        ///  @return a string
        /// </summary>

        public int ValueInt
        {
            get
            {
                byte[] data = Data;
                return (((data[0] & 0x0ff) << 24) | ((data[1] & 0x0ff) << 16) |
                        ((data[2] & 0x0ff) << 8) | (data[3] & 0x0ff));
            }
            set
            {
                var data = new byte[4];
                data[0] = (byte)(value >> 24 & 0x0ff);
                data[1] = (byte)(value >> 16 & 0x0ff);
                data[2] = (byte)(value >> 8 & 0x0ff);
                data[3] = (byte)(value & 0x0ff);
                Data = data;
            }
        }

        /// <summary>
        ///  Returns the value of this attribute as a string.
        ///  Tries to resolve enumerations.
        ///  @see TinyRadius.attribute.RadiusAttribute#getAttributeValue()
        /// </summary>

        public override string Value
        {
            get
            {
                int value = ValueInt;
                AttributeType at = GetAttributeTypeObject();
                if (at != null)
                {
                    String name = at.GetEnumeration(value);
                    if (name != null)
                        return name;
                }
                return value.ToString();
            }
            set
            {
                AttributeType at = GetAttributeTypeObject();
                if (at != null)
                {
                    int val = at.GetEnumeration(value);
                    if (val != -1)
                    {
                        ValueInt = val;
                        return;
                    }
                }

                ValueInt = Convert.ToInt32(value);
            }
        }



        /// <summary>
        ///  Check attribute Length.
        ///  @see TinyRadius.attribute.RadiusAttribute#readAttribute(byte[], int, int)
        /// </summary>

        public override void ReadAttribute(byte[] data, int offset, int length)
        {
            if (length != 6)
                throw new RadiusException("integer attribute: expected 4 bytes data");
            base.ReadAttribute(data, offset, length);
        }
    }
}