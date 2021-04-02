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
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Exceptions;
using DbScriptDeploy.BLL.Repositories;

namespace Test.DbScriptDeploy.BLL.Commands
{
    [TestFixture]
    public class ScriptExecutionCreateCommandTest
    {
        private IScriptExecutionCreateCommand _createScriptExecutionCommand;

        private IDbContext _dbContext;
        private IScriptExecutionValidator _scriptExecutionValidator;

        [SetUp]
        public void ScriptExecutionCreateCommandTest_SetUp()
        {
            _dbContext = Substitute.For<IDbContext>();
            _scriptExecutionValidator = Substitute.For<IScriptExecutionValidator>();

            _createScriptExecutionCommand = new ScriptExecutionCreateCommand(_dbContext, _scriptExecutionValidator);
        }

        [TearDown]
        public void ScriptExecutionCreateCommandTest_TearDown()
        {
            // delete all .db files (in case previous tests have failed)
            TestHelper.DeleteTestFiles(AppContext.BaseDirectory, "*.dbtest");

        }

        [Test]
        public void Execute_ValidationFails_ThrowsException()
        {
            ScriptExecutionModel model = DataHelper.CreateScriptExecutionModel();

            _scriptExecutionValidator.Validate(Arg.Any<ScriptExecutionModel>()).Returns(new ValidationResult("error"));

            // execute
            TestDelegate del = () => _createScriptExecutionCommand.Execute(model);
            
            // assert
            Assert.Throws<ValidationException>(del);

            // we shouldn't have even tried to do the insert
            _dbContext.DidNotReceive().ExecuteScalar<int>(Arg.Any<string>(), Arg.Any<object>());
            _dbContext.DidNotReceive().ExecuteNonQuery(Arg.Any<string>(), Arg.Any<object>());
        }

        [Test]
        public void Execute_ValidationSucceeds_ScriptExecutionRecordInserted()
        {
            ScriptExecutionModel model = DataHelper.CreateScriptExecutionModel();

            _scriptExecutionValidator.Validate(Arg.Any<ScriptExecutionModel>()).Returns(new ValidationResult());

            // execute
            _createScriptExecutionCommand.Execute(model);

            // assert
            _dbContext.Received(1).ExecuteScalar<int>(Arg.Any<string>(), Arg.Any<object>());
        }

        /// <summary>
        /// Tests that the insert actually works
        /// </summary>
        [Test]
        public void Execute_IntegrationTest_SQLite()
        {
            DateTime startTime = DateTime.UtcNow.AddSeconds(-1);
            string filePath = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName() + ".dbtest");
            ScriptExecutionModel model = DataHelper.CreateScriptExecutionModel();
     

            using (SQLiteDbContext dbContext = new SQLiteDbContext(filePath))
            {
                dbContext.Initialise();
                dbContext.BeginTransaction();

                // create the script
                IScriptExecutionRepository scriptExecutionRepo = new ScriptExecutionRepository(dbContext);
                IScriptExecutionValidator scriptExecutionValidator = new ScriptExecutionValidator();

                IScriptExecutionCreateCommand createScriptExecutionCommand = new ScriptExecutionCreateCommand(dbContext, scriptExecutionValidator);
                ScriptExecutionModel script = createScriptExecutionCommand.Execute(model);

                // load the script and make sure it saved correctly
                ScriptExecutionModel savedModel = scriptExecutionRepo.GetById(script.Id);

                Assert.IsNotNull(savedModel);
                Assert.AreEqual(script.ScriptId, savedModel.ScriptId);
                Assert.AreEqual(script.EnvironmentId, savedModel.EnvironmentId);
            }

        }



    }
}
