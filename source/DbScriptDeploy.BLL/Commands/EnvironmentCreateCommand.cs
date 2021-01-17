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
using DbScriptDeploy.BLL.Security;

namespace DbScriptDeploy.BLL.Commands
{
    public interface IEnvironmentCreateCommand
    {
        EnvironmentModel Execute(EnvironmentModel environment);
    }
    public class EnvironmentCreateCommand : IEnvironmentCreateCommand
    {
        private readonly IDbContext _dbContext;
        private readonly IEnvironmentValidator _environmentValidator;
        private readonly IEncryptionProvider _encryptionProvider;
        private const int SALT_LENGTH = 10;

        public EnvironmentCreateCommand(IDbContext dbContext, IEnvironmentValidator environmentValidator, IEncryptionProvider encryptionProvider)
        {
            _dbContext = dbContext;
            _environmentValidator = environmentValidator;
            _encryptionProvider = encryptionProvider;
        }

        public EnvironmentModel Execute(EnvironmentModel environment)
        {

            // set any properties
            environment.CreateDate = DateTime.UtcNow;
            if (environment.DisplayOrder <= 0)
            {
                environment.DisplayOrder = 1000000;
            }

            // validate before we try to hash the password
            ValidationResult result = _environmentValidator.Validate(environment);
            if (!result.Success)
            {
                throw new ValidationException(result.Messages);
            }

            // insert new record
            string createDate = environment.CreateDate.ToString(SQLiteDbContext.DateTimeFormat);
            string password = _encryptionProvider.SimpleEncrypt(environment.Password, _encryptionProvider.NewKey(), DomainConstants.EncryptionSalt);

            string sql = $"INSERT INTO Environment (Name, ProjectId, Host, DbType, Database, Port, UserName, Password, DisplayOrder, DesignationId, CreateDate) VALUES (@Name, @ProjectId, @Host, @DbType, @Database, @Port, @UserName, '{password}', @DisplayOrder, @DesignationId, '{createDate}');SELECT last_insert_rowid()";
            environment.Id = _dbContext.ExecuteScalar<int>(sql, environment);

            return environment;
        }
    }
}
