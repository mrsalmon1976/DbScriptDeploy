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
    public interface IScriptExecutionCreateCommand
    {
        ScriptExecutionModel Execute(ScriptExecutionModel scriptExecution);
    }
    public class ScriptExecutionCreateCommand : IScriptExecutionCreateCommand
    {
        private readonly IDbContext _dbContext;
        private readonly IScriptExecutionValidator _scriptExecutionValidator;

        public ScriptExecutionCreateCommand(IDbContext dbContext, IScriptExecutionValidator scriptExecutionValidator)
        {
            _dbContext = dbContext;
            _scriptExecutionValidator = scriptExecutionValidator;
        }

        public ScriptExecutionModel Execute(ScriptExecutionModel scriptExecution)
        {

            // validate before we try to hash the password
            ValidationResult result = _scriptExecutionValidator.Validate(scriptExecution);
            if (!result.Success)
            {
                throw new ValidationException(result.Messages);
            }

            // insert new record
            string executionStartDate = scriptExecution.ExecutionStartDate.ToString(SQLiteDbContext.DateTimeFormat);
            string executionCompleteDate = scriptExecution.ExecutionCompleteDate.ToString(SQLiteDbContext.DateTimeFormat);

            string sql = $"INSERT INTO ScriptExecution (ScriptId, EnvironmentId, ExecutionStartDate, ExecutionCompleteDate) VALUES (@ScriptId, @EnvironmentId, '{executionStartDate}', '{executionCompleteDate}');SELECT last_insert_rowid()";
            scriptExecution.Id = _dbContext.ExecuteScalar<int>(sql, scriptExecution);

            return scriptExecution;
        }
    }
}
