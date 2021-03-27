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
    public class ScriptTagRepositoryTest
    {
        private IDbContext _dbContext;


        [SetUp]
        public void ScriptTagRepositoryTest_SetUp()
        {
            _dbContext = Substitute.For<IDbContext>();
        }

        [TearDown]
        public void ScriptTagRepositoryTest_TearDown()
        {
            // delete all .db files (in case previous tests have failed)
            TestHelper.DeleteTestFiles(AppContext.BaseDirectory, "*.dbtest");

        }


        #region GetByScriptId 

        /// <summary>
        /// Tests that the GetByScriptId method fetches all tags for a given script Id
        /// </summary>
        [Test]
        public void GetByScriptId_Integration_ReturnsData()
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName() + ".dbtest");
            ScriptModel model = DataHelper.CreateScriptModel();
            model.Tags = new string[] { "Tag1", "Tag2" };
            model.Name = Path.GetRandomFileName();

            using (SQLiteDbContext dbContext = new SQLiteDbContext(filePath))
            {
                dbContext.Initialise();
                dbContext.BeginTransaction();

                IScriptTagRepository scriptTagRepo = new ScriptTagRepository(dbContext);

                IScriptCreateCommand createScriptCommand = new ScriptCreateCommand(dbContext, new ScriptValidator());

                // create the script
                ScriptModel script = createScriptCommand.Execute(model);

                ScriptTagModel[] result = scriptTagRepo.GetByScriptId(script.Id).ToArray();
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Length);
                Assert.AreEqual("Tag1", result[0].Tag);
                Assert.AreEqual("Tag2", result[1].Tag);
            }

        }

        #endregion


        #region GetByScriptId 

        /// <summary>
        /// Tests that the GetByProjectId method fetches all tags for a given script Id
        /// </summary>
        [Test]
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

                IScriptTagRepository scriptTagRepo = new ScriptTagRepository(dbContext);

                IScriptCreateCommand createScriptCommand = new ScriptCreateCommand(dbContext, new ScriptValidator());

                // create the script
                ScriptModel script1_A = createScriptCommand.Execute(model1_A);
                ScriptModel script1_B = createScriptCommand.Execute(model1_B);
                ScriptModel script2_A = createScriptCommand.Execute(model2_A);

                List<ScriptTagModel> result1 = scriptTagRepo.GetByProjectId(projectId1).ToList();
                Assert.AreEqual(2, result1.Count);
                Assert.IsNotNull(result1.SingleOrDefault(x => x.ScriptId == script1_A.Id));
                Assert.IsNotNull(result1.SingleOrDefault(x => x.ScriptId == script1_B.Id));

                List<ScriptTagModel> result2 = scriptTagRepo.GetByProjectId(projectId2).ToList();
                Assert.AreEqual(1, result2.Count);
                Assert.IsNotNull(result2.SingleOrDefault(x => x.ScriptId == script2_A.Id));
            }

        }

        #endregion



    }
}
