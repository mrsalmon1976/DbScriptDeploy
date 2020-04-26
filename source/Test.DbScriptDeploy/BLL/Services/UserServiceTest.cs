using DbScriptDeploy.BLL.Commands;
using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.BLL.Security;
using DbScriptDeploy.BLL.Services;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.DbScriptDeploy.BLL.Services
{
    [TestFixture]
    public class UserServiceTest
    {
        private IUserService _userService;

        private IDbContext _dbContext;
        private IUserRepository _userRepo;
        private ICreateUserCommand _createUserCommand;
        private ICreateUserClaimCommand _createUserClaimCommand;

        [SetUp]
        public void UserServiceTest_SetUp()
        {
            _dbContext = Substitute.For<IDbContext>();
            _userRepo = Substitute.For<IUserRepository>();
            _createUserCommand = Substitute.For<ICreateUserCommand>();
            _createUserClaimCommand = Substitute.For<ICreateUserClaimCommand>();
            _userService = new UserService(_dbContext, _userRepo, _createUserCommand, _createUserClaimCommand);
        }

        #region InitialiseAdminUser Tests

        [Test]
        public void InitialiseAdminUser_AdminUserExists_ReturnsExistingUser()
        {
            UserModel user = DataHelper.CreateUserModel();
            user.UserName = UserService.AdminUserName;

            _userRepo.GetByUserName(UserService.AdminUserName).Returns(user);

            // execute
            UserModel result = _userService.InitialiseAdminUser();
            Assert.IsNotNull(result);

            _userRepo.Received(1).GetByUserName(user.UserName);
            _createUserCommand.DidNotReceive().Execute(Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public void InitialiseAdminUser_AdminUserDoesNotExist_ReturnsNewUser()
        {
            UserModel user = new UserModel();
            user.UserName = UserService.AdminUserName;
            user.Password = UserService.AdminDefaultPassword;

            UserClaimModel claim = new UserClaimModel();
            claim.UserId = user.Id;
            claim.Name = ClaimNames.Administrator;

            _createUserCommand.Execute(user.UserName, user.Password).Returns(user);
            _createUserClaimCommand.Execute(user.Id, ClaimNames.Administrator, null).Returns(claim);

            // execute
            UserModel result = _userService.InitialiseAdminUser();
            Assert.IsNotNull(result);

            _userRepo.Received(1).GetByUserName(user.UserName);
            _createUserCommand.Received(1).Execute(user.UserName, user.Password);
            _createUserClaimCommand.Received(1).Execute(user.Id, ClaimNames.Administrator, null);
            _dbContext.Received(1).BeginTransaction();
            _dbContext.Received(1).Commit();
        }


        #endregion
    }
}
