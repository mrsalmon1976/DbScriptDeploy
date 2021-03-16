using DbScriptDeploy.BLL.Commands;
using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.BLL.Security;
using DbScriptDeploy.BLL.Encoding;
using DbScriptDeploy.Modules.Api;
using DbScriptDeploy.Security;
using DbScriptDeploy.ViewModels.Api;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Responses.Negotiation;
using Nancy.Testing;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using DbScriptDeploy.Services;

namespace Test.DbScriptDeploy.Modules.Api
{
    [TestFixture]
    public class ProjectApiModuleTest
    {
        private IDbContext _dbContext;
        private IProjectRepository _projectRepo;
        private IProjectCreateCommand _projectCreateCommand;
        private IScriptCreateCommand _scriptCreateCommand;
        private IProjectViewService _projectViewService;

        [SetUp]
        public void ProjectApiModuleTest_SetUp()
        {
            _dbContext = Substitute.For<IDbContext>();
            _projectRepo = Substitute.For<IProjectRepository>();
            _projectCreateCommand = Substitute.For<IProjectCreateCommand>();
            _scriptCreateCommand = Substitute.For<IScriptCreateCommand>();
            _projectViewService = Substitute.For<IProjectViewService>();
        }

        #region AddProject Tests

        [Test]
        public void AddProject_ValidData_ReturnsProjectData()
        {
            // setup
            var currentUser = new ClaimsPrincipal(new GenericPrincipal(new GenericIdentity("Joe Soap"), new string[] { }));
            var browser = CreateBrowser(currentUser);
            var user = DataHelper.CreateUserModel();


            ProjectModel project = DataHelper.CreateProjectModel();
            _projectCreateCommand.Execute(project.Name).Returns(project);

            // execute
            var response = browser.Post(ProjectApiModule.Route_Post_AddProject, (with) =>
            {
                with.HttpRequest();
                with.FormValue("projectName", project.Name);
                with.FormsAuth(user.Id, new FormsAuthenticationConfiguration());
            }).Result;

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            _projectCreateCommand.Received(1).Execute(project.Name);

            ProjectViewModel result = JsonConvert.DeserializeObject<ProjectViewModel>(response.Body.AsString());
            string safeProjectId = UrlUtility.EncodeNumber(project.Id);
            Assert.AreEqual(safeProjectId, result.Id);

            _dbContext.Received(1).BeginTransaction();
            _dbContext.Received(1).Commit();
        }


        #endregion

        #region AddScript Tests

        [Test]
        public void AddScript_ValidData_ReturnsProjectData()
        {
            // setup
            var currentUser = new ClaimsPrincipal(new GenericPrincipal(new GenericIdentity("Joe Soap"), new string[] { }));
            var browser = CreateBrowser(currentUser);
            var user = DataHelper.CreateUserModel();


            ScriptModel scriptModel = DataHelper.CreateScriptModel();
            _scriptCreateCommand.Execute(Arg.Any<ScriptModel>()).Returns(scriptModel);

            ScriptViewModel scriptViewModel = ScriptViewModel.FromScriptModel(scriptModel);

            // execute
            var response = browser.Post(ProjectApiModule.Route_Post_Project_Script.Replace("{projectId}", scriptViewModel.ProjectId) , (with) =>
            {
                with.HttpRequest();
                with.JsonBody<ScriptViewModel>(scriptViewModel);
                with.Header("Content-Type", "application/json; charset=utf-8");
                with.FormsAuth(user.Id, new FormsAuthenticationConfiguration());
            }).Result;

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            _scriptCreateCommand.Received(1).Execute(Arg.Any<ScriptModel>());

            ScriptViewModel result = JsonConvert.DeserializeObject<ScriptViewModel>(response.Body.AsString());
            Assert.AreEqual(scriptViewModel.ProjectId, result.ProjectId);

            _dbContext.Received(1).BeginTransaction();
            _dbContext.Received(1).Commit();
        }


        #endregion

        #region LoadUserProjects Tests

        [Test]
        public void LoadUserProjects_LoadsDataForCurrentUser()
        {
            // setup
            Guid userId = Guid.NewGuid();
            var currentUser = new UserPrincipal(userId, new GenericIdentity("Joe Soap"));
            var browser = CreateBrowser(currentUser);

            List<ProjectModel> userProjects = new List<ProjectModel>();
            ProjectModel project1 = DataHelper.CreateProjectModel();
            userProjects.Add(project1);
            userProjects.Add(DataHelper.CreateProjectModel());
            userProjects.Add(DataHelper.CreateProjectModel());
            _projectRepo.GetAllByUserId(userId).Returns(userProjects);

            // execute
            var response = browser.Get(ProjectApiModule.Route_Get_Api_Projects_User, (with) =>
            {
                with.HttpRequest();
                with.FormsAuth(userId, new FormsAuthenticationConfiguration());
            }).Result;

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            _projectRepo.Received(1).GetAllByUserId(userId);

            List<ProjectViewModel> result = JsonConvert.DeserializeObject<List<ProjectViewModel>>(response.Body.AsString());
            Assert.AreEqual(userProjects.Count, result.Count);

            ProjectViewModel pvm = result.First();
            Assert.AreEqual(project1.Id, UrlUtility.DecodeNumber(pvm.Id));
            Assert.AreEqual(project1.Name, pvm.Name);
        }


        #endregion

        #region LoadProjectScripts Tests

        [Test]
        public void LoadProjectScripts_LoadsData()
        {
            // setup
            Guid userId = Guid.NewGuid();
            int projectId = new Random().Next(10, 1000);
            string sProjectId = UrlUtility.EncodeNumber(projectId);
            var currentUser = new UserPrincipal(userId, new GenericIdentity("Joe Soap"));
            var browser = CreateBrowser(currentUser);

            List<ScriptViewModel> scripts = new List<ScriptViewModel>();
            scripts.Add(ScriptViewModel.FromScriptModel(DataHelper.CreateScriptModel(id: 111, projectId)));
            scripts.Add(ScriptViewModel.FromScriptModel(DataHelper.CreateScriptModel(id: 222, projectId)));
            scripts.Add(ScriptViewModel.FromScriptModel(DataHelper.CreateScriptModel(id: 333, projectId)));
            _projectViewService.LoadScripts(sProjectId).Returns(scripts);

            // execute
            var response = browser.Get(ProjectApiModule.Route_Get_ProjectScripts.Replace("{id}", sProjectId), (with) =>
            {
                with.HttpRequest();
                with.FormsAuth(userId, new FormsAuthenticationConfiguration());
            }).Result;

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            _projectViewService.Received(1).LoadScripts(sProjectId);

            List<ScriptViewModel> result = JsonConvert.DeserializeObject<List<ScriptViewModel>>(response.Body.AsString());
            Assert.AreEqual(scripts.Count, result.Count);

            ScriptViewModel svm = result.First();
            Assert.AreEqual(scripts[0].Id, svm.Id);
            Assert.AreEqual(scripts[0].Name, svm.Name);
        }

        #endregion

        #region Private Methods

        private Browser CreateBrowser(ClaimsPrincipal currentUser)
        {

            var browser = new Browser((bootstrapper) =>
                            bootstrapper.Module(new ProjectApiModule(_dbContext, _projectRepo, _projectViewService, _projectCreateCommand, _scriptCreateCommand))
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
