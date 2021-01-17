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
    public class ProjectCreateCommandTest
    {
        private IProjectCreateCommand _createProjectCommand;

        private IDbContext _dbContext;
        private IProjectValidator _projectValidator;

        [SetUp]
        public void ProjectCreateCommandTest_SetUp()
        {
            _dbContext = Substitute.For<IDbContext>();
            _projectValidator = Substitute.For<IProjectValidator>();

            _createProjectCommand = new ProjectCreateCommand(_dbContext, _projectValidator);
        }

        [TearDown]
        public void ProjectCreateCommandTest_TearDown()
        {
            // delete all .db files (in case previous tests have failed)
            TestHelper.DeleteTestFiles(AppContext.BaseDirectory, "*.dbtest");

        }

        [Test]
        public void Execute_ValidationFails_ThrowsException()
        {
            ProjectModel model = DataHelper.CreateProjectModel();

            _projectValidator.Validate(Arg.Any<ProjectModel>()).Returns(new ValidationResult("error"));

            // execute
            TestDelegate del = () => _createProjectCommand.Execute(model.Name);
            
            // assert
            Assert.Throws<ValidationException>(del);

            // we shouldn't have even tried to do the insert
            _dbContext.DidNotReceive().ExecuteNonQuery(Arg.Any<string>(), Arg.Any<object>());
        }

        [Test]
        public void Execute_ValidationSucceeds_RecordInserted()
        {
            ProjectModel model = DataHelper.CreateProjectModel();

            _projectValidator.Validate(Arg.Any<ProjectModel>()).Returns(new ValidationResult());

            // execute
            _createProjectCommand.Execute(model.Name);

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
            string projectName = DataHelper.RandomString();

            using (SQLiteDbContext dbContext = new SQLiteDbContext(filePath))
            {
                dbContext.Initialise();
                dbContext.BeginTransaction();

                // create the user
                IProjectRepository projectRepo = new ProjectRepository(dbContext);
                IProjectValidator projectValidator = new ProjectValidator();

                IProjectCreateCommand createProjectCommand = new ProjectCreateCommand(dbContext, projectValidator);
                ProjectModel project = createProjectCommand.Execute(projectName);

                ProjectModel savedProject = projectRepo.GetById(project.Id);

                Assert.IsNotNull(savedProject);
                Assert.AreEqual(project.Name, savedProject.Name);
                Assert.LessOrEqual(startTime, savedProject.CreateDate);
                Assert.GreaterOrEqual(DateTime.UtcNow, savedProject.CreateDate);

            }

        }



    }
}
