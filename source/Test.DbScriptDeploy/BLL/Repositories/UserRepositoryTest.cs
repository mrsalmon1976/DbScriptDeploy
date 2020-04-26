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
    public class UserRepositoryTest
    {
        private IDbContext _dbContext;


        [SetUp]
        public void UserRepositoryTest_SetUp()
        {
            _dbContext = Substitute.For<IDbContext>();
        }

        [TearDown]
        public void UserRepositoryTest_TearDown()
        {
            // delete all .db files (in case previous tests have failed)
            TestHelper.DeleteTestFiles(AppContext.BaseDirectory, "*.dbtest");

        }

        /// <summary>
        /// Tests that the GetAll method returns a sorted list of users
        /// </summary>
        [Test]
        public void GetAll_Integration_ReturnsSortedList()
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName() + ".dbtest");

            using (SQLiteDbContext dbContext = new SQLiteDbContext(filePath))
            {
                dbContext.Initialise();
                dbContext.BeginTransaction();

                IUserRepository userRepo = new UserRepository(dbContext);

                ICreateUserCommand createUserCommand = new CreateUserCommand(dbContext, new UserValidator(userRepo), new PasswordProvider());

                // create the users
                UserModel userB = DataHelper.CreateUserModel();
                userB.UserName = "bbb";
                createUserCommand.Execute(userB.UserName, userB.Password);

                UserModel userC = DataHelper.CreateUserModel();
                userC.UserName = "ccc";
                createUserCommand.Execute(userC.UserName, userC.Password);

                UserModel userA = DataHelper.CreateUserModel();
                userA.UserName = "aaa";
                createUserCommand.Execute(userA.UserName, userA.Password);

                List<UserModel> result = userRepo.GetAll().ToList();
                Assert.AreEqual(3, result.Count);
                Assert.AreEqual(userA.UserName, result[0].UserName);
                Assert.AreEqual(userB.UserName, result[1].UserName);
                Assert.AreEqual(userC.UserName, result[2].UserName);
            }

        }

        /// <summary>
        /// Tests that the GetByUserName method fetches a user by it's user name
        /// </summary>
        [Test]
        public void GetByUserName_Integration_ReturnsData()
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName() + ".dbtest");
            string userName = Path.GetRandomFileName();

            using (SQLiteDbContext dbContext = new SQLiteDbContext(filePath))
            {
                dbContext.Initialise();
                dbContext.BeginTransaction();

                IUserRepository userRepo = new UserRepository(dbContext);

                ICreateUserCommand createUserCommand = new CreateUserCommand(dbContext, new UserValidator(userRepo), new PasswordProvider());

                // create the user
                UserModel user = DataHelper.CreateUserModel();
                user.UserName = userName;
                createUserCommand.Execute(user.UserName, user.Password);

                UserModel result = userRepo.GetByUserName(userName);
                Assert.IsNotNull(result);
                Assert.AreEqual(userName, result.UserName);
            }

        }
    }
}
