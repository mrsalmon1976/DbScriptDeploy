using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Owin;

namespace DbScriptDeploy
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            var appSettingsSection = _config.GetSection("AppSettings");
            IAppSettings settings = new AppSettings();
            _config.GetSection("AppSettings").Bind(settings);

            //app.UseOwin(x => x.UseNancy());
            app.UseOwin(x => x.UseNancy(options => options.Bootstrapper = new AppBootStrapper(env, settings)));


        }
    }
}
