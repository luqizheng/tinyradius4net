using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyRadius.Net.Cfg;

namespace TinyRadius.Net.Net.Core.Test
{
    /// <summary>
    ///This is a test class for ConfigTest and is intended
    ///to contain all ConfigTest Unit Tests
    ///</summary>
    [TestClass]
    public class ConfigTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

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
        ///A test for ValidateByLdap
        ///</summary>
        [TestMethod]
        public void ValidateByLdapTest()
        {
            string path = string.Empty; // TODO: Initialize to an appropriate value
            var target = new Config(path); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.ValidateByLdap = expected;
            actual = target.ValidateByLdap;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ValidateByDatabase
        ///</summary>
        [TestMethod]
        public void ValidateByDatabaseTest()
        {
            string path = string.Empty; // TODO: Initialize to an appropriate value
            var target = new Config(path); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.ValidateByDatabase = expected;
            actual = target.ValidateByDatabase;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for NasSettings
        ///</summary>
        [TestMethod]
        public void NasSettingsTest()
        {
            string path = "NotExist.xml";
            var target = new Config(path);
            target.NasSettings = new Dictionary<string, NasSetting>();
            target.NasSettings.Add("192.168.1.1", new NasSetting() { Ip = IPAddress.Parse("192.168.1.1"),SecretKey= "123" }); ;
            var pint = new IPEndPoint(IPAddress.Parse("192.168.1.1"), 7000);
            Assert.AreEqual("123", target.NasSettings[pint.Address.ToString()]);
        }

        /// <summary>
        ///A test for LdapSetting
        ///</summary>
        [TestMethod]
        public void LdapSettingTest()
        {
            string path = string.Empty; // TODO: Initialize to an appropriate value
            var target = new Config(path); // TODO: Initialize to an appropriate value
            LdapSetting expected = null; // TODO: Initialize to an appropriate value
            LdapSetting actual;
            target.LdapSetting = expected;
            actual = target.LdapSetting;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for EnableAuthentication
        ///</summary>
        [TestMethod]
        public void EnableAuthenticationTest()
        {
            string path = string.Empty; // TODO: Initialize to an appropriate value
            var target = new Config(path); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.EnableAuthentication = expected;
            actual = target.EnableAuthentication;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for EnableAccount
        ///</summary>
        [TestMethod]
        public void EnableAccountTest()
        {
            string path = string.Empty; // TODO: Initialize to an appropriate value
            var target = new Config(path); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.EnableAccount = expected;
            actual = target.EnableAccount;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DatabaseSetting
        ///</summary>
        [TestMethod]
        public void DatabaseSettingTest()
        {
            string path = string.Empty; // TODO: Initialize to an appropriate value
            var target = new Config(path); // TODO: Initialize to an appropriate value
            DatabaseSetting expected = null; // TODO: Initialize to an appropriate value
            DatabaseSetting actual;
            target.DatabaseSetting = expected;
            actual = target.DatabaseSetting;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for AuthPort
        ///</summary>
        [TestMethod]
        public void AuthPortTest()
        {
            string path = string.Empty; // TODO: Initialize to an appropriate value
            var target = new Config(path); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.AuthPort = expected;
            actual = target.AuthPort;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for AuthListentIp
        ///</summary>
        [TestMethod]
        public void AuthListentIpTest()
        {
            string path = string.Empty; // TODO: Initialize to an appropriate value
            var target = new Config(path); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.AuthListentIp = expected;
            actual = target.AuthListentIp;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for AcctPort
        ///</summary>
        [TestMethod]
        public void AcctPortTest()
        {
            string path = string.Empty; // TODO: Initialize to an appropriate value
            var target = new Config(path); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.AcctPort = expected;
            actual = target.AcctPort;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for AccountListentIp
        ///</summary>
        [TestMethod]
        public void AccountListentIpTest()
        {
            string path = string.Empty; // TODO: Initialize to an appropriate value
            var target = new Config(path); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.AccountListentIp = expected;
            actual = target.AccountListentIp;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Save
        ///</summary>
        [TestMethod]
        public void SaveTest()
        {
            string path = string.Empty; // TODO: Initialize to an appropriate value
            var target = new Config(path); // TODO: Initialize to an appropriate value
            target.Save();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for InitBy
        ///</summary>
        [TestMethod]
        [DeploymentItem("TinyRadius.Net.Core.dll")]
        public void InitByTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            var target = new Config_Accessor(param0); // TODO: Initialize to an appropriate value
            Config config = null; // TODO: Initialize to an appropriate value
            target.InitBy(config);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Config Constructor
        ///</summary>
        [TestMethod]
        public void ConfigConstructorTest()
        {
            string path = string.Empty; // TODO: Initialize to an appropriate value
            var target = new Config(path);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}