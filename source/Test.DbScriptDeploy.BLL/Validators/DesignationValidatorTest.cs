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
    public class TSqlExecutionServiceTest
    {
        private IDesignationValidator _designationValidator;

        [SetUp]
        public void DesignationValidatorTest_SetUp()
        {
            _designationValidator = new DesignationValidator();
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("   ")]
        public void Validate_InvalidName_ReturnsFailure(string name)
        {
            DesignationModel model = DataHelper.CreateDesignationModel();
            model.Name = name;

            ValidationResult result = _designationValidator.Validate(model);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(1, result.Messages.Count);
            Assert.IsTrue(result.Messages[0].Contains("Designation name"));
        }

        [Test]
        public void Validate_AllFieldsValid_ReturnsSuccess()
        {
            DesignationModel model = DataHelper.CreateDesignationModel();

            ValidationResult result = _designationValidator.Validate(model);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(0, result.Messages.Count);
        }


    }
}
