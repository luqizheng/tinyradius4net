using System;
using System.Collections.Generic;
using TinyRadius.Net.Attributes;

namespace TinyRadius.Net.Dictionaries
{
    /// <summary>
    /// Represents a Radius attribute type.
    /// </summary>
    public class AttributeType
    {
        private Type _attributeClass;
        private Dictionary<int, string> _enumeration;
        private String _name;
        private int _typeCode;

        /// <summary>
        /// Create a new attribute type.
        /// @param code Radius attribute type code
        /// @param name Attribute type name
        /// @param type RadiusAttribute descendant who handles
        /// attributes of this type
        /// </summary>
        public AttributeType(int code, String name, Type type)
        {
            VendorId = -1;
            TypeCode = code;
            Name = name;
            Class = type;
        }

        /// <summary>
        /// Constructs a Vendor-Specific sub-attribute type.
        /// @param vendor vendor ID
        /// @param code sub-attribute type code
        /// @param name sub-attribute name
        /// @param type sub-attribute class
        /// </summary>
        public AttributeType(int vendor, int code, String name, Type type)
        {
            TypeCode = code;
            Name = name;
            Class = type;
            VendorId = vendor;
        }

        /// <summary>
        /// Sets the Radius type code for this attribute type.
        /// @param code type code, 1-255
        /// </summary>
        public int TypeCode
        {
            set
            {
                if (value < 1 || value > 255)
                    throw new ArgumentException("code out of bounds");
                _typeCode = value;
            }
            get { return _typeCode; }
        }

        /// <summary>
        /// Sets the name of this type.
        /// @param name type name
        /// </summary>
        public string Name
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("name is empty");
                this._name = value;
            }
            get { return _name; }
        }

        /// <summary>
        /// Sets the RadiusAttribute descendant class which represents
        /// attributes of this type.
        /// </summary>
        public Type Class
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value", "Class is null");
                if (typeof(RadiusAttribute) != value)
                    throw new ArgumentException("type is not a RadiusAttribute descendant");
                _attributeClass = value;
            }
            get { return _attributeClass; }
        }

        /// <summary>
        /// Returns the vendor ID.
        /// No vendor specific attribute = -1 
        /// @return vendor ID
        /// </summary>
        public int VendorId { get; set; }

        /// <summary>
        /// Returns the name of the given integer value if this attribute
        /// is an enumeration, or null if it is not or if the integer value
        /// is unknown. 
        /// @return name
        /// </summary>
        public String GetEnumeration(int value)
        {
            if (_enumeration != null)
                /*return (String) enumeration.get(new Integer(value));*/
                return _enumeration[value];
            else
                return null;
        }

        /// <summary>
        /// Returns the number of the given string value if this attribute is
        /// an enumeration, or null if it is not or if the string value is unknown.
        /// @param value string value
        /// @return Integer or -1
        /// </summary>
        public int GetEnumeration(String value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("value is empty");
            if (_enumeration == null)
                return -1;
            foreach (var entry in _enumeration)
            {
                if (entry.Value == value)
                    return entry.Key;
            }
            return -1;
            /*for (Iterator i = enumeration.entrySet().iterator(); i.hasNext();)
            {
                var e = (Map.Entry) i.next();
                if (e.getValue().equals(value))
                    return (Integer) e.getKey();
            }
            return null;*/
        }

        /// <summary>
        /// Adds a name for an integer value of this attribute.
        /// @param num number that shall get a name
        /// @param name the name for this number
        /// </summary>
        public void AddEnumerationValue(int num, String name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name is empty");
            if (_enumeration == null)
                _enumeration = new Dictionary<int, string>();
            _enumeration.Add(num, name);
        }

        /// <summary>
        /// String representation of AttributeType object
        /// for debugging purposes.
        /// @return string
        /// @see java.lang.Object#toString()
        /// </summary>
        public override String ToString()
        {
            String s = TypeCode +
                       "/" + Name +
                       ": " + _attributeClass.Name;
            if (VendorId != -1)
                s += " (vendor " + VendorId + ")";
            return s;
        }
    }
}