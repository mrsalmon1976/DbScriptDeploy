using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.BLL.Commands;
using DbScriptDeploy.BLL.Validators;
using DbScriptDeploy.BLL.Security;
using DbScriptDeploy.BLL.Models;

namespace Test.DbScriptDeploy.BLL.Repositories
{
    [TestFixture]
    public class ProjectRepositoryTest
    {
        private IDbContext _dbContext;


        [SetUp]
        public void ProjectRepositoryTest_SetUp()
        {
            _dbContext = Substitute.For<IDbContext>();
        }

        [TearDown]
        public void ProjectRepositoryTest_TearDown()
        {
            // delete all .db files (in case previous tests have failed)
            TestHelper.DeleteTestFiles(AppContext.BaseDirectory, "*.dbtest");

        }


        #region GetById 

        /// <summary>
        /// Tests that the GetById method fetches a project by its id
        /// </summary>
        [Test]
        public void GetById_Integration_ReturnsData()
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName() + ".dbtest");
            string name = Path.GetRandomFileName();

            using (SQLiteDbContext dbContext = new SQLiteDbContext(filePath))
            {
                dbContext.Initialise();
                dbContext.BeginTransaction();

                IProjectRepository projectRepo = new ProjectRepository(dbContext);

                IProjectCreateCommand createProjectCommand = new ProjectCreateCommand(dbContext, new ProjectValidator());

                // create the user
                ProjectModel project = createProjectCommand.Execute(name);

                ProjectModel result = projectRepo.GetById(project.Id);
                Assert.IsNotNull(result);
                Assert.AreEqual(name, result.Name);
            }

        }

        #endregion

        #region GetAll

          /// <summary>
        /// Tests that the GetAll method gets all projects, ordered aphabetically
        /// </summary>
        [TestCase]
        public void GetAll_Integration_ReturnsData()
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName() + ".dbtest");
            string name = Path.GetRandomFileName();

            using (SQLiteDbContext dbContext = new SQLiteDbContext(filePath))
            {
                dbContext.Initialise();
                dbContext.BeginTransaction();

                IProjectRepository projectRepo = new ProjectRepository(dbContext);
                IProjectCreateCommand createProjectCommand = new ProjectCreateCommand(dbContext, new ProjectValidator());

                // create the projects
                ProjectModel project_z = createProjectCommand.Execute("zzzzzzz");
                ProjectModel project_t = createProjectCommand.Execute("tttttt");
                ProjectModel project_T = createProjectCommand.Execute("TTTTTTT");
                ProjectModel project_Z = createProjectCommand.Execute("ZZZZZZZ");
                ProjectModel project_a = createProjectCommand.Execute("aaaaaaa");

                List<ProjectModel> result = projectRepo.GetAll().ToList();

                Assert.AreEqual(5, result.Count);
                Assert.AreEqual(project_a.Id, result[0].Id);
                Assert.AreEqual(project_t.Id, result[1].Id);
                Assert.AreEqual(project_T.Id, result[2].Id);
                Assert.AreEqual(project_z.Id, result[3].Id);
                Assert.AreEqual(project_Z.Id, result[4].Id);
            }

        }

        #endregion
        


    }
}
