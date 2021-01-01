using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.BLL
{
    public static class DomainConstants
    {
        public static byte[] EncryptionSalt = new byte[10] { 213, 25, 44, 43, 164, 180, 115, 128, 74, 94 };
    }
}
