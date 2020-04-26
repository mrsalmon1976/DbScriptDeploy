using DbScriptDeploy.BLL.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.DbScriptDeploy.BLL.Data
{
    [TestFixture]
    public class SQLiteDbContextTest
    {
        private SQLiteDbContext _dbContext;
        private string _filePath;

        [SetUp]
        public void SQLiteDbContextTest_SetUp()
        {
            _filePath = Path.Combine(AppContext.BaseDirectory, "_test_" + Path.GetRandomFileName() + ".db");
            _dbContext = new SQLiteDbContext(_filePath);
        }

        [TearDown]
        public void SQLiteDbContextTest_TearDown()
        {
            _dbContext.Dispose();

            // delete all .db files (in case previous tests have failed)
            TestHelper.DeleteTestFiles(AppContext.BaseDirectory, "_test_*.db");
        }

        #region Initalise Tests

        [Test]
        public void Initialise_CreatesDatabaseFile()
        {
            _dbContext.Initialise();
            Assert.IsTrue(File.Exists(_filePath));
        }

        [Test]
        public void Initialise_CreatesTable_Project()
        {
            _dbContext.Initialise();
            const string sql = "SELECT * FROM Project";
            _dbContext.ExecuteNonQuery(sql);

            Assert.IsTrue(File.Exists(_filePath));
        }

        [Test]
        public void Initialise_CreatesTable_User()
        {
            _dbContext.Initialise();
            const string sql = "SELECT * FROM User";
            _dbContext.ExecuteNonQuery(sql);

            Assert.IsTrue(File.Exists(_filePath));
        }


        [Test]
        public void Initialise_CreatesTable_UserClaim()
        {
            _dbContext.Initialise();
            const string sql = "SELECT * FROM UserClaim";
            _dbContext.ExecuteNonQuery(sql);

            Assert.IsTrue(File.Exists(_filePath));
        }


        #endregion
    }
}
