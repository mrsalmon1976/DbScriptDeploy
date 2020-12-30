using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.BLL.Validators;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.DbScriptDeploy.BLL.Validators
{
    [TestFixture]
    public class EnvironmentValidatorTest
    {
        private IEnvironmentValidator _environmentValidator;

        [SetUp]
        public void EnvironmentValidatorTest_SetUp()
        {
            _environmentValidator = new EnvironmentValidator();
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("   ")]
        public void Validate_InvalidHostName_ReturnsFailure(string hostName)
        {
            EnvironmentModel model = DataHelper.CreateEnvironmentModel();
            model.HostName = hostName;

            ValidationResult result = _environmentValidator.Validate(model);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(1, result.Messages.Count);
            Assert.IsTrue(result.Messages[0].Contains("Host name"));
        }

        [Test]
        public void Validate_InvalidDbType_ReturnsFailure()
        {
            EnvironmentModel model = DataHelper.CreateEnvironmentModel();
            model.DbType = Lookups.DatabaseType.None;

            ValidationResult result = _environmentValidator.Validate(model);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(1, result.Messages.Count);
            Assert.IsTrue(result.Messages[0].Contains("Database type"));
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("   ")]
        public void Validate_InvalidDbName_ReturnsFailure(string dbName)
        {
            EnvironmentModel model = DataHelper.CreateEnvironmentModel();
            model.DbName = dbName;

            ValidationResult result = _environmentValidator.Validate(model);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(1, result.Messages.Count);
            Assert.IsTrue(result.Messages[0].Contains("Database name"));
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-1000)]
        public void Validate_InvalidPort_ReturnsFailure(int port)
        {
            EnvironmentModel model = DataHelper.CreateEnvironmentModel();
            model.Port = port;

            ValidationResult result = _environmentValidator.Validate(model);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(1, result.Messages.Count);
            Assert.IsTrue(result.Messages[0].Contains("Port number"));
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("   ")]
        public void Validate_InvalidUserName_ReturnsFailure(string userName)
        {
            EnvironmentModel model = DataHelper.CreateEnvironmentModel();
            model.UserName = userName;

            ValidationResult result = _environmentValidator.Validate(model);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(1, result.Messages.Count);
            Assert.IsTrue(result.Messages[0].Contains("User name"));
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-1000)]
        public void Validate_InvalidDisplay_ReturnsFailure(int displayOrder)
        {
            EnvironmentModel model = DataHelper.CreateEnvironmentModel();
            model.DisplayOrder = displayOrder;

            ValidationResult result = _environmentValidator.Validate(model);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(1, result.Messages.Count);
            Assert.IsTrue(result.Messages[0].Contains("Display order"));
        }

        [Test]
        public void Validate_InvalidProjectId_ReturnsFailure()
        {
            EnvironmentModel model = DataHelper.CreateEnvironmentModel();
            model.ProjectId = Guid.Empty;

            ValidationResult result = _environmentValidator.Validate(model);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(1, result.Messages.Count);
            Assert.IsTrue(result.Messages[0].Contains("Project must"));
        }

        [Test]
        public void Validate_AllFieldsValid_ReturnsSuccess()
        {
            EnvironmentModel model = DataHelper.CreateEnvironmentModel();

            ValidationResult result = _environmentValidator.Validate(model);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(0, result.Messages.Count);
        }


    }
}
