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
    public interface IScriptValidator
    {
        ValidationResult Validate(ScriptModel model);
    }

    public class ScriptValidator : IScriptValidator
    {

        public ScriptValidator()
        {
        }

        public ValidationResult Validate(ScriptModel model)
        {
            ValidationResult result = new ValidationResult();

            if (String.IsNullOrWhiteSpace(model.Name))
            {
                result.Messages.Add("Name cannot be empty");
            }
            if (String.IsNullOrWhiteSpace(model.ScriptUp))
            {
                result.Messages.Add("Up script cannot be empty");
            }
            if (model.ProjectId <= 0)
            {
                result.Messages.Add("Project must be set");
            }
            return result;
        }
    }
}
