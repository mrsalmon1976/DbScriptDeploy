using DbScriptDeploy.BLL.Encoding;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.ViewModels.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.Services
{
    public interface IModelBinderService
    {
        ScriptModel BindScriptModel(ScriptViewModel model);

        ScriptViewModel BindScriptViewModel(ScriptModel model);

        ScriptExecutionViewModel BindScriptExecutionViewModel(ScriptExecutionModel model);

    }

    public class ModelBinderService : IModelBinderService
    {

        public ScriptModel BindScriptModel(ScriptViewModel model)
        {
            return new ScriptModel()
            {
                Id = (String.IsNullOrWhiteSpace(model.Id) ? 0 : UrlUtility.DecodeNumber(model.Id)),
                Name = model.Name,
                ProjectId = (String.IsNullOrWhiteSpace(model.ProjectId) ? 0 : UrlUtility.DecodeNumber(model.ProjectId)),
                ScriptDown = model.ScriptDown,
                ScriptUp = model.ScriptUp,
                CreateDate = model.CreateDate
            };
        }
        public ScriptViewModel BindScriptViewModel(ScriptModel model)
        {
            return new ScriptViewModel()
            {
                Id = (model.Id <= 0 ? "" : UrlUtility.EncodeNumber(model.Id)),
                Name = model.Name,
                ProjectId = (model.ProjectId <= 0 ? "" : UrlUtility.EncodeNumber(model.ProjectId)),
                ScriptDown = model.ScriptDown,
                ScriptUp = model.ScriptUp,
                CreateDate = model.CreateDate
            };
        }

        public ScriptExecutionViewModel BindScriptExecutionViewModel(ScriptExecutionModel model)
        {
            return new ScriptExecutionViewModel()
            {
                Id = (model.Id <= 0 ? "" : UrlUtility.EncodeNumber(model.Id)),
                ScriptId = (model.ScriptId <= 0 ? "" : UrlUtility.EncodeNumber(model.ScriptId)),
                EnvironmentId = (model.EnvironmentId <= 0 ? "" : UrlUtility.EncodeNumber(model.EnvironmentId)),
                ExecutionStartDate = model.ExecutionStartDate,
                ExecutionCompleteDate = model.ExecutionCompleteDate
            };
        }
    }
}
