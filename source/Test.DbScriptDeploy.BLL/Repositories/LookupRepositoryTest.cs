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
    public class LookupRepositoryTest
    {
        private IDbContext _dbContext;


        [SetUp]
        public void LookupRepositoryTest_SetUp()
        {
            _dbContext = Substitute.For<IDbContext>();
        }

        [TearDown]
        public void LookupRepositoryTest_TearDown()
        {
            // delete all .db files (in case previous tests have failed)
            TestHelper.DeleteTestFiles(AppContext.BaseDirectory, "*.dbtest");

        }


        #region GetAllDesignations 

        /// <summary>
        /// Tests that the GetAllDesignations method gets all designations in the database
        /// </summary>
        [Test]
        public void GetAllDesignations_ReturnsData()
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName() + ".dbtest");
            string name = Path.GetRandomFileName();

            using (SQLiteDbContext dbContext = new SQLiteDbContext(filePath))
            {
                dbContext.Initialise();
                dbContext.BeginTransaction();

                ILookupRepository lookupRepo = new LookupRepository(dbContext);

                IDesignationCreateCommand createDesignationCommand = new DesignationCreateCommand(dbContext, new DesignationValidator());

                // create the user
                DesignationModel designation = createDesignationCommand.Execute(name);

                DesignationModel result = lookupRepo.GetAllDesignations().FirstOrDefault(x => x.Id == designation.Id);
                Assert.IsNotNull(result);
                Assert.AreEqual(name, result.Name);
            }

        }

        #endregion

        


    }
}
