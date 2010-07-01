using System;
using System.Text;

namespace TinyRadius.Net.Util
{
    /**
     * This class contains miscellaneous static utility functions.
     */

    public class RadiusUtil
    {
        /**
         * Returns the passed string as a byte array containing the
         * string in UTF-8 representation.
         * @param str Java string
         * @return UTF-8 byte array
         */

        public static byte[] GetUtf8Bytes(String str)
        {
            try
            {
                return Encoding.UTF8.GetBytes(str);
            }
            catch (EncoderFallbackException uee)
            {
                //return str.getBytes();
                return Encoding.Default.GetBytes(str);
            }
        }

        /**
         * Creates a string from the passed byte array containing the
         * string in UTF-8 representation.
         * @param utf8 UTF-8 byte array
         * @return Java string
         */

        public static String GetStringFromUtf8(byte[] utf8)
        {
            try
            {
                return Encoding.UTF8.GetString(utf8);
                //return new String(utf8, "UTF-8");
            }
            catch
            {
                return BitConverter.ToString(utf8);
            }
        }

        /**
         * Returns the byte array as a hex string in the format
         * "0x1234".
         * @param data byte array
         * @return hex string
         */

        public static String GetHexString(byte[] data)
        {
            var hex = new StringBuilder("0x");
            if (data != null)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    //String digit = Integer.toString(data[i] & 0x0ff, 16);
                    string digit = Convert.ToString(data[i], 16);
                    if (digit.Length < 2)
                        hex.Append('0');
                    hex.Append(digit);
                }
            }
            return hex.ToString();
        }
    }
}