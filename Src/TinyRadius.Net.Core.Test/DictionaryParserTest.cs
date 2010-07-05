using TinyRadius.Net.Dictionaries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace TinyRadius.Net.Net.Core.Test
{


    /// <summary>
    ///This is a test class for DictionaryParserTest and is intended
    ///to contain all DictionaryParserTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DictionaryParserTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for ParseDictionary
        ///</summary>
        [TestMethod()]
        public void ParseDictionaryTest()
        {
            
            const string DictionaryResource = "TinyRadius.Net.Dictionaries.default_dictionary";
            var stream = typeof(DefaultDictionary).Assembly.GetManifestResourceStream(DictionaryResource);

            IWritableDictionary dictionary = new MemoryDictionary();

            DictionaryParser.ParseDictionary(stream, dictionary);
          
        }
    }
}
