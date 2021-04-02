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
    public interface IScriptExecutionValidator
    {
        ValidationResult Validate(ScriptExecutionModel model);
    }

    public class ScriptExecutionValidator : IScriptExecutionValidator
    {

        public ScriptExecutionValidator()
        {
        }

        public ValidationResult Validate(ScriptExecutionModel model)
        {
            ValidationResult result = new ValidationResult();

            if (model.ScriptId <= 0)
            {
                result.Messages.Add("Script reference must be set");
            }
            if (model.EnvironmentId <= 0)
            {
                result.Messages.Add("Environment reference must be set");
            }
            if (model.ExecutionCompleteDate < model.ExecutionStartDate)
            {
                result.Messages.Add("Execution completion date cannot be before execution start date");
            }
            return result;
        }
    }
}
