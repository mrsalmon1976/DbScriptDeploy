using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.Core.Encoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.ViewModels.Api
{
    public class EnvironmentViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ProjectId { get; set; }

        public string Host { get; set; }

        public Lookups.DatabaseType DbType { get; set; }

        public string Database { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public int DisplayOrder { get; set; }

        public int DesignationId { get; set; }

        public DateTime CreateDate { get; set; }

        public static EnvironmentViewModel FromEnvironmentModel(EnvironmentModel model)
        {
            return new EnvironmentViewModel()
            {
                Id = (model.Id <= 0 ? "" : UrlUtility.EncodeNumber(model.Id)),
                Name = model.Name,
                ProjectId = (model.ProjectId <= 0 ? "" : UrlUtility.EncodeNumber(model.ProjectId)),
                Host = model.Host,
                DbType = model.DbType,
                Database = model.Database,
                Port = model.Port,
                UserName = model.UserName,
                Password = model.Password,
                DisplayOrder = model.DisplayOrder,
                DesignationId = model.DesignationId,
                CreateDate = model.CreateDate
            };
        }

        public EnvironmentModel ToEnvironmentModel()
        {
            return new EnvironmentModel()
            {
                Id = (String.IsNullOrWhiteSpace(this.Id) ? 0 : UrlUtility.DecodeNumber(this.Id)),
                Name = this.Name,
                ProjectId = (String.IsNullOrWhiteSpace(this.ProjectId) ? 0 : UrlUtility.DecodeNumber(this.ProjectId)),
                Host = this.Host,
                DbType = this.DbType,
                Database = this.Database,
                Port = this.Port,
                UserName = this.UserName,
                Password = this.Password,
                DisplayOrder = this.DisplayOrder,
                DesignationId = this.DesignationId,
                CreateDate = this.CreateDate
            };
        }


    }
}
