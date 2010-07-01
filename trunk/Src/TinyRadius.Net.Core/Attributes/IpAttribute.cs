using System.Text;
using TinyRadius.Net.Util;
using System;

namespace TinyRadius.Net.Attributes
{
    /**
     * This class represents a Radius attribute for an IP number.
     */
    public class IpAttribute : RadiusAttribute
    {

        /**
         * Constructs an empty IP attribute.
         */
        public IpAttribute()
        {

        }

        /**
         * Constructs an IP attribute.
         * @param type attribute type code
         * @param value value, format: xx.xx.xx.xx
         */
        public IpAttribute(int type, String value)
        {
            Type = type;
            this.Value = value;
        }

        /**
         * Constructs an IP attribute.
         * @param type attribute type code
         * @param ipNum value as a 32 bit unsigned int
         */
        public IpAttribute(int type, long ipNum)
        {
            Type = type;
            IpAsLong = ipNum;
        }
        /**
       * Returns the attribute value (IP number) as a string of the
       * format "xx.xx.xx.xx".
       * @see TinyRadius.attribute.RadiusAttribute#getAttributeValue()
       */
        public override string Value
        {
            get
            {
                var ip = new StringBuilder();
                byte[] data = Data;
                if (data == null || data.Length != 4)
                    throw new NotImplementedException("ip attribute: expected 4 bytes attribute data");

                ip.Append(data[0] & 0x0ff)
                    .Append(".")
                    .Append(data[1] & 0x0ff)
                    .Append(".")
                    .Append(data[2] & 0x0ff)
                    .Append(".")
                    .Append(data[3] & 0x0ff);

                return ip.ToString();
            }
            set
            {
                if (value == null || value.Length < 7 || value.Length > 15)
                    throw new ArgumentException("bad IP number");

                string[] ips = value.Split('.');
                if (ips.Length != 4)
                    throw new ArgumentException("bad IP number: 4 numbers required");

                byte[] data = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    int num = Convert.ToInt32(ips[0]);
                    if (num < 0 || num > 255)
                        throw new ArgumentException("bad IP number: num out of bounds");
                    data[i] = (byte)num;
                }
                Data = data;
            }
        }



        /**
         * Gets or sets the IP number as a 32 bit unsigned number. The number is
         * returned in a long because Java does not support unsigned ints.
         * @return IP number
         */
        public long IpAsLong
        {
            get
            {
                var data = Data;
                if (data == null || data.Length != 4)
                    throw new NotImplementedException("expected 4 bytes attribute data");
                return ((long)(data[0] & 0x0ff)) << 24 | (data[1] & 0x0ff) << 16 |
                       (data[2] & 0x0ff) << 8 | (data[3] & 0x0ff);
            }
            set
            {
                var data = new byte[4];
                data[0] = (byte)((value >> 24) & 0x0ff);
                data[1] = (byte)((value >> 16) & 0x0ff);
                data[2] = (byte)((value >> 8) & 0x0ff);
                data[3] = (byte)(value & 0x0ff);
                Data = data;
            }
        }



        /**
         * Check attribute Length.
         * @see TinyRadius.attribute.RadiusAttribute#readAttribute(byte[], int, int)
         */
        public override void ReadAttribute(byte[] data, int offset, int length)
        {
            if (length != 6)
                throw new RadiusException("IP attribute: expected 6 bytes data");
            base.ReadAttribute(data, offset, length);
        }

    }
}