using System;

namespace TinyRadius.Net.Dictionaries
{
    /**
     * The default dictionary is a singleton object containing
     * a dictionary in the memory that is filled on application
     * startup using the default dictionary file from the
     * classpath resource
     * <code>TinyRadius.dictionary.default_dictionary</code>.
     */
    public class DefaultDictionary : MemoryDictionary
    {

        /**
         * Returns the singleton instance of this object.
         * @return DefaultDictionary instance
         */
        public static IWritableDictionary GetDefaultDictionary()
        {
            return Instance;
        }

        /**
         * Make constructor private so that a DefaultDictionary
         * cannot be constructed by other classes. 
         */
        private DefaultDictionary()
        {
            try
            {
                var stream = typeof(DefaultDictionary).Assembly.GetManifestResourceStream(DictionaryResource);

                DictionaryParser.ParseDictionary(stream, Instance);
            }
            catch (Exception e)
            {
                throw new NotImplementedException("default dictionary unavailable", e);
            }
        }

        private const string DictionaryResource = "TinyRadius.Net/Dictionaries/default_dictionary";

        /**
         * Creates the singleton instance of this object
         * and parses the classpath ressource.
         */
        public static readonly DefaultDictionary Instance = new DefaultDictionary();

    }




}
