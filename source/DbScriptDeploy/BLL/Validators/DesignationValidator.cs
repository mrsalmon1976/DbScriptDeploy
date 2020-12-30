using DbScriptDeploy.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbScriptDeploy.BLL.Repositories;

namespace DbScriptDeploy.BLL.Validators
{
    public interface IDesignationValidator
    {
        ValidationResult Validate(DesignationModel model);
    }

    public class DesignationValidator : IDesignationValidator
    {

        public DesignationValidator()
        {
        }

        public ValidationResult Validate(DesignationModel model)
        {
            ValidationResult result = new ValidationResult();

            if (String.IsNullOrWhiteSpace(model.Name))
            {
                result.Messages.Add("Designation name cannot be empty");
            }
            return result;
        }
    }
}
