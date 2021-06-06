using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Security;
using DbScriptDeploy.BLL.Services.Communications;
using DbScriptDeploy.Console.Data;
using DbScriptDeploy.Console.Properties;
using DbScriptDeploy.Console.Services.Communication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DbScriptDeploy.BLL.Commands;
using DbScriptDeploy.BLL.Validators;

namespace DbScriptDeploy.Console
{
    public class Startup
    {

        private string _appRoot = "";

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _appRoot = env.ContentRootPath;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // initialise app settings - some of this is pulled from the user secrets file
            IEmailSettings emailSettings = this.Configuration.GetSection("EmailSettings").Get<EmailSettings>();
            IAppSettings appSettings = this.Configuration.GetSection("AppSettings").Get<AppSettings>();
            appSettings.EmailSettings = emailSettings;
            services.AddSingleton<IAppSettings>(appSettings);

            // configure the database
            ConfigureDatabase(services);

            // configure identity
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddScoped<IEmailSender, EmailSender>();

            // automapper
            services.AddAutoMapper(typeof(AppMappingProfile));

            // validators
            services.AddScoped<IProjectValidator, ProjectValidator>();

            // BLL services
            services.AddScoped<IEmailService, SendGridEmailService>();

            // BLL commands
            services.AddScoped<IProjectCreateCommand, ProjectCreateCommand>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RoleManager<IdentityRole> roleManager)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            ConfigureRoles(roleManager);

        }

        private void ConfigureDatabase(IServiceCollection services)
        {
            //var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            //    .UseSqlite(Configuration.GetConnectionString("DefaultConnection"))
            //    .Options;

            services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            //using (var context = new ApplicationDbContext(contextOptions))
            //{
            //    context.Database.EnsureCreated();
            //    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context), null);

            //    //context.Database.Migrate();
            //}

            // Initialise the database only on application start
            string dbPath = Path.Combine(_appRoot, "db", "DbScriptDeploy.db");

            IDbContextFactory dbContextFactory = new DbContextFactory(dbPath);
            services.AddSingleton<IDbContextFactory>(dbContextFactory);
            services.AddScoped<IDbContext>((sp) => dbContextFactory.GetDbContext());

            // initialise the database
            using (IDbContext dbc = dbContextFactory.GetDbContext())
            {
                dbc.Initialise();
            }



        }

        private void ConfigureRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync(Roles.Administrator).Result)
            {
                roleManager.CreateAsync(new IdentityRole(Roles.Administrator));
            }

        }
    }
}
