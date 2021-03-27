using DbScriptDeploy.BLL.Encoding;
using DbScriptDeploy.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.ViewModels.Api
{
    public class ScriptTagViewModel
    {
        private string _tagCssClass = null;

        public string Id { get; set; }

        public string ScriptId { get; set; }

        public string Tag { get; set; }

        public string CssClass
        {
            get
            {
                if (_tagCssClass == null)
                {
                    int id = UrlUtility.DecodeNumber(this.Id);
                    int ordinal = id % CssClasses.Length;
                    _tagCssClass = CssClasses[ordinal];
                }
                return _tagCssClass;
            }
        }
                        
        public DateTime CreateDate { get; set; }

        public ScriptTagModel ToScriptModel()
        {
            return new ScriptTagModel()
            {
                Id = (String.IsNullOrWhiteSpace(this.Id) ? 0 : UrlUtility.DecodeNumber(this.Id)),
                Tag = this.Tag,
                ScriptId = (String.IsNullOrWhiteSpace(this.ScriptId) ? 0 : UrlUtility.DecodeNumber(this.ScriptId)),
                CreateDate = this.CreateDate
            };
        }

        public static ScriptTagViewModel FromScriptModel(ScriptTagModel model)
        {
            return new ScriptTagViewModel()
            {
                Id = (model.Id <= 0 ? "" : UrlUtility.EncodeNumber(model.Id)),
                Tag = model.Tag,
                ScriptId = (model.ScriptId <= 0 ? "" : UrlUtility.EncodeNumber(model.ScriptId)),
                CreateDate = model.CreateDate,
            };
        }

        public static string[] CssClasses = new string[] {
            "bg-amber",
            "bg-black",
            "bg-blue",
            "bg-blue-grey",
            "bg-brown",
            "bg-cyan",
            "bg-deep-orange",
            "bg-deep-purple",
            "bg-green",
            "bg-grey",
            "bg-indigo",
            "bg-light-blue",
            "bg-light-green",
            "bg-lime",
            "bg-orange",
            "bg-pink",
            "bg-purple",
            "bg-red",
            "bg-teal",
            "bg-yellow"
        };

    }
}
