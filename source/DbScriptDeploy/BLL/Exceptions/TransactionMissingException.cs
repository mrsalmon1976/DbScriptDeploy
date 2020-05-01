using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.BLL.Exceptions
{
    public class TransactionMissingException : Exception
    {
        public TransactionMissingException() : base("No transaction found on underlying DbContext")
        {
        }
    }
}
