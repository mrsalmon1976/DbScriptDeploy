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
    public interface IScriptCreateCommand
    {
        ScriptModel Execute(ScriptModel Script);
    }
    public class ScriptCreateCommand : IScriptCreateCommand
    {
        private readonly IDbContext _dbContext;
        private readonly IScriptValidator _scriptValidator;

        public ScriptCreateCommand(IDbContext dbContext, IScriptValidator scriptValidator)
        {
            _dbContext = dbContext;
            _scriptValidator = scriptValidator;
        }

        public ScriptModel Execute(ScriptModel script)
        {

            // set any properties
            script.CreateDate = DateTime.UtcNow;

            // validate before we try to hash the password
            ValidationResult result = _scriptValidator.Validate(script);
            if (!result.Success)
            {
                throw new ValidationException(result.Messages);
            }

            // insert new record
            string createDate = script.CreateDate.ToString(SQLiteDbContext.DateTimeFormat);

            string sql = $"INSERT INTO Script (Name, ProjectId, ScriptUp, ScriptDown, CreateDate) VALUES (@Name, @ProjectId, @ScriptUp, @ScriptDown, '{createDate}');SELECT last_insert_rowid()";
            script.Id = _dbContext.ExecuteScalar<int>(sql, script);

            foreach (string tag in script.Tags)
            {
                if (String.IsNullOrWhiteSpace(tag))
                {
                    continue;
                }
                sql = $"INSERT INTO ScriptTag (ScriptId, Tag, CreateDate) VALUES (@ScriptId, @Tag, '{createDate}')";
                _dbContext.ExecuteNonQuery(sql, new { ScriptId = script.Id, Tag = tag });
            }

            return script;
        }
    }
}
