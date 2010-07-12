using System;

namespace TinyRadius.Net.Dictionaries
{
    /**
     * A dictionary that is not read-only. Provides methods
     * to add entries to the dictionary.
     */

    public interface IWritableDictionary : IDictionary
    {
        /**
         * Adds the given vendor to the dictionary.
         * @param vendorId vendor ID
         * @param vendorName name of the vendor
         */
        void AddVendor(int vendorId, String vendorName);

        /**
         * Adds an AttributeType object to the dictionary.
         * @param attributeType AttributeType object
         */
        void AddAttributeType(AttributeType attributeType);
    }
}