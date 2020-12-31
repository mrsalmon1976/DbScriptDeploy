﻿using DbScriptDeploy.BLL.Data;
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

        public static DesignationModel CreateDesignationModel()
        {
            DesignationModel model = new DesignationModel();
            model.Id = Guid.NewGuid();
            model.Name = "TestDesignation";
            model.CreateDate = DateTime.UtcNow;
            return model;
        }

        public static EnvironmentModel CreateEnvironmentModel()
        {
            Random r = new Random();
            EnvironmentModel model = new EnvironmentModel();
            model.DatabaseName = "MyFakeDb";
            model.DbType = Lookups.DatabaseType.SqlServer;
            model.DisplayOrder = 1;
            model.HostName = "MyFakeServer";
            model.Password = "password";
            model.Port = 9876;
            model.ProjectId = r.Next(1, 1000);
            model.UserName = "Eric";
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
