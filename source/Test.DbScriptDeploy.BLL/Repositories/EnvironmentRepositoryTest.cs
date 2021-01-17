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
    public class EnvironmentRepositoryTest
    {
        private IDbContext _dbContext;


        [SetUp]
        public void EnvironmentRepositoryTest_SetUp()
        {
            _dbContext = Substitute.For<IDbContext>();
        }

        [TearDown]
        public void EnvironmentRepositoryTest_TearDown()
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
            EnvironmentModel model = DataHelper.CreateEnvironmentModel();
            model.Name = Path.GetRandomFileName();

            using (SQLiteDbContext dbContext = new SQLiteDbContext(filePath))
            {
                dbContext.Initialise();
                dbContext.BeginTransaction();

                IEnvironmentRepository environmentRepo = new EnvironmentRepository(dbContext);

                IEnvironmentCreateCommand createEnvironmentCommand = new EnvironmentCreateCommand(dbContext, new EnvironmentValidator(), new AESGCM());

                // create the user
                EnvironmentModel environment = createEnvironmentCommand.Execute(model);

                EnvironmentModel result = environmentRepo.GetById(environment.Id);
                Assert.IsNotNull(result);
                Assert.AreEqual(model.Name, result.Name);
            }

        }

        #endregion

        #region GetAllByProjectId 

        /// <summary>
        /// Tests that the GetAllByProjectId method only fetches the appropriate environments 
        /// </summary>
        [Test]
        public void GetAllByProjectId_Integration_ReturnsData()
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName() + ".dbtest");
            string name = Path.GetRandomFileName();

            using (SQLiteDbContext dbContext = new SQLiteDbContext(filePath))
            {
                dbContext.Initialise();
                dbContext.BeginTransaction();

                IEnvironmentRepository environmentRepo = new EnvironmentRepository(dbContext);
                IUserRepository userRepo = new UserRepository(dbContext);
                IUserClaimRepository userClaimRepo = new UserClaimRepository(dbContext);

                IProjectCreateCommand createProjectCommand = new ProjectCreateCommand(dbContext, new ProjectValidator());
                IEnvironmentCreateCommand createEnvironmentCommand = new EnvironmentCreateCommand(dbContext, new EnvironmentValidator(), new AESGCM());

                // create the projects
                ProjectModel projectNotExpected = createProjectCommand.Execute("projectNotExpected");
                ProjectModel project1 = createProjectCommand.Execute("project1");

                //create the environments
                EnvironmentModel environmentNotExpected = DataHelper.CreateEnvironmentModel(projectNotExpected.Id);
                EnvironmentModel environment1a = DataHelper.CreateEnvironmentModel(project1.Id);
                EnvironmentModel environment1b = DataHelper.CreateEnvironmentModel(project1.Id);

                createEnvironmentCommand.Execute(environmentNotExpected);
                createEnvironmentCommand.Execute(environment1a);
                createEnvironmentCommand.Execute(environment1b);

                List<EnvironmentModel> result = environmentRepo.GetAllByProjectId(project1.Id).ToList();

                Assert.AreEqual(2, result.Count);
                Assert.IsNull(result.FirstOrDefault(x => x.Id == environmentNotExpected.Id));
                Assert.IsNotNull(result.FirstOrDefault(x => x.Id == environment1a.Id));
                Assert.IsNotNull(result.FirstOrDefault(x => x.Id == environment1b.Id));
            }

        }

        #endregion


    }
}
