using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.BLL.Services;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.DbScriptDeploy.BLL.Services
{
    [TestFixture]
    public class TSqlExecutionServiceTest
    {
        private TSqlExecutionService _tsqlExecutionService;

        [SetUp]
        public void TSqlExecutionServiceTest_SetUp()
        {
            _tsqlExecutionService = new TSqlExecutionService();
        }

        [TestCase("select * from MyTable here x > 100")]
        [TestCase("Select * rom MyTable where x > 100")]
        [TestCase("elect * from MyTable where x > 100")]
        public void Parse_InvalidSql_ReturnsErrors(string sql)
        {
            var result = _tsqlExecutionService.Parse(sql);

            Assert.Greater(result.Count(), 0);
        }

        [TestCase("select * from MyTable where x > 100")]
        [TestCase(@"select * from MyTable where x > 100
                    GO
                    select * from MyTable2 where y = 1000
                    GO")]
        public void Parse_ValidSql_ReturnsNoErrors(string sql)
        {
            var result = _tsqlExecutionService.Parse(sql);

            Assert.AreEqual(0, result.Count());
        }

    }
}
