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
    public class ScriptExecutionRepositoryTest
    {
        private IDbContext _dbContext;


        [SetUp]
        public void ScriptExecutionRepositoryTest_SetUp()
        {
            _dbContext = Substitute.For<IDbContext>();
        }

        [TearDown]
        public void ScriptExecutionRepositoryTest_TearDown()
        {
            // delete all .db files (in case previous tests have failed)
            TestHelper.DeleteTestFiles(AppContext.BaseDirectory, "*.dbtest");

        }

        #region GetByScriptId 

        /// <summary>
        /// Tests that the GetByScriptId method fetches all tags for a given script Id
        /// </summary>
        [Test]
        [Ignore("Not fully implemented yet")]
        public void GetByScriptId_Integration_ReturnsData()
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName() + ".dbtest");
            ScriptModel model = DataHelper.CreateScriptModel();
            model.Name = Path.GetRandomFileName();

            using (SQLiteDbContext dbContext = new SQLiteDbContext(filePath))
            {
                dbContext.Initialise();
                dbContext.BeginTransaction();

                IScriptExecutionRepository scriptExecutionRepo = new ScriptExecutionRepository(dbContext);

                // create the script
                IScriptCreateCommand createScriptCommand = new ScriptCreateCommand(dbContext, new ScriptValidator());
                ScriptModel script = createScriptCommand.Execute(model);

                // create the executions
                IScriptExecutionCreateCommand createScriptExecutionCommand = new ScriptExecutionCreateCommand(dbContext, new ScriptExecutionValidator());
                ScriptExecutionModel sem1 = DataHelper.CreateScriptExecutionModel(scriptId: script.Id);
                ScriptExecutionModel sem2 = DataHelper.CreateScriptExecutionModel(scriptId: script.Id);
                sem1 = createScriptExecutionCommand.Execute(sem1);
                sem2 = createScriptExecutionCommand.Execute(sem2);

                ScriptExecutionModel[] result = scriptExecutionRepo.GetByScriptId(script.Id).ToArray();
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Length);
                Assert.IsNotNull(result.SingleOrDefault(x => x.Id == sem1.Id));
                Assert.IsNotNull(result.SingleOrDefault(x => x.Id == sem2.Id));
            }

        }

        #endregion


        #region GetByScriptId 

        /// <summary>
        /// Tests that the GetByProjectId method fetches all tags for a given script Id
        /// </summary>
        [Test]
        [Ignore("Not fully implemented yet")]
        public void GetByProjectId_Integration_ReturnsData()
        {
            Random r = new Random();
            string filePath = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName() + ".dbtest");
            int projectId1 = r.Next(10, 1000);
            int projectId2 = r.Next(2000, 3000);
            ScriptModel model1_A = DataHelper.CreateScriptModel(projectId: projectId1);
            ScriptModel model1_B = DataHelper.CreateScriptModel(projectId: projectId1);
            
            ScriptModel model2_A = DataHelper.CreateScriptModel(projectId: projectId2);

            using (SQLiteDbContext dbContext = new SQLiteDbContext(filePath))
            {
                dbContext.Initialise();
                dbContext.BeginTransaction();

                IScriptExecutionRepository scriptExecutionRepo = new ScriptExecutionRepository(dbContext);

                IScriptCreateCommand createScriptCommand = new ScriptCreateCommand(dbContext, new ScriptValidator());

                // create the script
                ScriptModel script1_A = createScriptCommand.Execute(model1_A);
                ScriptModel script1_B = createScriptCommand.Execute(model1_B);
                ScriptModel script2_A = createScriptCommand.Execute(model2_A);

                List<ScriptExecutionModel> result1 = scriptExecutionRepo.GetByProjectId(projectId1).ToList();
                Assert.AreEqual(2, result1.Count);
                Assert.IsNotNull(result1.SingleOrDefault(x => x.ScriptId == script1_A.Id));
                Assert.IsNotNull(result1.SingleOrDefault(x => x.ScriptId == script1_B.Id));

                List<ScriptExecutionModel> result2 = scriptExecutionRepo.GetByProjectId(projectId2).ToList();
                Assert.AreEqual(1, result2.Count);
                Assert.IsNotNull(result2.SingleOrDefault(x => x.ScriptId == script2_A.Id));
            }

        }

        #endregion


    }
}
