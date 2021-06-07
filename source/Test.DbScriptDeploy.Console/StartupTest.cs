using DbScriptDeploy.Console;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Test.DbScriptDeploy.Console
{
    [TestFixture]
    public class StartupTest
    {
        [Test]
        public async Task LoginRequest_Integration()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appSettings.test.json").Build();

            TestServer server = new TestServer(new WebHostBuilder()
                .UseConfiguration(config)
                .UseEnvironment("Development")
                .UseStartup<Startup>());
            HttpClient client = server.CreateClient();

            var response = await client.GetAsync("/Identity/Account/RegisterAdmin");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
        }
    }
}
