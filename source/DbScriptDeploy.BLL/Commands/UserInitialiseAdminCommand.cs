using DbScriptDeploy.BLL.Commands;
using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Exceptions;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.BLL.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.BLL.Commands
{

    public interface IUserInitialiseAdminCommand
    {
        UserModel Execute();
    }

    public class UserInitialiseAdminCommand : IUserInitialiseAdminCommand
    {
        private readonly IDbContext _dbContext;
        private readonly IUserRepository _userRepo;
        private readonly IUserCreateCommand _createUserCommand;
        private readonly IUserClaimCreateCommand _createUserClaimCommand;

        public const string AdminUserName = "admin";
        public const string AdminDefaultPassword = "password";

        public UserInitialiseAdminCommand(IDbContext dbContext, IUserRepository userRepo, IUserCreateCommand createUserCommand, IUserClaimCreateCommand createUserClaimCommand)
        {
            _dbContext = dbContext;
            _userRepo = userRepo;
            _createUserCommand = createUserCommand;
            _createUserClaimCommand = createUserClaimCommand;
        }

        public UserModel Execute()
        {
            if (_dbContext.Transaction == null)
            {
                throw new TransactionMissingException();
            }
            UserModel user = _userRepo.GetByUserName(AdminUserName);
            if (user == null)
            {
                user = _createUserCommand.Execute(AdminUserName, AdminDefaultPassword);
                user.Claims.Add(_createUserClaimCommand.Execute(user.Id, ClaimNames.Administrator, null));
            }
            return user;
        }
    }
}
