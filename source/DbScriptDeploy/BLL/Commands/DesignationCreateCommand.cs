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
    public interface IDesignationCreateCommand
    {
        DesignationModel Execute(string name);
    }
    public class DesignationCreateCommand : IDesignationCreateCommand
    {
        private readonly IDbContext _dbContext;
        private readonly IDesignationValidator _designationValidator;

        public DesignationCreateCommand(IDbContext dbContext, IDesignationValidator designationValidator)
        {
            _dbContext = dbContext;
            _designationValidator = designationValidator;
        }

        public DesignationModel Execute(string name)
        {
            DesignationModel designation = new DesignationModel();
            designation.Id = Guid.NewGuid();
            designation.Name = name;
            designation.CreateDate = DateTime.UtcNow;

            // validate before we try to hash the password
            ValidationResult result = _designationValidator.Validate(designation);
            if (!result.Success)
            {
                throw new ValidationException(result.Messages);
            }

            // insert new record
            string createDate = designation.CreateDate.ToString(SQLiteDbContext.DateTimeFormat);
            string sql = $"INSERT INTO Designation (Id, Name, CreateDate) VALUES (@Id, @Name, '{createDate}')";
            _dbContext.ExecuteNonQuery(sql, designation);

            return designation;
        }
    }
}
