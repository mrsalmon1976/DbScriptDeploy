using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.BLL.Encoding
{
    public class UrlUtility
    {
        public static string EncodeNumber(int i)
        {
            return Microsoft.AspNetCore.WebUtilities.Base64UrlTextEncoder.Encode(BitConverter.GetBytes(i));
        }

        public static int DecodeNumber(string s)
        {
            return BitConverter.ToInt32(Microsoft.AspNetCore.WebUtilities.Base64UrlTextEncoder.Decode(s));
        }

    }
}
