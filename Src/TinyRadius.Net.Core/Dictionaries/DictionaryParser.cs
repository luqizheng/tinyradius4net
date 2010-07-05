using System;
using System.IO;
using TinyRadius.Net.Attributes;

namespace TinyRadius.Net.Dictionaries
{
    /// <summary>
    /// Parses a dictionary in "Radiator format" and fills a
    /// WritableDictionary.
    /// </summary>
    public class DictionaryParser
    {
        /// <summary>
        /// Returns a new dictionary filled with the contents
        /// from the given input stream.
        /// @param source input stream
        /// @return dictionary object
        /// @throws IOException
        /// </summary>
        //public static IDictionary ParseDictionary(Stream source)
        //{
        //    IWritableDictionary d = new MemoryDictionary();
        //    ParseDictionary(source, d);
        //    return d;
        //}

        /// <summary>
        /// Parses the dictionary from the specified InputStream.
        /// @param source input stream
        /// @param dictionary dictionary data is written to
        /// @throws IOException syntax errors
        /// @throws NotImplementedException syntax errors
        /// </summary>
        public static void ParseDictionary(Stream source, IWritableDictionary dictionary)
        {
            // read each line separately
            var @in = new StreamReader(source);

            String line;
            int lineNum = -1;
            while ((line = @in.ReadLine()) != null)
            {
                // ignore comments
                lineNum++;
                line = line.Trim();
                if (line.StartsWith("#") || line.Length == 0)
                    continue;

                // tokenize line by whitespace
                string[] tok = System.Text.RegularExpressions.Regex.Split(line, "[\\t ]+");
                String lineType = tok[0].ToUpper();
                switch (lineType)
                {
                    case ("ATTRIBUTE"):
                        parseAttributeLine(dictionary, tok, lineNum);
                        break;
                    case ("VALUE"):
                        ParseValueLine(dictionary, tok, lineNum);
                        break;
                    case ("$INCLUDE"):
                        IncludeDictionaryFile(dictionary, tok, lineNum);
                        break;
                    case ("VENDORATTR"):
                        ParseVendorAttributeLine(dictionary, tok, lineNum);
                        break;
                    case ("VENDOR"):
                        ParseVendorLine(dictionary, tok, lineNum);
                        break;
                    default:
                        throw new IOException("unknown line type: " + lineType + " line: " + lineNum);
                }
            }
        }

        /// <summary>
        /// Parse a line that declares an attribute.
        /// </summary>
        private static void parseAttributeLine(IWritableDictionary dictionary, string[] tok, int lineNum)
        {
            if (tok.Length != 4)
            {
                throw new IOException("syntax error on line " + lineNum);
            }

            // read name, code, type
            String name = tok[1];
            int code = Convert.ToInt32(tok[2]);
            String typeStr = tok[3];

            // translate type to class
            Type type = code == VendorSpecificAttribute.VENDOR_SPECIFIC ? typeof(VendorSpecificAttribute) : GetAttributeTypeClass(code, typeStr);

            // create and cache object
            dictionary.AddAttributeType(new AttributeType(code, name, type));
        }

        /// <summary>
        /// Parses a VALUE line containing an enumeration value.
        /// </summary>
        private static void ParseValueLine(IWritableDictionary dictionary, string[] tok, int lineNum)
        {
            try
            {
                if (tok.Length != 4)
                    throw new IOException("expect 4 columns but actual is " + tok.Length);

                String typeName = tok[1].Trim();
                String enumName = tok[2].Trim();
                String valStr = tok[3].Trim();

                AttributeType at = dictionary.GetAttributeTypeByName(typeName);
                if (at == null)
                    throw new IOException("unknown attribute type: " + typeName + ", line: " + lineNum);
                at.AddEnumerationValue(Convert.ToInt32(valStr), enumName);
            }
            catch (Exception ex)
            {
                throw new IOException("syntax error on line" + lineNum, ex);
            }
        }

        /// <summary>
        /// Parses a line that declares a Vendor-Specific attribute.
        /// </summary>
        private static void ParseVendorAttributeLine(IWritableDictionary dictionary, string[] tok, int lineNum)
        {
            if (tok.Length != 5)
                throw new IOException("syntax error on line " + lineNum);

            String vendor = tok[1].Trim();
            String name = tok[2].Trim();
            int code = Convert.ToInt32(tok[3].Trim());
            String typeStr = tok[4].Trim();

            Type type = GetAttributeTypeClass(code, typeStr);
            var at = new AttributeType(Convert.ToInt32(vendor), code, name, type);
            dictionary.AddAttributeType(at);
        }

        /// <summary>
        /// Parses a line containing a vendor declaration.
        /// </summary>
        private static void ParseVendorLine(IWritableDictionary dictionary, string[] tok, int lineNum)
        {
            if (tok.Length != 3)
                throw new IOException("syntax error on line " + lineNum);

            int vendorId = Convert.ToInt32(tok[1].Trim());
            String vendorName = tok[2].Trim();
            dictionary.AddVendor(vendorId, vendorName);
        }

        /// <summary>
        /// Includes a dictionary file.
        /// </summary>
        private static void IncludeDictionaryFile(IWritableDictionary dictionary, string[] tok, int lineNum)
        {
            if (tok.Length != 2)
                throw new IOException("syntax error on line " + lineNum);
            String includeFile = tok[1];

            var incf = new FileInfo(includeFile);
            if (!incf.Exists)
                throw new IOException("inclueded file '" + includeFile + "' not found, line " + lineNum);

            FileStream fs = incf.OpenRead();
            ParseDictionary(fs, dictionary);

            // line numbers begin with 0 again, but file name is
            // not mentioned in exceptions
            // furthermore, this method does not allow to include
            // classpath resources
        }

        /// <summary>
        /// Returns the RadiusAttribute descendant class for the given
        /// attribute type.
        /// @param typeStr string|octets|integer|date|ipaddr
        /// @return RadiusAttribute class or descendant
        /// </summary>
        private static Type GetAttributeTypeClass(int attributeType, String typeStr)
        {
            Type type = typeof(RadiusAttribute);
            typeStr = typeStr.ToLower();
            switch (typeStr)
            {
                case "string":
                    type = typeof(StringAttribute);
                    break;
                case "octets":
                    type = typeof(RadiusAttribute);
                    break;
                case "date":
                case "integer":
                    type = typeof(IntegerAttribute);
                    break;
                case "ipaddr":
                    type = typeof(IpAttribute);
                    break;
            }
            return type;
        }
    }
}