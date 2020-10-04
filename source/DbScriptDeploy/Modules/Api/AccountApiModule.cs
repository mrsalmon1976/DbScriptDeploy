using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.BLL.Security;
using DbScriptDeploy.ViewModels.Api;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.Modules.Api
{
    public class AccountApiModule : NancyModule
    {
        public const string Route_Login_Post = "/api/account/login";

        private readonly IUserRepository _userRepo;
        private readonly IPasswordProvider _passwordProvider;

        public AccountApiModule(IUserRepository userRepo, IPasswordProvider passwordProvider)
        {
            _userRepo = userRepo;
            _passwordProvider = passwordProvider;

            Post(Route_Login_Post, x =>
            {
                return this.AccountLoginPost();
            });
        }

        public dynamic AccountLoginPost()
        {
            AccountLoginViewModel model = this.Bind<AccountLoginViewModel>();

            if ((!String.IsNullOrWhiteSpace(model.UserName)) && (!String.IsNullOrWhiteSpace(model.Password)))
            {
                // get the user
                UserModel user = _userRepo.GetByUserName(model.UserName);
                if (user != null && _passwordProvider.CheckPassword(model.Password, user.Password))
                {
                    return this.Login(user.Id, DateTime.Now.AddDays(30));
                }
            }

            // no user name/password supplied OR we have an invalid attempt to log in - either way, we say no
            string result = "Invalid user name or password";
            return this.Response.AsJson(result, HttpStatusCode.Unauthorized);

        }
    }
}
