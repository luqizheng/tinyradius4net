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
            const string dictionaryResource = "TinyRadius.Net.Dictionaries.default_dictionary";
            var stream = typeof(DefaultDictionary).Assembly.GetManifestResourceStream(dictionaryResource);
            DictionaryParser.ParseDictionary(stream, this);
        }



        /**
         * Creates the singleton instance of this object
         * and parses the classpath ressource.
         */
        public static readonly DefaultDictionary Instance = new DefaultDictionary();

    }




}
