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
    public class UserClaimRepositoryTest
    {
        private IDbContext _dbContext;


        [SetUp]
        public void UserClaimRepositoryTest_SetUp()
        {
            _dbContext = Substitute.For<IDbContext>();
        }

        [TearDown]
        public void UserClaimRepositoryTest_TearDown()
        {
            // delete all .db files (in case previous tests have failed)
            TestHelper.DeleteTestFiles(AppContext.BaseDirectory, "*.dbtest");

        }

        /// <summary>
        /// Tests that the GetByUserId method fetches a user by it's user name
        /// </summary>
        [Test]
        public void GetByUserId_Integration_ReturnsData()
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName() + ".dbtest");
            string userName = Path.GetRandomFileName();

            using (SQLiteDbContext dbContext = new SQLiteDbContext(filePath))
            {
                dbContext.Initialise();
                dbContext.BeginTransaction();

                IUserRepository userRepo = new UserRepository(dbContext);
                IUserClaimRepository userClaimRepo = new UserClaimRepository(dbContext);

                IUserCreateCommand createUserCommand = new UserCreateCommand(dbContext, new UserValidator(userRepo), new PasswordProvider());
                IUserClaimCreateCommand createUserClaimCommand = new UserClaimCreateCommand(dbContext, new UserClaimValidator(userClaimRepo));

                // create the user
                UserModel user = DataHelper.CreateUserModel();
                user.UserName = userName;
                createUserCommand.Execute(user.UserName, user.Password);

                // create two claims
                UserClaimModel claim1 = createUserClaimCommand.Execute(user.Id, Guid.NewGuid().ToString(), null);
                UserClaimModel claim2 = createUserClaimCommand.Execute(user.Id, Guid.NewGuid().ToString(), null);

                List<UserClaimModel> result = userClaimRepo.GetByUserId(user.Id).ToList();
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Count);
                Assert.IsNotNull(result.FirstOrDefault(x => x.Id == claim1.Id));
                Assert.IsNotNull(result.FirstOrDefault(x => x.Id == claim2.Id));
            }

        }
    }
}
