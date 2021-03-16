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
    public class ScriptRepositoryTest
    {
        private IDbContext _dbContext;


        [SetUp]
        public void ScriptRepositoryTest_SetUp()
        {
            _dbContext = Substitute.For<IDbContext>();
        }

        [TearDown]
        public void ScriptRepositoryTest_TearDown()
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
            ScriptModel model = DataHelper.CreateScriptModel();
            model.Name = Path.GetRandomFileName();

            using (SQLiteDbContext dbContext = new SQLiteDbContext(filePath))
            {
                dbContext.Initialise();
                dbContext.BeginTransaction();

                IScriptRepository scriptRepo = new ScriptRepository(dbContext);

                IScriptCreateCommand createScriptCommand = new ScriptCreateCommand(dbContext, new ScriptValidator());

                // create the script
                ScriptModel script = createScriptCommand.Execute(model);

                ScriptModel result = scriptRepo.GetById(script.Id);
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

                IScriptRepository scriptRepo = new ScriptRepository(dbContext);
                IUserRepository userRepo = new UserRepository(dbContext);
                IUserClaimRepository userClaimRepo = new UserClaimRepository(dbContext);

                IProjectCreateCommand createProjectCommand = new ProjectCreateCommand(dbContext, new ProjectValidator());
                IScriptCreateCommand createScriptCommand = new ScriptCreateCommand(dbContext, new ScriptValidator());

                // create the projects
                ProjectModel projectNotExpected = createProjectCommand.Execute("projectNotExpected");
                ProjectModel project1 = createProjectCommand.Execute("project1");

                //create the scripts
                ScriptModel scriptNotExpected = DataHelper.CreateScriptModel(projectId: projectNotExpected.Id);
                ScriptModel script1a = DataHelper.CreateScriptModel(projectId: project1.Id);
                ScriptModel script1b = DataHelper.CreateScriptModel(projectId: project1.Id);

                createScriptCommand.Execute(scriptNotExpected);
                createScriptCommand.Execute(script1a);
                createScriptCommand.Execute(script1b);

                List<ScriptModel> result = scriptRepo.GetAllByProjectId(project1.Id).ToList();

                Assert.AreEqual(2, result.Count);
                Assert.IsNull(result.FirstOrDefault(x => x.Id == scriptNotExpected.Id));
                Assert.IsNotNull(result.FirstOrDefault(x => x.Id == script1a.Id));
                Assert.IsNotNull(result.FirstOrDefault(x => x.Id == script1b.Id));
            }

        }

        #endregion


    }
}
