using Microsoft.AspNetCore.Hosting;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy
{
    public class AppRootPathProvider : IRootPathProvider
    {
        private readonly IWebHostEnvironment _environment;

        public AppRootPathProvider(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public string GetRootPath()
        {
            return _environment.ContentRootPath;
        }
    }
}
