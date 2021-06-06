using DbScriptDeploy.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.DbScriptDeploy.Console
{
    public class TestDataHelper
    {
        public static ProjectModel CreateProjectModel()
        {
            ProjectModel model = new ProjectModel();
            model.Id = new Random().Next(10, 1000);
            model.Name = "TestProject_" + Guid.NewGuid().ToString();
            model.CreateDate = DateTime.UtcNow;
            return model;
        }
    }
}
