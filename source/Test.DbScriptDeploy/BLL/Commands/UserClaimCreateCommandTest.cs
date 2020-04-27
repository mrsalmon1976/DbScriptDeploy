using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using DbScriptDeploy.BLL.Commands;
using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Validators;
using DbScriptDeploy.BLL.Security;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Exceptions;
using DbScriptDeploy.BLL.Repositories;

namespace Test.DbScriptDeploy.BLL.Commands
{
    [TestFixture]
    public class UserClaimCreateCommandTest
    {
        private IUserClaimCreateCommand _createUserClaimCommand;

        private IDbContext _dbContext;
        private IUserClaimValidator _userClaimValidator;

        [SetUp]
        public void UserClaimCreateCommandTest_SetUp()
        {
            _dbContext = Substitute.For<IDbContext>();
            _userClaimValidator = Substitute.For<IUserClaimValidator>();

            _createUserClaimCommand = new UserClaimCreateCommand(_dbContext, _userClaimValidator);
        }

        [TearDown]
        public void UserClaimCreateCommandTest_TearDown()
        {
            // delete all .db files (in case previous tests have failed)
            TestHelper.DeleteTestFiles(AppContext.BaseDirectory, "*.dbtest");

        }

        [Test]
        public void Execute_ValidationFails_ThrowsException()
        {
            UserClaimModel model = DataHelper.CreateUserClaimModel();

            _userClaimValidator.Validate(Arg.Any<UserClaimModel>()).Returns(new ValidationResult("error"));

            // execute
            TestDelegate del = () => _createUserClaimCommand.Execute(model.UserId, model.Name, model.ProjectId);
            
            // assert
            Assert.Throws<ValidationException>(del);

            // we shouldn't have even tried to do the insert
            _dbContext.DidNotReceive().ExecuteNonQuery(Arg.Any<string>(), Arg.Any<object>());
        }

        [Test]
        public void Execute_ValidationSucceeds_RecordInserted()
        {
            UserClaimModel model = DataHelper.CreateUserClaimModel();

            _userClaimValidator.Validate(Arg.Any<UserClaimModel>()).Returns(new ValidationResult());

            // execute
            _createUserClaimCommand.Execute(model.UserId, model.Name, model.ProjectId);

            // assert
            _dbContext.Received(1).ExecuteNonQuery(Arg.Any<string>(), Arg.Any<object>());
        }


        /// <summary>
        /// Tests that the insert actually works
        /// </summary>
        [Test]
        public void Execute_IntegrationTest_SQLite()
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName() + ".dbtest");
            using (SQLiteDbContext dbContext = new SQLiteDbContext(filePath))
            {
                dbContext.Initialise();
                dbContext.BeginTransaction();

                // create the user
                UserClaimModel claim = DataHelper.CreateUserClaimModel(Guid.NewGuid(), DataHelper.RandomString(), Guid.NewGuid());

                IUserClaimRepository userClaimRepo = new UserClaimRepository(dbContext);
                IUserClaimValidator userClaimValidator = new UserClaimValidator(userClaimRepo);

                IUserClaimCreateCommand createUserClaimCommand = new UserClaimCreateCommand(dbContext, userClaimValidator);
                createUserClaimCommand.Execute(claim.UserId, claim.Name, claim.ProjectId);

                UserClaimModel savedClaim = userClaimRepo.GetByUserId(claim.UserId).FirstOrDefault(x => x.ProjectId == claim.ProjectId);

                Assert.IsNotNull(savedClaim);
                Assert.AreEqual(claim.UserId, savedClaim.UserId);
                Assert.AreEqual(claim.Name, savedClaim.Name);
                Assert.AreEqual(claim.ProjectId, savedClaim.ProjectId);
            }

        }



    }
}
