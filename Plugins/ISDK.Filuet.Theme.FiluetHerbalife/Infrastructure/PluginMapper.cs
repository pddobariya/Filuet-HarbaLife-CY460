using AutoMapper;
using ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Models;
using ISDK.Filuet.Theme.FiluetHerbalife.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Blogs;
using Nop.Web.Models.Catalog;
using AdminModels = Nop.Web.Areas.Admin.Models;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Infrastructure
{
    internal class PluginMapper
    {
        #region Method

        private static PluginMapper _instance = new PluginMapper();

        public static IMapper Mapper
        {
            get { return _instance._mapper; }
        }

        private IMapper _mapper;

        public PluginMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SettingsModelProfile>();
            });

            _mapper = config.CreateMapper();
        }

        #endregion
    }

    internal class SettingsModelProfile : Profile
    {
        #region Method

        public SettingsModelProfile()
        {
            CreateMap<BlogPostModel, FiluetBlogPostModel>().ReverseMap();
            CreateMap<CategoryModel, FiluetCategoryModel>().ReverseMap();
            CreateMap<ProductDetailsModel, FiluetProductDetailsModel>().ReverseMap();

            CreateMap<AdminModels.Catalog.CategoryModel, Areas.Admin.Models.Catalog.FiluetCategoryModel>().ReverseMap();
            CreateMap<AdminModels.Catalog.CategoryLocalizedModel, Areas.Admin.Models.Catalog.FiluetCategoryLocalizedModel>().ReverseMap();
            CreateMap<OnlineOrdering.CorePlugin.Models.Catalog.FiluetAdminProductModel, Areas.Admin.Models.Product.FiluetProductModel>().ReverseMap();
            CreateMap<AdminModels.Catalog.ProductLocalizedModel, Areas.Admin.Models.Product.FiluetProductLocalizedModel>().ReverseMap();
        }

        #endregion
    }
}
