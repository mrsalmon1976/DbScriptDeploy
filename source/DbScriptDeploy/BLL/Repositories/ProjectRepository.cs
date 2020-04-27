using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.BLL.Repositories
{
    public interface IProjectRepository
    {
        ProjectModel GetById(Guid id);

    }

    public class ProjectRepository : IProjectRepository
    {

        private IDbContext _dbContext;

        public ProjectRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ProjectModel GetById(Guid id)
        {
            const string sql = "SELECT * FROM Project WHERE Id = @Id";
            return _dbContext.Query<ProjectModel>(sql, new { Id = id }).SingleOrDefault();
        }

    }
}
