using DbScriptDeploy.BLL.Commands;
using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.BLL.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.BLL.Services
{

    public interface IUserService
    {
        UserModel InitialiseAdminUser();
    }

    public class UserService : IUserService
    {
        private readonly IDbContext _dbContext;
        private readonly IUserRepository _userRepo;
        private readonly ICreateUserCommand _createUserCommand;
        private readonly ICreateUserClaimCommand _createUserClaimCommand;

        public const string AdminUserName = "admin";
        public const string AdminDefaultPassword = "password";

        public UserService(IDbContext dbContext, IUserRepository userRepo, ICreateUserCommand createUserCommand, ICreateUserClaimCommand createUserClaimCommand)
        {
            _dbContext = dbContext;
            _userRepo = userRepo;
            _createUserCommand = createUserCommand;
            _createUserClaimCommand = createUserClaimCommand;
        }

        public UserModel InitialiseAdminUser()
        {
            UserModel user = _userRepo.GetByUserName(AdminUserName);
            if (user == null)
            {
                _dbContext.BeginTransaction();
                user = _createUserCommand.Execute(AdminUserName, AdminDefaultPassword);
                user.Claims.Add(_createUserClaimCommand.Execute(user.Id, ClaimNames.Administrator, null));
                _dbContext.Commit();
            }
            return user;
        }
    }
}
