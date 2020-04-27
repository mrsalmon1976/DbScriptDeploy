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
    public interface IProjectCreateCommand
    {
        ProjectModel Execute(string name);
    }
    public class ProjectCreateCommand : IProjectCreateCommand
    {
        private IDbContext _dbContext;
        private IProjectValidator _projectValidator;

        public ProjectCreateCommand(IDbContext dbContext, IProjectValidator projectValidator)
        {
            _dbContext = dbContext;
            _projectValidator = projectValidator;
        }

        public ProjectModel Execute(string name)
        {
            ProjectModel project = new ProjectModel();
            project.Name = name;
            project.CreateDate = DateTime.UtcNow;

            // validate before we try to hash the password
            ValidationResult result = _projectValidator.Validate(project);
            if (!result.Success)
            {
                throw new ValidationException(result.Messages);
            }

            // insert new record
            string createDate = project.CreateDate.ToString(SQLiteDbContext.DateTimeFormat);
            string sql = $"INSERT INTO Project (Id, Name, CreateDate) VALUES (@Id, @Name, '{createDate}')";
            _dbContext.ExecuteNonQuery(sql, project);

            return project;
        }
    }
}
