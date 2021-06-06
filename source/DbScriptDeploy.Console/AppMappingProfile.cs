using AutoMapper;
using DbScriptDeploy.BLL.Encoding;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.Console.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.Console
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            CreateMap<ProjectModel, ProjectViewModel>()
                .ForMember(x => x.Id, map => map.MapFrom(source => UrlUtility.EncodeNumber(source.Id)));
        }
    }
}
