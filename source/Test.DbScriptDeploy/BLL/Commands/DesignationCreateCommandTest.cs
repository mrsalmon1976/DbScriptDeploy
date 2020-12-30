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
    public class DesignationCreateCommandTest
    {
        private IDesignationCreateCommand _createDesignationCommand;

        private IDbContext _dbContext;
        private IDesignationValidator _designationValidator;

        [SetUp]
        public void DesignationCreateCommandTest_SetUp()
        {
            _dbContext = Substitute.For<IDbContext>();
            _designationValidator = Substitute.For<IDesignationValidator>();

            _createDesignationCommand = new DesignationCreateCommand(_dbContext, _designationValidator);
        }

        [TearDown]
        public void DesignationCreateCommandTest_TearDown()
        {
            // delete all .db files (in case previous tests have failed)
            TestHelper.DeleteTestFiles(AppContext.BaseDirectory, "*.dbtest");

        }

        [Test]
        public void Execute_ValidationFails_ThrowsException()
        {
            DesignationModel model = DataHelper.CreateDesignationModel();

            _designationValidator.Validate(Arg.Any<DesignationModel>()).Returns(new ValidationResult("error"));

            // execute
            TestDelegate del = () => _createDesignationCommand.Execute(model.Name);
            
            // assert
            Assert.Throws<ValidationException>(del);

            // we shouldn't have even tried to do the insert
            _dbContext.DidNotReceive().ExecuteNonQuery(Arg.Any<string>(), Arg.Any<object>());
        }

        [Test]
        public void Execute_ValidationSucceeds_RecordInserted()
        {
            DesignationModel model = DataHelper.CreateDesignationModel();

            _designationValidator.Validate(Arg.Any<DesignationModel>()).Returns(new ValidationResult());

            // execute
            _createDesignationCommand.Execute(model.Name);

            // assert
            _dbContext.Received(1).ExecuteNonQuery(Arg.Any<string>(), Arg.Any<object>());
        }


        /// <summary>
        /// Tests that the insert actually works
        /// </summary>
        [Test]
        public void Execute_IntegrationTest_SQLite()
        {
            DateTime startTime = DateTime.UtcNow.AddSeconds(-1);
            string filePath = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName() + ".dbtest");
            string DesignationName = DataHelper.RandomString();

            using (SQLiteDbContext dbContext = new SQLiteDbContext(filePath))
            {
                dbContext.Initialise();
                dbContext.BeginTransaction();

                // create the user
                ILookupRepository lookupRepo = new LookupRepository(dbContext);
                IDesignationValidator DesignationValidator = new DesignationValidator();

                IDesignationCreateCommand createDesignationCommand = new DesignationCreateCommand(dbContext, DesignationValidator);
                DesignationModel designation = createDesignationCommand.Execute(DesignationName);

                DesignationModel savedDesignation = lookupRepo.GetAllDesignations().FirstOrDefault(x => x.Id == designation.Id);

                Assert.IsNotNull(savedDesignation);
                Assert.AreEqual(designation.Name, savedDesignation.Name);
                Assert.LessOrEqual(startTime, savedDesignation.CreateDate);
                Assert.GreaterOrEqual(DateTime.UtcNow, savedDesignation.CreateDate);

            }

        }



    }
}
