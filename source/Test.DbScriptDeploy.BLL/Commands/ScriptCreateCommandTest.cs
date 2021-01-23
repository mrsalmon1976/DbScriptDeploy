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
    public class ScriptCreateCommandTest
    {
        private IScriptCreateCommand _createScriptCommand;

        private IDbContext _dbContext;
        private IScriptValidator _scriptValidator;

        [SetUp]
        public void ScriptCreateCommandTest_SetUp()
        {
            _dbContext = Substitute.For<IDbContext>();
            _scriptValidator = Substitute.For<IScriptValidator>();

            _createScriptCommand = new ScriptCreateCommand(_dbContext, _scriptValidator);
        }

        [TearDown]
        public void ScriptCreateCommandTest_TearDown()
        {
            // delete all .db files (in case previous tests have failed)
            TestHelper.DeleteTestFiles(AppContext.BaseDirectory, "*.dbtest");

        }

        [Test]
        public void Execute_ValidationFails_ThrowsException()
        {
            ScriptModel model = DataHelper.CreateScriptModel();

            _scriptValidator.Validate(Arg.Any<ScriptModel>()).Returns(new ValidationResult("error"));

            // execute
            TestDelegate del = () => _createScriptCommand.Execute(model);
            
            // assert
            Assert.Throws<ValidationException>(del);

            // we shouldn't have even tried to do the insert
            _dbContext.DidNotReceive().ExecuteScalar<int>(Arg.Any<string>(), Arg.Any<object>());
            _dbContext.DidNotReceive().ExecuteNonQuery(Arg.Any<string>(), Arg.Any<object>());
        }

        [Test]
        public void Execute_ValidationSucceeds_ScriptRecordInserted()
        {
            ScriptModel model = DataHelper.CreateScriptModel();

            _scriptValidator.Validate(Arg.Any<ScriptModel>()).Returns(new ValidationResult());

            // execute
            _createScriptCommand.Execute(model);

            // assert
            _dbContext.Received(1).ExecuteScalar<int>(Arg.Any<string>(), Arg.Any<object>());
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(7)]
        public void Execute_ValidationSucceeds_ScriptTagRecordsInserted(int tagCount)
        {
            List<string> tags = new List<string>();
            for (int i=0; i<tagCount; i++)
            {
                tags.Add(Guid.NewGuid().ToString());
            }
            ScriptModel model = DataHelper.CreateScriptModel();
            model.Tags = tags.ToArray();

            _scriptValidator.Validate(Arg.Any<ScriptModel>()).Returns(new ValidationResult());

            // execute
            _createScriptCommand.Execute(model);

            // assert
            _dbContext.Received(tagCount).ExecuteNonQuery(Arg.Any<string>(), Arg.Any<object>());
        }

        /// <summary>
        /// Tests that the insert actually works
        /// </summary>
        [Test]
        public void Execute_IntegrationTest_SQLite()
        {
            DateTime startTime = DateTime.UtcNow.AddSeconds(-1);
            string filePath = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName() + ".dbtest");
            ScriptModel model = DataHelper.CreateScriptModel();
            model.Tags = new string[] { "Tag1", "Tag3", "Tag2" };
     

            using (SQLiteDbContext dbContext = new SQLiteDbContext(filePath))
            {
                dbContext.Initialise();
                dbContext.BeginTransaction();

                // create the script
                IScriptRepository scriptRepo = new ScriptRepository(dbContext);
                IScriptTagRepository scriptTagRepo = new ScriptTagRepository(dbContext);
                IScriptValidator scriptValidator = new ScriptValidator();

                IScriptCreateCommand createScriptCommand = new ScriptCreateCommand(dbContext, scriptValidator);
                ScriptModel script = createScriptCommand.Execute(model);

                // load the script and make sure it saved correctly
                ScriptModel savedScript = scriptRepo.GetById(script.Id);

                Assert.IsNotNull(savedScript);
                Assert.AreEqual(script.Name, savedScript.Name);
                Assert.AreEqual(script.ProjectId, savedScript.ProjectId);
                Assert.AreEqual(script.ScriptUp, savedScript.ScriptUp);
                Assert.AreEqual(script.ScriptDown, savedScript.ScriptDown);
                Assert.LessOrEqual(startTime, savedScript.CreateDate);
                Assert.GreaterOrEqual(DateTime.UtcNow, savedScript.CreateDate);

                List<ScriptTagModel> scriptTags = scriptTagRepo.GetByScriptId(script.Id).ToList();
                Assert.AreEqual(3, scriptTags.Count);
                Assert.AreEqual("Tag1", scriptTags[0].Tag);
                Assert.AreEqual("Tag2", scriptTags[1].Tag);
                Assert.AreEqual("Tag3", scriptTags[2].Tag);

            }

        }



    }
}
