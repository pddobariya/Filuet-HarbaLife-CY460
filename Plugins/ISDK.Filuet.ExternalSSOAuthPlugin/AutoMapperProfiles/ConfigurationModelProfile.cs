using AutoMapper;
using ISDK.Filuet.ExternalSSOAuthPlugin.Areas.Admin.Models;
using Nop.Core.Infrastructure.Mapper;

namespace ISDK.Filuet.ExternalSSOAuthPlugin.AutoMapperProfiles
{
    public class ConfigurationModelProfile : Profile, IOrderedMapperProfile
    {
        #region Methods

        public ConfigurationModelProfile()
        {
            CreateMap<SSOAuthPluginSettings, ConfigurationModel>();
            CreateMap<ConfigurationModel, SSOAuthPluginSettings>();
        }

        public int Order => 10;

        #endregion
    }
}
