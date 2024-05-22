using AutoMapper;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Models;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Models.Customer;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Models.ShoppingCart;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon;
using Nop.Web.Models.Customer;
using Nop.Web.Models.ShoppingCart;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Infrastructure
{
    #region Methods

    internal class SettingsModelProfile : Profile
    {
        public SettingsModelProfile()
        {
            CreateMap<ConfigurationModel, FiluetCorePluginSettings>().ReverseMap();
        }
    }

    internal class OrderTotalsModelProfile : Profile
    {
        public OrderTotalsModelProfile()
        {
            CreateMap<OrderTotalsModel, FiluetOrderTotalsModel>();
        }
    }

    internal class CustomerInfoModelProfile : Profile
    {
        public CustomerInfoModelProfile()
        {
            CreateMap<CustomerInfoModel, FiluetCustomerInfoModel>();
        }
    }

    internal class ShoppingCartModelProfile : Profile
    {
        public ShoppingCartModelProfile()
        {
            CreateMap<ShoppingCartModel, FiluetShoppingCartModel>();
        }
    }

    internal class PluginMapper
    {
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
                cfg.AddProfile<CustomerInfoModelProfile>();
                cfg.AddProfile<ShoppingCartModelProfile>();
                cfg.AddProfile<SettingsModelProfile>();
                cfg.AddProfile<OrderTotalsModelProfile>();
            });

            _mapper = config.CreateMapper();
        }
    }

    #endregion
}
