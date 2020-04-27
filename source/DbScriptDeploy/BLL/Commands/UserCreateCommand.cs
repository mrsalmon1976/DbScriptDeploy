using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Exceptions;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Security;
using DbScriptDeploy.BLL.Validators;

namespace DbScriptDeploy.BLL.Commands
{
    public interface IUserCreateCommand
    {
        UserModel Execute(string userName, string password);
    }
    public class UserCreateCommand : IUserCreateCommand
    {
        private IDbContext _dbContext;
        private IUserValidator _userValidator;
        private IPasswordProvider _passwordProvider;

        public UserCreateCommand(IDbContext dbContext, IUserValidator userValidator, IPasswordProvider passwordProvider)
        {
            _dbContext = dbContext;
            _userValidator = userValidator;
            _passwordProvider = passwordProvider;
        }

        public UserModel Execute(string userName, string password)
        {
            UserModel user = new UserModel();
            user.UserName = userName;
            user.Password = password;
            user.PasswordExpired = true;
            user.CreateDate = DateTime.UtcNow;

            // validate before we try to hash the password
            ValidationResult result = _userValidator.Validate(user);
            if (!result.Success)
            {
                throw new ValidationException(result.Messages);
            }

            // hash the password
            user.Password = _passwordProvider.HashPassword(password, _passwordProvider.GenerateSalt());

            // insert new record
            string createDate = user.CreateDate.ToString(SQLiteDbContext.DateTimeFormat);
            string sql = $"INSERT INTO User (Id, UserName, Password, PasswordExpired, CreateDate) VALUES (@Id, @UserName, @Password, 1, '{createDate}')";
            _dbContext.ExecuteNonQuery(sql, user);

            return user;
        }
    }
}
