using DbScriptDeploy.BLL.Encoding;
using DbScriptDeploy.ViewModels.Api;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.DbScriptDeploy.ViewModels.Api
{
    [TestFixture]
    public class ScriptTagViewModelTest
    {
        #region LoadEnvironments Tests

        /// <summary>
        /// This test just checks that, using random numbers, we don't get issues getting a css class
        /// </summary>
        [Test]
        public void CssClasses_GivenRandomInt_ReturnsCssClass()
        {
            Random r = new Random();
            string cssClass = "";
            int scriptTagId = 0;
            ScriptTagViewModel model = null;

            for (int i = 0; i < 1000; i++)
            {
                scriptTagId = r.Next(1, 1000000);
                model = new ScriptTagViewModel();
                model.Id = UrlUtility.EncodeNumber(scriptTagId);

                cssClass = model.CssClass;
                Assert.IsNotNull(cssClass);
                Assert.IsNotEmpty(cssClass);
            }

            scriptTagId = 1;
            model = new ScriptTagViewModel();
            model.Id = UrlUtility.EncodeNumber(scriptTagId);
            cssClass = model.CssClass;
            Assert.IsNotNull(cssClass);
            Assert.IsNotEmpty(cssClass);

            scriptTagId = Int32.MaxValue;
            model = new ScriptTagViewModel();
            model.Id = UrlUtility.EncodeNumber(scriptTagId);
            cssClass = model.CssClass;
            Assert.IsNotNull(cssClass);
            Assert.IsNotEmpty(cssClass);

        }

        /// <summary>
        /// Checks that css classes are given evenly out.
        /// </summary>
        [TestCase(49)]
        [TestCase(91)]
        [TestCase(1000)]
        [TestCase(1093)]
        [TestCase(5093)]
        public void GetTagClass_GivenScripts_AssignsClassesEvenly(int scriptCount)
        {
            string cssClass = "";
            Dictionary<string, int> dictClasses = new Dictionary<string, int>();

            for (int i = 0; i < scriptCount; i++)
            {
                ScriptTagViewModel model = new ScriptTagViewModel();
                model.Id = UrlUtility.EncodeNumber(i);
                cssClass = model.CssClass;
                if (dictClasses.ContainsKey(cssClass))
                {
                    dictClasses[cssClass] = dictClasses[cssClass] + 1;
                }
                else
                {
                    dictClasses.Add(cssClass, 1);
                }
            }

            // check that all css classes are accounted for
            Assert.AreEqual(ScriptTagViewModel.CssClasses.Length, dictClasses.Keys.Count);

            // maximum value should never be more than one bigger than min
            int max = dictClasses.Values.Max();
            int min = dictClasses.Values.Min();
            Assert.IsTrue(max == min || max == (min + 1), "Maximum number of classes in dictionary should never more more than 1 greater than minimum");
        }


        #endregion
    }
}
