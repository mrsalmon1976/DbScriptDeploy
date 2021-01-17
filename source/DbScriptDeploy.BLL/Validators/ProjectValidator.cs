﻿using DbScriptDeploy.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbScriptDeploy.BLL.Repositories;

namespace DbScriptDeploy.BLL.Validators
{
    public interface IProjectValidator
    {
        ValidationResult Validate(ProjectModel model);
    }

    public class ProjectValidator : IProjectValidator
    {

        public ProjectValidator()
        {
        }

        public ValidationResult Validate(ProjectModel model)
        {
            ValidationResult result = new ValidationResult();

            if (String.IsNullOrWhiteSpace(model.Name))
            {
                result.Messages.Add("Project name cannot be empty");
            }
            return result;
        }
    }
}
