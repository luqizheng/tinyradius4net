/**
 * $Id: DefaultDictionary.java,v 1.1 2005/09/04 22:11:00 wuttke Exp $
 * Created on 28.08.2005
 * @author mw
 * @version $Revision: 1.1 $
 */
using System.IO;
using TinyRadius.Net.Directories;
using System;
namespace TinyRadius.Net.Directories
{
/**
 * The default dictionary is a singleton object containing
 * a dictionary in the memory that is filled on application
 * startup using the default dictionary file from the
 * classpath resource
 * <code>TinyRadius.dictionary.default_dictionary</code>.
 */
public class DefaultDictionary: MemoryDictionary
{

	/**
	 * Returns the singleton instance of this object.
	 * @return DefaultDictionary instance
	 */
    public static IWritableDictionary getDefaultDictionary()
    {
		return instance;
	}
	
	/**
	 * Make constructor private so that a DefaultDictionary
	 * cannot be constructed by other classes. 
	 */
	private DefaultDictionary() 
    {
        /*try 
        {
    		InputStream source = DefaultDictionary.class.getClassLoader().getResourceAsStream(DICTIONARY_RESOURCE);

			DictionaryParser .parseDictionary(source, instance);
		} catch (IOException e) {
			throw new RuntimeException("default dictionary unavailable", e);
		}*/
	}
	
	private const String DICTIONARY_RESOURCE = "tinyradius/dictionary/default_dictionary";
	
	/**
	 * Creates the singleton instance of this object
	 * and parses the classpath ressource.
	 */
    public static readonly DefaultDictionary Instance = new DefaultDictionary();
    
}

    

	
}
