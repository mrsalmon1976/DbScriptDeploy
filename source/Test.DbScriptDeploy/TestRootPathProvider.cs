using Nancy;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.DbScriptDeploy
{
    public class TestRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            return TestContext.CurrentContext.TestDirectory;
        }

    }
}
