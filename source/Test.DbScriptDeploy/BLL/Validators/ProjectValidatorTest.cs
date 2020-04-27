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
    public class ProjectValidatorTest
    {
        private IProjectValidator _projectValidator;

        [SetUp]
        public void ProjectValidatorTest_SetUp()
        {
            _projectValidator = new ProjectValidator();
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("   ")]
        public void Validate_InvalidName_ReturnsFailure(string name)
        {
            ProjectModel model = DataHelper.CreateProjectModel();
            model.Name = name;

            ValidationResult result = _projectValidator.Validate(model);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(1, result.Messages.Count);
            Assert.IsTrue(result.Messages[0].Contains("Project name"));
        }

        [Test]
        public void Validate_AllFieldsValid_ReturnsSuccess()
        {
            ProjectModel model = DataHelper.CreateProjectModel();

            ValidationResult result = _projectValidator.Validate(model);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(0, result.Messages.Count);
        }


    }
}
