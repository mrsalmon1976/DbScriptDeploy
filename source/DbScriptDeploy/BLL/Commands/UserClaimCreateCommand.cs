using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Exceptions;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.BLL.Commands
{
    public interface IUserClaimCreateCommand
    {
        UserClaimModel Execute(Guid userId, string name, Guid? projectId);

    }

    public class UserClaimCreateCommand : IUserClaimCreateCommand
    {
        private readonly IDbContext _dbContext;
        private readonly IUserClaimValidator _userClaimValidator;

        public UserClaimCreateCommand(IDbContext dbContext, IUserClaimValidator userClaimValidator)
        {
            _dbContext = dbContext;
            _userClaimValidator = userClaimValidator;
        }

        public UserClaimModel Execute(Guid userId, string name, Guid? projectId)
        {
            UserClaimModel userClaim = new UserClaimModel();
            userClaim.UserId = userId;
            userClaim.Name = name;
            userClaim.ProjectId = projectId;

            ValidationResult result = _userClaimValidator.Validate(userClaim);
            if (!result.Success)
            {
                throw new ValidationException(result.Messages);
            }

            // insert new record
            string sql = @"INSERT INTO UserClaim (Id, UserId, Name, ProjectId) VALUES (@Id, @UserId, @Name, @ProjectId)";
            _dbContext.ExecuteNonQuery(sql, userClaim);

            return userClaim;

        }
    }
}
