using AutoMapper;
using DbScriptDeploy.BLL.Commands;
using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Encoding;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.Console;
using DbScriptDeploy.Console.AppCode.Security;
using DbScriptDeploy.Console.Areas.Api;
using DbScriptDeploy.Console.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Test.DbScriptDeploy.Console;

namespace Test.DbScriptDeploy.Modules.Api
{
    [TestFixture]
    public class ProjectControllerTest
    {
        private ILogger<ProjectController> _logger;
        private IDbContext _dbContext;
        private IMapper _mapper;
        private IProjectCreateCommand _projectCreateCommand;
        private IProjectRepository _projectRepo;
        //private IScriptCreateCommand _scriptCreateCommand;
        //private IProjectViewService _projectViewService;
        //private IModelBinderService _modelBinderService;

        private ProjectController _projectController;

        [SetUp]
        public void ProjectControllerTest_SetUp()
        {
            var mapperConfig = new MapperConfiguration(cfg => {
                cfg.AddProfile<AppMappingProfile>();
            });

            _logger = Substitute.For<ILogger<ProjectController>>();
            _dbContext = Substitute.For<IDbContext>();
            _mapper = new Mapper(mapperConfig);
            _projectCreateCommand = Substitute.For<IProjectCreateCommand>();
            _projectRepo = Substitute.For<IProjectRepository>();
            //_scriptCreateCommand = Substitute.For<IScriptCreateCommand>();
            //_projectViewService = Substitute.For<IProjectViewService>();
            //_modelBinderService = Substitute.For<IModelBinderService>();

            _projectController = new ProjectController(_logger, _dbContext, _mapper, _projectCreateCommand, _projectRepo);
        }

        #region GetUserProjects

        [Test]
        public void GetUserProjects_IsAdmin_GetsAllProjects()
        {
            // setup
            IAppUser appUser = Substitute.For<IAppUser>();
            appUser.IsAdmin.Returns(true);
            _projectController.CurrentUser = appUser;


            int projectId = new Random().Next(1, 1000);
            List < ProjectModel> projects = new List<ProjectModel>();
            projects.Add(new ProjectModel() { Id = projectId }); ;
            _projectRepo.GetAll().Returns(projects);
            //List<ProjectViewModel> projectViewModels = new List<ProjectViewModel>();
            //projectViewModels.Add(new ProjectViewModel() { Id = Guid.NewGuid().ToString() });;
            //_mapper.Map<IEnumerable<>

            // execute
            List<ProjectViewModel> result = _projectController.GetUserProjects().ToList();

            // assert
            _projectRepo.Received(1).GetAll();

            Assert.AreEqual(result[0].Id, UrlUtility.EncodeNumber(projectId));

        }

        #endregion

        #region PostProject Tests

        [Test]
        public void PostProject_ValidData_ReturnsProjectData()
        {
            // setup
            ProjectModel project = TestDataHelper.CreateProjectModel();
            _projectCreateCommand.Execute(project.Name).Returns(project);

            // execute
            ProjectViewModel projectViewModel = new ProjectViewModel();
            projectViewModel.Name = project.Name;
            ProjectViewModel result = _projectController.PostProject(projectViewModel);

            // assert
            _projectCreateCommand.Received(1).Execute(project.Name);

            string safeProjectId = UrlUtility.EncodeNumber(project.Id);
            Assert.AreEqual(safeProjectId, result.Id);

            _dbContext.Received(1).BeginTransaction();
            _dbContext.Received(1).Commit();
        }


        #endregion

        /*
        #region AddScript Tests

        [Test]
        public void AddScript_ValidData_ReturnsProjectData()
        {
            // setup
            var currentUser = new ClaimsPrincipal(new GenericPrincipal(new GenericIdentity("Joe Soap"), new string[] { }));
            var browser = CreateBrowser(currentUser);
            var user = DataHelper.CreateUserModel();
            var modelBinder = new ModelBinderService();


            ScriptModel scriptModel = DataHelper.CreateScriptModel();
            _scriptCreateCommand.Execute(Arg.Any<ScriptModel>()).Returns(scriptModel);

            ScriptViewModel scriptViewModel = modelBinder.BindScriptViewModel(scriptModel);
            _modelBinderService.BindScriptViewModel(Arg.Any<ScriptModel>()).Returns(scriptViewModel);

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
            var modelBinder = new ModelBinderService();

            List<ScriptViewModel> scripts = new List<ScriptViewModel>();
            scripts.Add(modelBinder.BindScriptViewModel(DataHelper.CreateScriptModel(id: 111, projectId)));
            scripts.Add(modelBinder.BindScriptViewModel(DataHelper.CreateScriptModel(id: 222, projectId)));
            scripts.Add(modelBinder.BindScriptViewModel(DataHelper.CreateScriptModel(id: 333, projectId)));
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
                            bootstrapper.Module(new ProjectApiModule(_dbContext, _projectRepo, _projectViewService, _modelBinderService, _projectCreateCommand, _scriptCreateCommand))
                                .RootPathProvider(new TestRootPathProvider())
                                .RequestStartup((container, pipelines, context) => {
                                    context.CurrentUser = currentUser;
                                })
                            );
            return browser;
        }

        #endregion
        */
    }
}
