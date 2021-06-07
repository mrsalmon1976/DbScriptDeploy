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
    public interface IProjectRepository
    {
        IEnumerable<ProjectModel> GetAll();

        ProjectModel GetById(int id);


    }

    public class ProjectRepository : IProjectRepository
    {

        private readonly IDbContext _dbContext;

        public ProjectRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<ProjectModel> GetAll()
        {
            const string sql = @"SELECT * FROM Project p ORDER BY p.Name COLLATE NOCASE";
            return _dbContext.Query<ProjectModel>(sql);
        }

        public ProjectModel GetById(int id)
        {
            const string sql = "SELECT * FROM Project WHERE Id = @Id";
            return _dbContext.Query<ProjectModel>(sql, new { Id = id }).SingleOrDefault();
        }




    }
}
