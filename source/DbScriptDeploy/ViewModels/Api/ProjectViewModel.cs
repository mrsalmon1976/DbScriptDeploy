using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Encoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.ViewModels.Api
{
    public class ProjectViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public static ProjectViewModel FromProjectModel(ProjectModel projectModel)
        {
            ProjectViewModel pvm = new ProjectViewModel();
            pvm.Id = UrlUtility.EncodeNumber(projectModel.Id);
            pvm.Name = projectModel.Name;
            return pvm;
        }
    }
}
