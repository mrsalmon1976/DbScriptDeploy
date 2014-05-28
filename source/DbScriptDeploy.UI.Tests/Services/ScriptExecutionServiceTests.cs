using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbScriptDeploy.UI.Events;
using DbScriptDeploy.UI.Models;
using DbScriptDeploy.UI.Services;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using StructureMap;
using SystemWrapper.IO;

namespace DbScriptDeploy.UI.Tests.Services
{
	[TestFixture]
	public class ScriptExecutionServiceTests
	{
        private string _testFilePath;
		private IScriptExecutionService _scriptExecutionService;

        private const string TestFileName = "TestFile.sql";

		#region SetUp and TearDown

		[TestFixtureSetUp]
		public void ScriptExecutionServiceTestsSetUp()
		{
			_testFilePath = Path.Combine(Environment.CurrentDirectory, TestFileName);
            _scriptExecutionService = new ScriptExecutionService();

			// make sure there is no test file!
			if (File.Exists(_testFilePath))
			{
				Assert.Fail("Unable to start test as a test file already exists: {0}", _testFilePath);
			}
		}

		[TestFixtureTearDown]
        public void ScriptExecutionServiceTestsTearDown()
		{

			if (File.Exists(_testFilePath))
			{
				File.Delete(_testFilePath);
			}
		}

		#endregion

        #region LoadScriptFromFile Tests

        [Test]
        public void LoadScriptFromFile_LoadFile_SetsId()
        {
            CreateScriptFile();

            Script script = _scriptExecutionService.LoadScriptFromFile(_testFilePath);

            Assert.IsNotNull(script);
            Assert.AreNotEqual(Guid.Empty, script.Id);
            
        }

        [Test]
        public void LoadScriptFromFile_LoadFile_SetsName()
        {
            CreateScriptFile();

            Script script = _scriptExecutionService.LoadScriptFromFile(_testFilePath);

            Assert.IsNotNull(script);
            Assert.AreEqual(TestFileName, script.Name);

        }

        [Test]
        public void LoadScriptFromFile_LoadFile_SetsScriptText()
        {
            string text = Guid.NewGuid().ToString() + Environment.NewLine + Guid.NewGuid().ToString();
            CreateScriptFile(text);

            Script script = _scriptExecutionService.LoadScriptFromFile(_testFilePath);

            Assert.IsNotNull(script);
            Assert.AreEqual(text, script.ScriptText);

        }

        [TestCase("--asdsd")]
        [TestCase("--asdsd")]
        [TestCase("---asdsd")] // No space - will still fail
        [TestCase("select * from X")]
        public void LoadScriptFromFile_NoTagInFile_NoTagSetOnModel(string line1)
        {
            string text = line1 + Environment.NewLine + Guid.NewGuid().ToString();
            CreateScriptFile(text);

            Script script = _scriptExecutionService.LoadScriptFromFile(_testFilePath);

            Assert.IsNotNull(script);
            Assert.IsNull(script.Tag);

        }

        [TestCase("--- Test", "Test")]
        [TestCase("---     Test       ", "Test")]
        [TestCase("--- This is a long tag!!!", "This is a long tag!!!")]
        public void LoadScriptFromFile_TagInFile_TagSet(string line1, string expectedTag)
        {
            string text = line1 + Environment.NewLine + Guid.NewGuid().ToString();
            CreateScriptFile(text);

            Script script = _scriptExecutionService.LoadScriptFromFile(_testFilePath);

            Assert.IsNotNull(script);
            Assert.AreEqual(expectedTag, script.Tag);

        }

        #endregion

        #region Private Methods

        private void CreateScriptFile(string contents = null)
        {
            using (StreamWriter writer = File.CreateText(_testFilePath))
            {
                writer.Write(contents ?? String.Empty);
            }
        }

        #endregion





    }
}
