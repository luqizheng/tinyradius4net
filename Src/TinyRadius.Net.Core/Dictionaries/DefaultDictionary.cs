using System.IO;

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


        /**
         * Creates the singleton instance of this object
         * and parses the classpath ressource.
         */
        public static readonly DefaultDictionary Instance = new DefaultDictionary();

        private DefaultDictionary()
        {
            const string dictionaryResource = "TinyRadius.Net.Dictionaries.default_dictionary";
            Stream stream = typeof (DefaultDictionary).Assembly.GetManifestResourceStream(dictionaryResource);
            DictionaryParser.ParseDictionary(stream, this);
        }

        public static IWritableDictionary GetDefaultDictionary()
        {
            return Instance;
        }
    }
}