using System;
using System.Collections.Generic;

namespace TinyRadius.Net.Dictionaries
{
    /**
     * A dictionary that keeps the values and names in hash maps
     * in the memory. The dictionary has to be filled using the
     * methods <code>addAttributeType</code> and
     * <code>addVendor</code>.
     * @see #addAttributeType(AttributeType)
     * @see #addVendor(int, String)
     * @see TinyRadius.dictionary.Hashtable
     * @see TinyRadius.dictionary.WritableDictionary
     */

    public class MemoryDictionary : IWritableDictionary
    {
        /**
         * Returns the AttributeType for the vendor -1 from the
         * cache.
         * @param typeCode attribute type code
         * @return AttributeType or null
         * @see TinyRadius.dictionary.Dictionary#getAttributeTypeByCode(int)
         */

        private readonly Dictionary<int, Dictionary<int, AttributeType>> attributesByCode =
            new Dictionary<int, Dictionary<int, AttributeType>>(); // <Integer, <Integer, AttributeType>>

        private readonly Dictionary<String, AttributeType> attributesByName = new Dictionary<string, AttributeType>();
        // <String, AttributeType>

        private readonly Dictionary<int, string> vendorsByCode = new Dictionary<int, string>(); // <Integer, String>

        #region IWritableDictionary Members

        public AttributeType GetAttributeTypeByCode(int typeCode)
        {
            return GetAttributeTypeByCode(-1, typeCode);
        }

        /**
         * Returns the specified AttributeType object.
         * @param vendorCode vendor ID or -1 for "no vendor"
         * @param typeCode attribute type code
         * @return AttributeType or null
         * @see TinyRadius.dictionary.Hashtable#getAttributeTypeByCode(int, int)
         */

        public AttributeType GetAttributeTypeByCode(int vendorCode, int typeCode)
        {
            Dictionary<int, AttributeType> vendorAttributes = attributesByCode[vendorCode];
            if (vendorAttributes == null)
                return null;
            else
                return vendorAttributes[typeCode];
        }

        /**
         * Retrieves the attribute type with the given name.
         * @param typeName name of the attribute type 
         * @return AttributeType or null
         * @see TinyRadius.dictionary.Hashtable#getAttributeTypeByName(java.lang.String)
         */

        public AttributeType GetAttributeTypeByName(String typeName)
        {
            return attributesByName[typeName];
        }

        /**
         * Searches the vendor with the given name and returns its
         * code. This method is seldomly used.
         * @param vendorName vendor name
         * @return vendor code or -1
         * @see TinyRadius.dictionary.Hashtable#getVendorId(java.lang.String)
         */

        public int GetVendorId(String vendorName)
        {
            foreach (var entry in vendorsByCode)
            {
                if (entry.Value == vendorName)
                    return entry.Key;
            }
            return -1;

        }

        /**
         * Retrieves the name of the vendor with the given code from
         * the cache.
         * @param vendorId vendor number
         * @return vendor name or null
         * @see TinyRadius.dictionary.Hashtable#getVendorName(int)
         */

        public String GetVendorName(int vendorId)
        {
            if (!vendorsByCode.ContainsKey(vendorId))
                return null;
            return vendorsByCode[vendorId];
        }

        /**
         * Adds the given vendor to the cache.
         * @param vendorId vendor ID
         * @param vendorName name of the vendor
         * @exception ArgumentException empty vendor name, invalid vendor ID
         */

        public void AddVendor(int vendorId, String vendorName)
        {
            if (vendorId < 0)
                throw new ArgumentException("vendor ID must be positive");
            if (GetVendorName(vendorId) != null)
                throw new ArgumentException("duplicate vendor code");
            if (string.IsNullOrEmpty(vendorName))
                throw new ArgumentException("vendor name empty");
            vendorsByCode.Add(vendorId, vendorName);
        }

        /**
         * Adds an AttributeType object to the cache.
         * @param attributeType AttributeType object
         * @exception ArgumentException duplicate attribute name/type code
         */

        public void AddAttributeType(AttributeType attributeType)
        {
            if (attributeType == null)
                throw new ArgumentException("attribute type must not be null");

            int vendorId = attributeType.VendorId;
            int typeCode = attributeType.TypeCode;
            String attributeName = attributeType.Name;

            if (attributesByName.ContainsKey(attributeName))
                throw new ArgumentException("duplicate attribute name: " + attributeName);

            Dictionary<int, AttributeType> vendorAttributes;
            if (!attributesByCode.ContainsKey(vendorId))
            {
                vendorAttributes = new Dictionary<int, AttributeType>();
                attributesByCode.Add(vendorId, vendorAttributes);
            }
            vendorAttributes = attributesByCode[vendorId];

            if (vendorAttributes.ContainsKey(typeCode))
                throw new ArgumentException("duplicate type code: " + typeCode);

            attributesByName.Add(attributeName, attributeType);
            vendorAttributes.Add(typeCode, attributeType);
        }

        #endregion
    }
}