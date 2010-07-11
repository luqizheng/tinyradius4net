using System;
using System.Collections.Generic;

namespace TinyRadius.Net.Dictionaries
{
    /// <summary>
    /// A dictionary that keeps the values and names in hash maps
    /// in the memory. The dictionary has to be filled using the
    /// methods <see cref="AddAttributeType"/> and
    /// <code>addVendor</code>.
    /// @see #addAttributeType(AttributeType)
    /// @see #addVendor(int, String)
    /// @see TinyRadius.dictionary.Hashtable
    /// @see TinyRadius.dictionary.WritableDictionary
    /// </summary>
    public class MemoryDictionary : IWritableDictionary
    {
        /// <summary>
        /// Returns the AttributeType for the vendor -1 from the
        /// cache.
        /// @param typeCode attribute type code
        /// @return AttributeType or null
        /// @see TinyRadius.dictionary.Dictionary#getAttributeTypeByCode(int)
        /// </summary>
        private readonly Dictionary<int, Dictionary<int, AttributeType>> _attributesByCode =
            new Dictionary<int, Dictionary<int, AttributeType>>(); // <Integer, <Integer, AttributeType>>

        private readonly Dictionary<String, AttributeType> _attributesByName = new Dictionary<string, AttributeType>();
        // <String, AttributeType>

        private readonly Dictionary<int, string> _vendorsByCode = new Dictionary<int, string>(); // <Integer, String>

        #region IWritableDictionary Members

        public AttributeType GetAttributeTypeByCode(int typeCode)
        {
            return GetAttributeTypeByCode(-1, typeCode);
        }

        /// <summary>
        /// Returns the specified AttributeType object.
        /// @param vendorCode vendor ID or -1 for "no vendor"
        /// @param typeCode attribute type code
        /// @return AttributeType or null
        /// @see TinyRadius.dictionary.Hashtable#getAttributeTypeByCode(int, int)
        /// </summary>
        public AttributeType GetAttributeTypeByCode(int vendorCode, int typeCode)
        {
            if (!_attributesByCode.ContainsKey(vendorCode))
                return null;
            Dictionary<int, AttributeType> vendorAttributes = _attributesByCode[vendorCode];
            return !vendorAttributes.ContainsKey(typeCode) ? null : vendorAttributes[typeCode];
        }

        /// <summary>
        /// Retrieves the attribute type with the given name.
        /// @param typeName name of the attribute type 
        /// @return AttributeType or null
        /// @see TinyRadius.dictionary.Hashtable#getAttributeTypeByName(java.lang.String)
        /// </summary>
        public AttributeType GetAttributeTypeByName(String typeName)
        {
            return _attributesByName[typeName];
        }

        /// <summary>
        /// Searches the vendor with the given name and returns its
        /// code. This method is seldomly used.
        /// @param vendorName vendor name
        /// @return vendor code or -1
        /// @see TinyRadius.dictionary.Hashtable#getVendorId(java.lang.String)
        /// </summary>
        public int GetVendorId(String vendorName)
        {
            foreach (var entry in _vendorsByCode)
            {
                if (entry.Value == vendorName)
                    return entry.Key;
            }
            return -1;
        }

        /// <summary>
        /// Retrieves the name of the vendor with the given code from
        /// the cache.
        /// @param vendorId vendor number
        /// @return vendor name or null
        /// @see TinyRadius.dictionary.Hashtable#getVendorName(int)
        /// </summary>
        public String GetVendorName(int vendorId)
        {
            if (!_vendorsByCode.ContainsKey(vendorId))
                return null;
            return _vendorsByCode[vendorId];
        }

        /// <summary>
        /// Adds the given vendor to the cache.
        /// @param vendorId vendor ID
        /// @param vendorName name of the vendor
        /// @exception ArgumentException empty vendor name, invalid vendor ID
        /// </summary>
        public void AddVendor(int vendorId, String vendorName)
        {
            if (vendorId < 0)
                throw new ArgumentException("vendor ID must be positive");
            if (GetVendorName(vendorId) != null)
                throw new ArgumentException("duplicate vendor code");
            if (string.IsNullOrEmpty(vendorName))
                throw new ArgumentException("vendor name empty");
            _vendorsByCode.Add(vendorId, vendorName);
        }

        /// <summary>
        /// Adds an AttributeType object to the cache.
        /// @param attributeType AttributeType object
        /// @exception ArgumentException duplicate attribute name/type code
        /// </summary>
        public void AddAttributeType(AttributeType attributeType)
        {
            if (attributeType == null)
                throw new ArgumentException("attribute type must not be null");

            int vendorId = attributeType.VendorId;
            int typeCode = attributeType.TypeCode;
            String attributeName = attributeType.Name;

            if (_attributesByName.ContainsKey(attributeName))
                throw new ArgumentException("duplicate attribute name: " + attributeName);

            Dictionary<int, AttributeType> vendorAttributes;
            if (!_attributesByCode.ContainsKey(vendorId))
            {
                vendorAttributes = new Dictionary<int, AttributeType>();
                _attributesByCode.Add(vendorId, vendorAttributes);
            }
            vendorAttributes = _attributesByCode[vendorId];

            if (vendorAttributes.ContainsKey(typeCode))
                throw new ArgumentException("duplicate type code: " + typeCode);

            _attributesByName.Add(attributeName, attributeType);
            vendorAttributes.Add(typeCode, attributeType);
        }

        #endregion
    }
}