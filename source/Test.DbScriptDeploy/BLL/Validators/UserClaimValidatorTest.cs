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
    public class UserClaimValidatorTest
    {
        private IUserClaimValidator _userClaimValidator;
        private IUserClaimRepository _userClaimRepo;

        [SetUp]
        public void UserValidatorTest_SetUp()
        {
            _userClaimRepo = Substitute.For<IUserClaimRepository>();

            _userClaimValidator = new UserClaimValidator(_userClaimRepo);
        }

        [Test]
        public void Validate_InvalidUserId_ReturnsFailure()
        {
            UserClaimModel model = DataHelper.CreateUserClaimModel();
            model.UserId = Guid.Empty;

            ValidationResult result = _userClaimValidator.Validate(model);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(1, result.Messages.Count);
            Assert.IsTrue(result.Messages[0].Contains("User id"));
        }


        [TestCase("")]
        [TestCase(null)]
        [TestCase("   ")]
        public void Validate_InvalidClaimName_ReturnsFailure(string name)
        {
            UserClaimModel model = DataHelper.CreateUserClaimModel();
            model.Name = name;

            ValidationResult result = _userClaimValidator.Validate(model);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(1, result.Messages.Count);
            Assert.IsTrue(result.Messages[0].Contains("Claim name"));
        }

        [Test]
        public void Validate_UserClaimAlreadyExists_ReturnsFailure()
        {
            UserClaimModel model = DataHelper.CreateUserClaimModel(projectId: Guid.NewGuid());

            List<UserClaimModel> existingClaims = new List<UserClaimModel>();
            existingClaims.Add(new UserClaimModel() { Name = model.Name, ProjectId = model.ProjectId });
            _userClaimRepo.GetByUserId(model.UserId).Returns(existingClaims);

            ValidationResult result = _userClaimValidator.Validate(model);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(1, result.Messages.Count);
            Assert.IsTrue(result.Messages[0].Contains("already assigned"));
        }

        [Test]
        public void Validate_AllFieldsValid_ReturnsSuccess()
        {
            UserClaimModel model = DataHelper.CreateUserClaimModel();

            ValidationResult result = _userClaimValidator.Validate(model);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(0, result.Messages.Count);
        }


    }
}
