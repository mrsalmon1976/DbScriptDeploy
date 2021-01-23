using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DbScriptDeploy.BLL.Services
{
    public class TSqlExecutionService : ISqlExecutionService
    {
        public IEnumerable<DbScriptDeploy.BLL.Models.ParseError> Parse(string sql)
        {
            TSqlParser parser = new TSql150Parser(true, SqlEngineType.All);
            IList<ParseError> errors;
            using (StringReader reader = new StringReader(sql))
            {
                parser.Parse(reader, out errors);
            }
            return errors.Select(x => new DbScriptDeploy.BLL.Models.ParseError(x.Message, x.Number, x.Offset, x.Line, x.Column));
            
        }
    }
}
