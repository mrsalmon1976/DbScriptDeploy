using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.Core.Encoding
{
    public class UrlEncoder
    {
        public static readonly string Alphabet = "abcdefghijklmnopqrstuvwxyz0123456789";
        public static readonly int Base = Alphabet.Length;

        public static string EncodeToAlphaNumericValue(int value)
        {
            if (value == 0) return Alphabet[0].ToString();

            var s = string.Empty;

            while (value > 0)
            {
                s += Alphabet[value % Base];
                value = value / Base;
            }

            return string.Join(string.Empty, s.Reverse());
        }

        public static int DecodeFromAlphaNumericValue(string value)
        {
            var i = 0;

            foreach (var c in value)
            {
                i = (i * Base) + Alphabet.IndexOf(c);
            }

            return i;
        }
    }
}
