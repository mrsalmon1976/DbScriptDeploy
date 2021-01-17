using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.BLL.Security;
using DbScriptDeploy.Modules.Api;
using DbScriptDeploy.Security;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Responses.Negotiation;
using Nancy.Testing;
//using Nancy.Testing;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Test.DbScriptDeploy.Modules.Api
{
    [TestFixture]
    public class AccountApiModuleTest
    {
        private IUserRepository _userRepo;
        private IPasswordProvider _passwordProvider;

        [SetUp]
        public void AccountApiModuleTest_SetUp()
        {
            _passwordProvider = Substitute.For<IPasswordProvider>();
            _userRepo = Substitute.For<IUserRepository>();
        }

        /*

        #region LoginGet Tests

        [Test]
        public void LoginGet_UserLoggedIn_RedirectsToDashboard()
        {
            // setup
            var currentUser = new UserIdentity() { Id = Guid.NewGuid(), UserName = "Joe Soap" };
            var browser = CreateBrowser(currentUser);

            // execute
            var response = browser.Get(Actions.Login.Default, (with) =>
            {
                with.HttpRequest();
                with.FormsAuth(currentUser.Id, new Nancy.Authentication.Forms.FormsAuthenticationConfiguration());
            });

            // assert
            response.ShouldHaveRedirectedTo(Actions.Dashboard.Default);
        }

        [Test]
        public void LoginGet_NoReturnUrl_DefaultsToDashboard()
        {
            // setup
            var browser = CreateBrowser(null);

            // execute
            var response = browser.Get(Actions.Login.Default, (with) =>
            {
                with.HttpRequest();
                //with.FormsAuth(Guid.NewGuid(), new Nancy.Authentication.Forms.FormsAuthenticationConfiguration());
            });

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            response.Body["#returnurl"]
                .ShouldExistOnce()
                .And.ShouldContainAttribute("value", Actions.Dashboard.Default);
        }

        [Test]
        public void LoginGet_WithReturnUrl_SetsReturnUrlFormValue()
        {
            // setup
            var browser = CreateBrowser(null);

            // execute
            var response = browser.Get(Actions.Login.Default, (with) =>
            {
                with.HttpRequest();
                with.Query("returnurl", "/test");
                //with.FormsAuth(Guid.NewGuid(), new Nancy.Authentication.Forms.FormsAuthenticationConfiguration());
            });

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            response.Body["#returnurl"]
                .ShouldExistOnce()
                .And.ShouldContainAttribute("value", "/test");
        }

        #endregion

        */

        #region LoginPost Tests

        [Test]
        public void LoginPost_NoUserName_LoginFailsWithoutCheck()
        {
            // setup
            var browser = CreateBrowser(null);

            // execute
            var response = browser.Post(AccountApiModule.Route_Login_Post, (with) =>
            {
                with.HttpRequest();
                with.FormValue("Password", "password");
            }).Result;

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            _userRepo.DidNotReceive().GetByUserName(Arg.Any<String>());
            _passwordProvider.DidNotReceive().CheckPassword(Arg.Any<string>(), Arg.Any<string>());

            string result = JsonConvert.DeserializeObject<string>(response.Body.AsString());
            Assert.IsTrue(result.Contains("Invalid"));
        }

        [Test]
        public void LoginPost_NoPassword_LoginFailsWithoutCheck()
        {
            // setup
            var browser = CreateBrowser(null);

            // execute
            var response = browser.Post(AccountApiModule.Route_Login_Post, (with) =>
            {
                with.HttpRequest();
                with.FormValue("UserName", "admin");
            }).Result;

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            _userRepo.DidNotReceive().GetByUserName(Arg.Any<String>());
            _passwordProvider.DidNotReceive().CheckPassword(Arg.Any<string>(), Arg.Any<string>());

            string result = JsonConvert.DeserializeObject<string>(response.Body.AsString());
            Assert.IsTrue(result.Contains("Invalid"));
        }

        [Test]
        public void LoginPost_UserNotFound_LoginFails()
        {
            // setup
            var browser = CreateBrowser(null);
            const string userName = "admin";

            // execute
            var response = browser.Post(AccountApiModule.Route_Login_Post, (with) =>
            {
                with.HttpRequest();
                with.FormValue("UserName", userName);
                with.FormValue("Password", "password");
            }).Result;

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            _userRepo.Received(1).GetByUserName(userName);
            _passwordProvider.DidNotReceive().CheckPassword(Arg.Any<string>(), Arg.Any<string>());

            string result = JsonConvert.DeserializeObject<string>(response.Body.AsString());
            Assert.IsTrue(result.Contains("Invalid"));
        }

        [Test]
        public void LoginPost_UserFoundButPasswordIncorrect_LoginFails()
        {
            const string userName = "admin";
            const string password = "password";
                
            // setup
            UserModel savedUser = new UserModel()
            {
                Id = Guid.NewGuid(),
                UserName = userName,
                Password = "hashedpassword"
            };
            _userRepo.GetByUserName(userName).Returns(savedUser);
            _passwordProvider.CheckPassword(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

            var browser = CreateBrowser(null);

            // execute
            var response = browser.Post(AccountApiModule.Route_Login_Post, (with) =>
            {
                with.HttpRequest();
                with.FormValue("UserName", userName);
                with.FormValue("Password", password);
            }).Result;

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            _passwordProvider.Received(1).CheckPassword(password, savedUser.Password);

            _userRepo.Received(1).GetByUserName(savedUser.UserName);
            _passwordProvider.Received(1).CheckPassword(password, savedUser.Password);

            string result = JsonConvert.DeserializeObject<string>(response.Body.AsString());
            Assert.IsTrue(result.Contains("Invalid"));
        }

        [Test]
        public void LoginPost_ValidLogin_LoginSucceeds()
        {
            const string userName = "admin";
            const string password = "password";

            // setup
            UserModel savedUser = new UserModel()
            {
                Id = Guid.NewGuid(),
                UserName = userName,
                Password = "hashedpassword"
            };
            _userRepo.GetByUserName(userName).Returns(savedUser);

            _passwordProvider.CheckPassword(password, savedUser.Password).Returns(true);

            var browser = new Browser((bootstrapper) =>
                            bootstrapper.Module(new AccountApiModule(_userRepo, _passwordProvider))
                                .RootPathProvider(new TestRootPathProvider())
                                .RequestStartup((container, pipelines, context) =>
                                {
                                    container.Register<IUserRepository>(Substitute.For<IUserRepository>());
                                    container.Register<IUserMapper, UserMapper>();
                                    var formsAuthConfiguration = new FormsAuthenticationConfiguration()
                                    {
                                        RedirectUrl = "~/login",
                                        UserMapper = container.Resolve<IUserMapper>(),
                                    };
                                    FormsAuthentication.Enable(pipelines, formsAuthConfiguration);
                                })
                            );

            // execute
            var response = browser.Post(AccountApiModule.Route_Login_Post, (with) =>
            {
                with.HttpRequest();
                with.FormValue("UserName", userName);
                with.FormValue("Password", password);
            }).Result;

            // assert
            Assert.AreEqual(HttpStatusCode.SeeOther, response.StatusCode);
            Assert.IsNotNull(response.Headers["Location"]);
            Assert.IsNotEmpty(response.Headers["Location"]);
            Assert.IsEmpty(response.Body.AsString());
            _passwordProvider.Received(1).CheckPassword("password", savedUser.Password);
            _userRepo.Received(1).GetByUserName(savedUser.UserName);
        }

        #endregion

        #region Private Methods

        private Browser CreateBrowser(ClaimsPrincipal currentUser)
        {
            var browser = new Browser((bootstrapper) =>
                            bootstrapper.Module(new AccountApiModule(_userRepo, _passwordProvider))
                                .RootPathProvider(new TestRootPathProvider())
                                .RequestStartup((container, pipelines, context) => {
                                    context.CurrentUser = currentUser;
                                })
                            );
            return browser;
        }

        #endregion
    }
}
