/**
 * $Id: DictionaryParser.java,v 1.2 2005/09/06 16:38:40 wuttke Exp $
 * Created on 28.08.2005
 * @author mw
 * @version $Revision: 1.2 $
 */
using TinyRadius.Net.Directories;
/*using java.io.BufferedReader;
using java.io.File;
using java.io.FileInputStream;
using java.io.IOException;
using java.io.InputStream;
using java.io.InputStreamReader;
using java.util.string;*/
using TinyRadius.Net.Attribute;
using System;
using System.IO;

namespace TinyRadius.Net.Directories
{

    /**
     * Parses a dictionary in "Radiator format" and fills a
     * WritableDictionary.
     */
    public class DictionaryParser
    {

        /**
         * Returns a new dictionary filled with the contents
         * from the given input stream.
         * @param source input stream
         * @return dictionary object
         * @throws IOException
         */
        public static Hashtable parseDictionary(Stream source)
        {
            IWritableDictionary d = new MemoryDictionary();
            parseDictionary(source, d);
            return d;
        }

        /**
         * Parses the dictionary from the specified InputStream.
         * @param source input stream
         * @param dictionary dictionary data is written to
         * @throws IOException syntax errors
         * @throws RuntimeException syntax errors
         */
        public static void parseDictionary(Stream source, IWritableDictionary dictionary)
        {
            // read each line separately
            StreamReader @in = new StreamReader(source);

            String line;
            int lineNum = -1;
            while ((line = @in.readLine()) != null)
            {
                // ignore comments
                lineNum++;
                line = line.trim();
                if (line.startsWith("#") || line.length() == 0)
                    continue;

                // tokenize line by whitespace
                string[] tok = line.Split('\t');
                String lineType = tok[0].ToUpper();
                if (lineType == ("ATTRIBUTE"))
                    parseAttributeLine(dictionary, tok, lineNum);
                else if (lineType == ("VALUE"))
                    parseValueLine(dictionary, tok, lineNum);
                else if (lineType == ("$INCLUDE"))
                    includeDictionaryFile(dictionary, tok, lineNum);
                else if (lineType == ("VENDORATTR"))
                    parseVendorAttributeLine(dictionary, tok, lineNum);
                else if (lineType == ("VENDOR"))
                    parseVendorLine(dictionary, tok, lineNum);
                else
                    throw new IOException("unknown line type: " + lineType + " line: " + lineNum);
            }
        }

        /**
         * Parse a line that declares an attribute.
         */
        private static void parseAttributeLine(IWritableDictionary dictionary, string[] tok, int lineNum)
        {
            if (tok.countTokens() != 4)
            {
                throw new IOException("syntax error on line " + lineNum);
            }

            // read name, code, type
            String name = tok[1];
            int code = Convert.ToInt32(tok[2]);
            String typeStr = tok[3];

            // translate type to class
            Type type;
            if (code == VendorSpecificAttribute.VENDOR_SPECIFIC)
            {
                type = typeof(VendorSpecificAttribute);
            }
            else
            {
                type = getAttributeTypeClass(code, typeStr);
            }

            // create and cache object
            dictionary.addAttributeType(new AttributeType(code, name, type));
        }

        /**
         * Parses a VALUE line containing an enumeration value.
         */
        private static void parseValueLine(IWritableDictionary dictionary, string[] tok, int lineNum)
        {
            if (tok.countTokens() != 4)
                throw new IOException("syntax error on line " + lineNum);

            String typeName = tok[1].Trim();
            String enumName = tok[2].Trim();
            String valStr = tok[3].Trim();

            AttributeType at = MemoryDictionary.getAttributeTypeByName(typeName);
            if (at == null)
                throw new IOException("unknown attribute type: " + typeName + ", line: " + lineNum);
            else
                at.addEnumerationValue(Convert.ToInt32(valStr), enumName);
        }

        /**
         * Parses a line that declares a Vendor-Specific attribute.
         */
        private static void parseVendorAttributeLine(IWritableDictionary dictionary, string[] tok, int lineNum)
        {
            if (tok.countTokens() != 5)
                throw new IOException("syntax error on line " + lineNum);

            String vendor = tok[1].Trim();
            String name = tok[2].Trim();
            int code = Convert.ToInt32(tok[3].Trim());
            String typeStr = tok[4].Trim();

            Type type = getAttributeTypeClass(code, typeStr);
            AttributeType at = new AttributeType(Convert.ToInt32(vendor), code, name, type);
            dictionary.addAttributeType(at);
        }

        /**
         * Parses a line containing a vendor declaration.
         */
        private static void parseVendorLine(IWritableDictionary dictionary, string[] tok, int lineNum)
        {
            if (tok.countTokens() != 2)
                throw new IOException("syntax error on line " + lineNum);

            int vendorId = Convert.ToInt32(tok.nextToken().trim());
            String vendorName = tok.nextToken().trim();

            dictionary.addVendor(vendorId, vendorName);
        }

        /**
         * Includes a dictionary file.
         */
        private static void includeDictionaryFile(IWritableDictionary dictionary, string[] tok, int lineNum)
        {
            if (tok.countTokens() != 1)
                throw new IOException("syntax error on line " + lineNum);
            String includeFile = tok.nextToken();

            File incf = new File(includeFile);
            if (!incf.exists())
                throw new IOException("inclueded file '" + includeFile + "' not found, line " + lineNum);

            FileInputStream fis = new FileInputStream(incf);
            parseDictionary(fis, dictionary);

            // line numbers begin with 0 again, but file name is
            // not mentioned in exceptions
            // furthermore, this method does not allow to include
            // classpath resources
        }

        /**
         * Returns the RadiusAttribute descendant class for the given
         * attribute type.
         * @param typeStr string|octets|integer|date|ipaddr
         * @return RadiusAttribute class or descendant
         */
        private static Type getAttributeTypeClass(int attributeType, String typeStr)
        {
            var type = typeof(RadiusAttribute);
            typeStr = typeStr.ToLower();
            if (typeStr == "string")
                type = typeof(StringAttribute);
            else if (typeStr.equalsIgnoreCase("octets"))
                type = typeof(RadiusAttribute);
            else if (typeStr == "integer" || typeStr = "date")
                type = typeof(IntegerAttribute);
            else if (typeStr.equalsIgnoreCase("ipaddr"))
                type = typeof(IpAttribute);
            return type;
        }

    }
}

