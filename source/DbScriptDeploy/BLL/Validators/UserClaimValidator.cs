using DbScriptDeploy.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbScriptDeploy.BLL.Repositories;

namespace DbScriptDeploy.BLL.Validators
{
    public interface IUserClaimValidator
    {
        ValidationResult Validate(UserClaimModel model);
    }

    public class UserClaimValidator : IUserClaimValidator
    {
        private IUserClaimRepository _userClaimRepo;

        public UserClaimValidator(IUserClaimRepository userClaimRepo)
        {
            this._userClaimRepo = userClaimRepo;
        }

        public ValidationResult Validate(UserClaimModel model)
        {
            ValidationResult result = new ValidationResult();

            if (model.UserId == Guid.Empty)
            {
                result.Messages.Add("User id must be a valid user value");
            }
            if (String.IsNullOrWhiteSpace(model.Name))
            {
                result.Messages.Add("Claim name cannot be empty");
            }
            IEnumerable<UserClaimModel> userClaims = _userClaimRepo.GetByUserId(model.UserId);
            UserClaimModel userClaim = userClaims.FirstOrDefault(x => x.Name == model.Name && x.ProjectId == model.ProjectId);
            if (userClaim != null) 
            {
                result.Messages.Add("The claim is already assigned to this user.");
            }

            return result;
        }
    }
}
