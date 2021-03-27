using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.DbScriptDeploy
{
    public class DataHelper
    {

        //public static ProjectModel CreateProjectModel()
        //{
        //    Random r = new Random();
        //    ProjectModel model = new ProjectModel();
        //    model.Id = r.Next(1, 1000);
        //    model.Name = Path.GetRandomFileName();
        //    return model;
        //}

        public static string RandomString()
        {
            return Path.GetTempFileName();
        }

        public static DesignationModel CreateDesignationModel(int id = 0)
        {
            DesignationModel model = new DesignationModel();
            model.Id = id;
            model.Name = "TestDesignation";
            model.CreateDate = DateTime.UtcNow;
            return model;
        }

        public static EnvironmentModel CreateEnvironmentModel(int? projectId = null)
        {
            Random r = new Random();
            EnvironmentModel model = new EnvironmentModel();
            model.Name = "TestEnvironment";
            model.Database = "MyFakeDb";
            model.DbType = Lookups.DatabaseType.SqlServer;
            model.DisplayOrder = 1;
            model.Host = "MyFakeServer";
            model.Port = 9876;
            model.ProjectId = projectId ?? r.Next(1, 1000);
            model.UserName = "Eric";
            model.Password = "password";
            model.DesignationId = r.Next(1, 1000);
            model.CreateDate = DateTime.UtcNow;
            return model;
        }

        public static ProjectModel CreateProjectModel()
        {
            ProjectModel model = new ProjectModel();
            model.Name = "TestProject";
            model.CreateDate = DateTime.UtcNow;
            return model;
        }

        public static ScriptModel CreateScriptModel(int? id = null, int? projectId = null)
        {
            Random r = new Random();
            ScriptModel model = new ScriptModel();
            model.Id = id ?? r.Next(1, 1000);
            model.Name = "TestScript";
            model.ProjectId = projectId ?? r.Next(1, 1000);
            model.Tags = new string[] { "Tag1" };
            model.ScriptUp = "This is the up script";
            model.ScriptDown = "This is the down script";
            model.CreateDate = DateTime.UtcNow;
            return model;
        }

        public static ScriptTagModel CreateScriptTagModel(int? id = null, int? scriptId = null)
        {
            Random r = new Random();
            ScriptTagModel model = new ScriptTagModel();
            model.Id = id ?? r.Next(1, 1000);
            model.Tag = "TestTag " + model.Id.ToString();
            model.ScriptId = scriptId ?? r.Next(1, 1000);
            model.CreateDate = DateTime.UtcNow;
            return model;
        }


        public static UserClaimModel CreateUserClaimModel(Guid? userId = null, string name = null, int? projectId = null)
        {
            UserClaimModel model = new UserClaimModel();
            model.UserId = userId ?? Guid.NewGuid();
            model.Name = name ?? Guid.NewGuid().ToString();
            model.ProjectId = projectId;
            return model;
        }

        public static UserModel CreateUserModel()
        {
            UserModel model = new UserModel();
            model.UserName = "TestUser";
            model.Password = "password";
            model.CreateDate = DateTime.UtcNow;
            return model;
        }

        //public static void InsertProjectModel(IDbContext dbContext, ProjectModel project)
        //{
        //    const string sql = @"INSERT INTO Projects (Name, CreateDate) VALUES (@Name, @CreateDate)";
        //    dbContext.ExecuteNonQuery(sql, project);
        //    project.Id = dbContext.ExecuteScalar<int>("select last_insert_rowid()");
        //}

    }
}
