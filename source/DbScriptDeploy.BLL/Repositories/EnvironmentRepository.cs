using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.BLL.Repositories
{
    public interface IEnvironmentRepository
    {
        EnvironmentModel GetById(int id);

        IEnumerable<EnvironmentModel> GetAllByProjectId(int projectId);

    }

    public class EnvironmentRepository : IEnvironmentRepository
    {

        private readonly IDbContext _dbContext;

        public EnvironmentRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public EnvironmentModel GetById(int id)
        {
            const string sql = "SELECT * FROM Environment WHERE Id = @Id";
            return _dbContext.Query<EnvironmentModel>(sql, new { Id = id }).SingleOrDefault();
        }

        public IEnumerable<EnvironmentModel> GetAllByProjectId(int projectId)
        {
           
            string sql = @"SELECT e.* 
                    FROM Environment e 
                    WHERE e.ProjectId = @ProjectId
                    ORDER BY e.Name";
            return _dbContext.Query<EnvironmentModel>(sql, new { ProjectId = projectId });
        }


    }
}
