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
        ProjectModel GetById(int id);

        IEnumerable<ProjectModel> GetAllByUserId(Guid userId);

    }

    public class ProjectRepository : IProjectRepository
    {

        private readonly IDbContext _dbContext;

        public ProjectRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ProjectModel GetById(int id)
        {
            const string sql = "SELECT * FROM Project WHERE Id = @Id";
            return _dbContext.Query<ProjectModel>(sql, new { Id = id }).SingleOrDefault();
        }

        public IEnumerable<ProjectModel> GetAllByUserId(Guid userId)
        {
            // check if the user is a super admin
            string sql = "SELECT COUNT(*) FROM UserClaim WHERE UserId = @UserId AND Name = @Name";
            int count = _dbContext.ExecuteScalar<int>(sql, new { UserId = userId, Name = ClaimNames.Administrator });
            bool isAdmin = (count > 0);
            if (isAdmin)
            {
                // administrators get access to all projects
                sql = @"SELECT * FROM Project p ORDER BY p.Name";
            }
            else
            {
                sql = @"SELECT p.* 
                        FROM Project p 
                        INNER JOIN UserClaim uc ON p.Id = uc.ProjectId 
                        WHERE uc.UserId = @UserId
                        AND uc.ProjectId IS NOT NULL
                        ORDER BY p.Name";
            }
            return _dbContext.Query<ProjectModel>(sql, new { UserId = userId });
        }


    }
}
