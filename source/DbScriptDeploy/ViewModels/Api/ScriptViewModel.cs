using DbScriptDeploy.BLL.Encoding;
using DbScriptDeploy.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.ViewModels.Api
{
    public class ScriptViewModel
    {
        public string Id { get; set; }

        public string ProjectId { get; set; }

        public string Name { get; set; }

        public string[] Tags { get; set; }
                        
        public string ScriptUp { get; set; }

        public string ScriptDown { get; set; }

        public ScriptModel ToScriptModel()
        {
            return new ScriptModel()
            {
                Id = (String.IsNullOrWhiteSpace(this.Id) ? 0 : UrlUtility.DecodeNumber(this.Id)),
                Name = this.Name,
                ProjectId = (String.IsNullOrWhiteSpace(this.ProjectId) ? 0 : UrlUtility.DecodeNumber(this.ProjectId)),
                ScriptDown = this.ScriptDown,
                ScriptUp = this.ScriptUp,
                Tags = this.Tags
            };
        }

        public static ScriptViewModel FromScriptModel(ScriptModel model)
        {
            return new ScriptViewModel()
            {
                Id = (model.Id <= 0 ? "" : UrlUtility.EncodeNumber(model.Id)),
                Name = model.Name,
                ProjectId = (model.ProjectId <= 0 ? "" : UrlUtility.EncodeNumber(model.ProjectId)),
                ScriptDown = model.ScriptDown,
                ScriptUp = model.ScriptUp,
                Tags = model.Tags,
            };
        }

    }
}
