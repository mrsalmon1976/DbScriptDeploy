using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.BLL.Repositories
{
    public interface IUserClaimRepository
    {
        IEnumerable<UserClaimModel> GetByUserId(Guid userId);

    }

    public class UserClaimRepository : IUserClaimRepository
    {

        private IDbContext _dbContext;

        public UserClaimRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<UserClaimModel> GetByUserId(Guid userId)
        {
            const string sql = "SELECT * FROM UserClaim WHERE UserId = @UserId";
            return _dbContext.Query<UserClaimModel>(sql, new { UserId = userId });
        }
    }
}
