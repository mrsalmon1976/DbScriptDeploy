using DbScriptDeploy.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.BLL.Data;

namespace DbScriptDeploy.BLL.Validators
{
    public interface IEnvironmentValidator
    {
        ValidationResult Validate(EnvironmentModel model);
    }

    public class EnvironmentValidator : IEnvironmentValidator
    {

        public EnvironmentValidator()
        {
        }

        public ValidationResult Validate(EnvironmentModel model)
        {
            ValidationResult result = new ValidationResult();

            if (String.IsNullOrWhiteSpace(model.HostName))
            {
                result.Messages.Add("Host name cannot be empty");
            }
            if (model.DbType == DatabaseType.None)
            {
                result.Messages.Add("Database type cannot be \"None\"");
            }
            if (String.IsNullOrWhiteSpace(model.DbName))
            {
                result.Messages.Add("Database name cannot be empty");
            }
            if (model.Port <= 0)
            {
                result.Messages.Add("Port number must be greater than 0");
            }
            if (String.IsNullOrWhiteSpace(model.UserName))
            {
                result.Messages.Add("User name cannot be empty");
            }
            if (model.DisplayOrder <= 0)
            {
                result.Messages.Add("Display order must be greater than 0");
            }
            if (model.ProjectId == Guid.Empty)
            {
                result.Messages.Add("Project must be set");
            }
            return result;
        }
    }
}
