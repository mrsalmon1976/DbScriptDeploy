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
    public class ScriptExecutionValidatorTest
    {
        private IScriptExecutionValidator _scriptExecutionValidator;

        [SetUp]
        public void ScriptExecutionValidatorTest_SetUp()
        {
            _scriptExecutionValidator = new ScriptExecutionValidator();
        }

        [TestCase(-100)]
        [TestCase(-1)]
        [TestCase(0)]
        public void Validate_InvalidScriptId_ReturnsFailure(int scriptId)
        {
            ScriptExecutionModel model = DataHelper.CreateScriptExecutionModel(scriptId: scriptId);

            ValidationResult result = _scriptExecutionValidator.Validate(model);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(1, result.Messages.Count);
            Assert.IsTrue(result.Messages[0].Contains("Script reference"));
        }

        [TestCase(-100)]
        [TestCase(-1)]
        [TestCase(0)]
        public void Validate_InvalidEnvironmentId_ReturnsFailure(int environmentId)
        {
            ScriptExecutionModel model = DataHelper.CreateScriptExecutionModel();
            model.EnvironmentId = environmentId;

            ValidationResult result = _scriptExecutionValidator.Validate(model);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(1, result.Messages.Count);
            Assert.IsTrue(result.Messages[0].Contains("Environment reference"));
        }



        [Test]
        public void Validate_CompletetionDateBeforeStartDate_ReturnsFailure()
        {
            ScriptExecutionModel model = DataHelper.CreateScriptExecutionModel();
            model.ExecutionStartDate = DateTime.UtcNow;
            model.ExecutionCompleteDate = model.ExecutionCompleteDate.AddMilliseconds(-1);

            ValidationResult result = _scriptExecutionValidator.Validate(model);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(1, result.Messages.Count);
            Assert.IsTrue(result.Messages[0].Contains("Execution completion date cannot be before execution start date"));
        }

        [Test]
        public void Validate_AllFieldsValid_ReturnsSuccess()
        {
            ScriptExecutionModel model = DataHelper.CreateScriptExecutionModel();

            ValidationResult result = _scriptExecutionValidator.Validate(model);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(0, result.Messages.Count);
        }


    }
}
