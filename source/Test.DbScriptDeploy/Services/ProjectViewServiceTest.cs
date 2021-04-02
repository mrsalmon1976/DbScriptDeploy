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

namespace Test.DbScriptDeploy.Services
{
    [TestFixture]
    public class ProjectViewServiceTest
    {
        private IProjectViewService _projectViewService;

        private IEnvironmentRepository _environmentRepo;
        private IScriptRepository _scriptRepo;
        private IScriptExecutionRepository _scriptExecutionRepo;
        private IScriptTagRepository _scriptTagRepo;
        private ILookupRepository _lookupRepo;
        private IModelBinderService _modelBinderService;

        [SetUp]
        public void ProjectViewServiceTest_SetUp()
        {
            _environmentRepo = Substitute.For<IEnvironmentRepository>();
            _scriptRepo = Substitute.For<IScriptRepository>();
            _scriptTagRepo = Substitute.For<IScriptTagRepository>();
            _scriptExecutionRepo = Substitute.For<IScriptExecutionRepository>();
            _lookupRepo = Substitute.For<ILookupRepository>();
            _modelBinderService = Substitute.For<IModelBinderService>();

            _projectViewService = new ProjectViewService(_environmentRepo, _scriptRepo, _scriptTagRepo, _scriptExecutionRepo, _lookupRepo, _modelBinderService);
        }

        #region LoadEnvironments Tests

        [Test]
        public void LoadEnvironments_WithString_LoadsData()
        {
            int projectId = new Random().Next(10, 1000);

            EnvironmentModel em1 = DataHelper.CreateEnvironmentModel(projectId: projectId);
            EnvironmentModel em2 = DataHelper.CreateEnvironmentModel(projectId: projectId);
            List<EnvironmentModel> environments = new List<EnvironmentModel>(new [] { em1, em2 });
            _environmentRepo.GetAllByProjectId(projectId).Returns(environments);

            IEnumerable<EnvironmentViewModel> result = _projectViewService.LoadEnvironments(projectId);

            _environmentRepo.Received(1).GetAllByProjectId(projectId);

            Assert.AreEqual(2, result.Count());
            Assert.IsNotNull(result.Select(x => x.Id == UrlUtility.EncodeNumber(em1.Id)));
            Assert.IsNotNull(result.Select(x => x.Id == UrlUtility.EncodeNumber(em2.Id)));
        }

        /// <summary>
        /// Test to ensure that when environments are loaded, they are also decorated with the designation name
        /// </summary>
        [Test]
        public void LoadEnvironments_WithString_DecoratesDesignationName()
        {
            int projectId = new Random().Next(10, 1000);

            EnvironmentModel em1 = DataHelper.CreateEnvironmentModel(projectId: projectId);
            EnvironmentModel em2 = DataHelper.CreateEnvironmentModel(projectId: projectId);
            List<EnvironmentModel> environments = new List<EnvironmentModel>(new[] { em1, em2 });
            _environmentRepo.GetAllByProjectId(projectId).Returns(environments);

            // set up the designations
            DesignationModel dm1 = DataHelper.CreateDesignationModel(em1.DesignationId);
            DesignationModel dm2 = DataHelper.CreateDesignationModel(em2.DesignationId);
            List<DesignationModel> designations = new List<DesignationModel>(new[] { dm1, dm2 } );
            _lookupRepo.GetAllDesignations().Returns(designations);

            IEnumerable<EnvironmentViewModel> result = _projectViewService.LoadEnvironments(projectId);

            _environmentRepo.Received(1).GetAllByProjectId(projectId);

            Assert.AreEqual(2, result.Count());
            Assert.IsNotNull(result.Select(x => x.Id == UrlUtility.EncodeNumber(em1.Id) && x.DesignationName == dm1.Name));
            Assert.IsNotNull(result.Select(x => x.Id == UrlUtility.EncodeNumber(em2.Id) && x.DesignationName == dm2.Name));
        }

        [Test]
        public void LoadEnvironments_WithInteger_LoadsDataCorrectly()
        {
            int projectId = new Random().Next(10, 1000);
            string sProjectId = UrlUtility.EncodeNumber(projectId);

            _projectViewService.LoadEnvironments(sProjectId);

            _environmentRepo.Received(1).GetAllByProjectId(projectId);

        }

        #endregion

        #region LoadScripts Tests

        [Test]
        public void LoadScripts_WithString_LoadsData()
        {
            int projectId = new Random().Next(10, 1000);
            _projectViewService = new ProjectViewService(_environmentRepo, _scriptRepo, _scriptTagRepo, _scriptExecutionRepo, _lookupRepo, new ModelBinderService());

            ScriptModel sm1 = DataHelper.CreateScriptModel(projectId: projectId);
            ScriptModel sm2 = DataHelper.CreateScriptModel(projectId: projectId);
            List<ScriptModel> scripts = new List<ScriptModel>(new[] { sm1, sm2 });
            _scriptRepo.GetAllByProjectId(projectId).Returns(scripts);

            IEnumerable<ScriptViewModel> result = _projectViewService.LoadScripts(projectId);

            _scriptRepo.Received(1).GetAllByProjectId(projectId);

            Assert.AreEqual(2, result.Count());
            Assert.IsNotNull(result.Select(x => x.Id == UrlUtility.EncodeNumber(sm1.Id)));
            Assert.IsNotNull(result.Select(x => x.Id == UrlUtility.EncodeNumber(sm2.Id)));
        }

        [Test]
        public void LoadScripts_TagsExist_LoadsScriptsWithTags()
        {
            int projectId = new Random().Next(10, 1000);
            _projectViewService = new ProjectViewService(_environmentRepo, _scriptRepo, _scriptTagRepo, _scriptExecutionRepo, _lookupRepo, new ModelBinderService());

            ScriptModel sm1 = DataHelper.CreateScriptModel(projectId: projectId);
            ScriptModel sm2 = DataHelper.CreateScriptModel(projectId: projectId);
            List<ScriptModel> scripts = new List<ScriptModel>(new[] { sm1, sm2 });
            _scriptRepo.GetAllByProjectId(projectId).Returns(scripts);

            ScriptTagModel stm1_1 = DataHelper.CreateScriptTagModel(scriptId: sm1.Id);
            ScriptTagModel stm1_2 = DataHelper.CreateScriptTagModel(scriptId: sm1.Id);
            ScriptTagModel stm2_1 = DataHelper.CreateScriptTagModel(scriptId: sm2.Id);
            ScriptTagModel stm2_2 = DataHelper.CreateScriptTagModel(scriptId: sm2.Id);
            List<ScriptTagModel> scriptTags = new List<ScriptTagModel>(new[] { stm1_1, stm1_2, stm2_1, stm2_2 });
            _scriptTagRepo.GetByProjectId(projectId).Returns(scriptTags);


            List<ScriptViewModel> result = _projectViewService.LoadScripts(projectId).ToList();

            _scriptRepo.Received(1).GetAllByProjectId(projectId);
            _scriptTagRepo.Received(1).GetByProjectId(projectId);

            Assert.AreEqual(2, result.Count());

            ScriptViewModel svm1 = result[0];
            Assert.IsNotNull(svm1.Tags.Select(x => x.Id == UrlUtility.EncodeNumber(stm1_1.Id)));
            Assert.IsNotNull(svm1.Tags.Select(x => x.Id == UrlUtility.EncodeNumber(stm1_2.Id)));

            ScriptViewModel svm2 = result[0];
            Assert.IsNotNull(svm2.Tags.Select(x => x.Id == UrlUtility.EncodeNumber(stm2_1.Id)));
            Assert.IsNotNull(svm2.Tags.Select(x => x.Id == UrlUtility.EncodeNumber(stm2_2.Id)));
        }

        [Test]
        public void LoadScripts_WithInteger_LoadsDataCorrectly()
        {
            int projectId = new Random().Next(10, 1000);
            string sProjectId = UrlUtility.EncodeNumber(projectId);

            _projectViewService.LoadScripts(sProjectId);

            _scriptRepo.Received(1).GetAllByProjectId(projectId);

        }

        #endregion

    }
}
