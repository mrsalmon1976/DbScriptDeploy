using DbScriptDeploy.BLL.Encoding;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.ViewModels.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.Services
{
    public interface IProjectViewService
    {
        IEnumerable<EnvironmentViewModel> LoadEnvironments(int projectId);

        IEnumerable<EnvironmentViewModel> LoadEnvironments(string projectId);

        IEnumerable<ScriptViewModel> LoadScripts(int projectId);
                                              
        IEnumerable<ScriptViewModel> LoadScripts(string projectId);

    }

    public class ProjectViewService : IProjectViewService
    {
        private readonly IEnvironmentRepository _environmentRepo;
        private readonly IScriptRepository _scriptRepo;
        private readonly IScriptTagRepository _scriptTagRepo;
        private readonly ILookupRepository _lookupRepo;
        private readonly IModelBinderService _modelBinderService;

        public ProjectViewService(IEnvironmentRepository environmentRepo, IScriptRepository scriptRepo, IScriptTagRepository scriptTagRepo, ILookupRepository lookupRepo, IModelBinderService modelBinderService)
        {
            _environmentRepo = environmentRepo;
            _scriptRepo = scriptRepo;
            _scriptTagRepo = scriptTagRepo;
            _lookupRepo = lookupRepo;
            _modelBinderService = modelBinderService;
        }

        public IEnumerable<EnvironmentViewModel> LoadEnvironments(int projectId)
        {
            var environments = _environmentRepo.GetAllByProjectId(projectId).Select(x => EnvironmentViewModel.FromEnvironmentModel(x)).ToList();
            var designations = _lookupRepo.GetAllDesignations();
            foreach (EnvironmentViewModel env in environments)
            {
                DesignationModel d = designations.Where(x => x.Id == env.DesignationId).SingleOrDefault();
                if (d != null) 
                {
                    env.DesignationName = d.Name;
                } 
            }
            return environments;
        }

        public IEnumerable<EnvironmentViewModel> LoadEnvironments(string projectId)
        {
            int pid = UrlUtility.DecodeNumber(projectId);
            return this.LoadEnvironments(pid);
        }

        public IEnumerable<ScriptViewModel> LoadScripts(int projectId)
        {
            var scripts = _scriptRepo.GetAllByProjectId(projectId);
            var scriptTags = _scriptTagRepo.GetByProjectId(projectId);

            List<ScriptViewModel> scriptViewModels = new List<ScriptViewModel>();

            // convert the scripts into viewmodel equivalents and decorate
            foreach (ScriptModel sm in scripts)
            {
                ScriptViewModel svm = _modelBinderService.BindScriptViewModel(sm); 
                svm.Tags.AddRange(scriptTags.Where(x => x.ScriptId == sm.Id).Select(y => ScriptTagViewModel.FromScriptModel(y)));
                scriptViewModels.Add(svm);
            }

            return scriptViewModels;
        }

        public IEnumerable<ScriptViewModel> LoadScripts(string projectId)
        {
            int pid = UrlUtility.DecodeNumber(projectId);
            return this.LoadScripts(pid);
        }
    }
}
