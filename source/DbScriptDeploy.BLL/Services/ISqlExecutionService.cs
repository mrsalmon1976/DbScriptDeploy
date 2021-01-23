using DbScriptDeploy.BLL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbScriptDeploy.BLL.Services
{
    public interface ISqlExecutionService
    {
        IEnumerable<ParseError> Parse(string sql);
    }
}
