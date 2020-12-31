using DbScriptDeploy.Core.Encoding;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.DbScriptDeploy.Core.Encoding
{
    [TestFixture]
    public class UrlUtilityTest
    {

        [Test]
        public void EncodeDecodeCheck()
        {
            Random r = new Random();
            for (int i=0; i<5; i++)
            {
                var n = r.Next(1, 10000);
                n = 1;
                var s = UrlUtility.EncodeNumber(n);
                var decoded = UrlUtility.DecodeNumber(s);
                Assert.Greater(s.Trim().Length, 3);
                Assert.AreEqual(n, decoded);
            }
        }
    }
}
