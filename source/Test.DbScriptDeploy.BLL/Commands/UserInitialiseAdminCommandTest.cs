using DbScriptDeploy.BLL.Commands;
using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Exceptions;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.BLL.Security;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.DbScriptDeploy.BLL.Services
{
    [TestFixture]
    public class UserInitialiseAdminCommandTest
    {
        private IUserInitialiseAdminCommand _userInitialiseAdminCommand;

        private IDbContext _dbContext;
        private IUserRepository _userRepo;
        private IUserCreateCommand _createUserCommand;
        private IUserClaimCreateCommand _createUserClaimCommand;

        [SetUp]
        public void UserServiceTest_SetUp()
        {
            _dbContext = Substitute.For<IDbContext>();
            _userRepo = Substitute.For<IUserRepository>();
            _createUserCommand = Substitute.For<IUserCreateCommand>();
            _createUserClaimCommand = Substitute.For<IUserClaimCreateCommand>();
            _userInitialiseAdminCommand = new UserInitialiseAdminCommand(_dbContext, _userRepo, _createUserCommand, _createUserClaimCommand);
        }

        #region InitialiseAdminUser Tests

        [Test]
        public void Execute_NoTransactionOnDbContext_ThrowsException()
        {
            IDbTransaction tran = null;
            _dbContext.Transaction.Returns(tran);

            // execute
            TestDelegate del = () => _userInitialiseAdminCommand.Execute();

            // assert
            Assert.Throws<TransactionMissingException>(del);
            _userRepo.DidNotReceive().GetByUserName(Arg.Any<string>());
        }


        [Test]
        public void Execute_AdminUserExists_ReturnsExistingUser()
        {
            UserModel user = DataHelper.CreateUserModel();
            user.UserName = UserInitialiseAdminCommand.AdminUserName;

            _userRepo.GetByUserName(UserInitialiseAdminCommand.AdminUserName).Returns(user);

            // execute
            UserModel result = _userInitialiseAdminCommand.Execute();
            Assert.IsNotNull(result);

            _userRepo.Received(1).GetByUserName(user.UserName);
            _createUserCommand.DidNotReceive().Execute(Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public void Execute_AdminUserDoesNotExist_ReturnsNewUser()
        {
            UserModel user = new UserModel();
            user.UserName = UserInitialiseAdminCommand.AdminUserName;
            user.Password = UserInitialiseAdminCommand.AdminDefaultPassword;

            UserClaimModel claim = new UserClaimModel();
            claim.UserId = user.Id;
            claim.Name = Claims.Administrator;

            _createUserCommand.Execute(user.UserName, user.Password).Returns(user);
            _createUserClaimCommand.Execute(user.Id, Claims.Administrator, null).Returns(claim);

            // execute
            UserModel result = _userInitialiseAdminCommand.Execute();
            Assert.IsNotNull(result);

            _userRepo.Received(1).GetByUserName(user.UserName);
            _createUserCommand.Received(1).Execute(user.UserName, user.Password);
            _createUserClaimCommand.Received(1).Execute(user.Id, Claims.Administrator, null);
        }


        #endregion
    }
}
