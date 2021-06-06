using AutoMapper;
using DbScriptDeploy.BLL.Commands;
using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.Console.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.Console.Areas.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly ILogger<ProjectController> _logger;
        private readonly IDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IProjectCreateCommand _projectCreateCommand;

        public ProjectController(ILogger<ProjectController> logger, IDbContext dbContext, IMapper mapper, IProjectCreateCommand projectCreateCommand)
        {
            _logger = logger;
            _dbContext = dbContext;
            _mapper = mapper;
            _projectCreateCommand = projectCreateCommand;
        }

        [HttpGet]
        [Route("/api/project/user")]
        public IEnumerable<ProjectViewModel> GetUserProjects()
        {
            ProjectViewModel eg = new ProjectViewModel();
            eg.Id = "HGHGHG";
            eg.Name = "EXAMPLE";
            return new List<ProjectViewModel>(new[] { eg });
            //UserPrincipal userPrincipal = this.Context.CurrentUser as UserPrincipal;
            //List<ProjectModel> projects = _projectRepo.GetAllByUserId(userPrincipal.UserId).ToList();
            //IEnumerable<ProjectViewModel> result = projects.Select(x => ProjectViewModel.FromProjectModel(x));
            //return this.Response.AsJson(result);

        }

        /// <summary>
        /// Create a new project
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        [Route("/api/project")]
        public ProjectViewModel PostProject([FromBody] ProjectViewModel projectViewModel)
        {
            _logger.LogDebug("Project received via /api/project");

            //var projectName = Request.Form.ProjectName;
            _dbContext.BeginTransaction();
            ProjectModel project = _projectCreateCommand.Execute(projectViewModel.Name);
            _dbContext.Commit();
            _logger.LogInformation($"Project {project.Id}/{project.Name} created");

            var result = _mapper.Map<ProjectViewModel>(project);
            return result;

        }

    }
}
