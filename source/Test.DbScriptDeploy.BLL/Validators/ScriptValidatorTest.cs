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
    public class ScriptValidatorTest
    {
        private IScriptValidator _scriptValidator;

        [SetUp]
        public void EnvironmentValidatorTest_SetUp()
        {
            _scriptValidator = new ScriptValidator();
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("   ")]
        public void Validate_InvalidName_ReturnsFailure(string name)
        {
            ScriptModel model = DataHelper.CreateScriptModel();
            model.Name = name;

            ValidationResult result = _scriptValidator.Validate(model);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(1, result.Messages.Count);
            Assert.IsTrue(result.Messages[0].Contains("Name"));
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("   ")]
        public void Validate_InvalidScriptUp_ReturnsFailure(string scriptUp)
        {
            ScriptModel model = DataHelper.CreateScriptModel();
            model.ScriptUp = scriptUp;

            ValidationResult result = _scriptValidator.Validate(model);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(1, result.Messages.Count);
            Assert.IsTrue(result.Messages[0].Contains("Up script"));
        }


        [Test]
        public void Validate_InvalidProjectId_ReturnsFailure()
        {
            ScriptModel model = DataHelper.CreateScriptModel();
            model.ProjectId = 0;

            ValidationResult result = _scriptValidator.Validate(model);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(1, result.Messages.Count);
            Assert.IsTrue(result.Messages[0].Contains("Project must"));
        }

        [Test]
        public void Validate_AllFieldsValid_ReturnsSuccess()
        {
            ScriptModel model = DataHelper.CreateScriptModel();

            ValidationResult result = _scriptValidator.Validate(model);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(0, result.Messages.Count);
        }


    }
}
