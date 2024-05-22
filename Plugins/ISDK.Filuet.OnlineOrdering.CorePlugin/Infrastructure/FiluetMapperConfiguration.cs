using AutoMapper;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Models;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Models.Catalog;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Models.Customer;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Models.Order;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core.Infrastructure.Mapper;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;
using Nop.Web.Models.Order;
using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Infrastructure
{
    class FiluetMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public FiluetMapperConfiguration()
        {
            CreateMap<ProductModel, FiluetAdminProductModel>().ForMember(model => model.BasicRetailPrice, options => options.Ignore()).ForMember(model => model.EarnBasePrice, options => options.Ignore())/*.ForMember(model => model.Form, options => options.Ignore())*/.ForMember(model => model.ProductForOrderCategory, options => options.Ignore());

            CreateMap<ProductDetailsModel, FiluetProductDetailsModel>().ForMember(model => model.BasicRetailPrice, options => options.Ignore()).ForMember(model => model.EarnBasePrice, options => options.Ignore())/*.ForMember(model => model.Form, options => options.Ignore())*/.ForMember(model => model.ProductForOrderCategory, options => options.Ignore());
            CreateMap<GeneralCommonSettingsModel, FiluetGeneralCommonSettingsModel>().ForMember(model => model.StoreInformationSettings, options => options.MapFrom(x => AutoMapperConfiguration.Mapper.Map<FiluetStoreInformationSettingsModel>(x.StoreInformationSettings)));
            CreateMap<StoreInformationSettingsModel, FiluetStoreInformationSettingsModel>();
            CreateMap<IEnumerable<KeyValuePair<string, StringValues>>, IFormCollection>();
            CreateMap<SocialModel, FiluetSocialModel>();
            CreateMap<CustomerSearchModel, FiluetCustomerSearchModel>();
            CreateMap<CustomerListModel, FiluetCustomerListModel>();
            CreateMap<CustomerModel, FiluetCustomerModel>();

            CreateMap<DistributorFopLimitsModel, CustomerLimits>();
            CreateMap<CustomerLimits, DistributorFopLimitsModel>();

            CreateMap<FiluetOrderDetailsModel, OrderDetailsModel>();
            CreateMap<OrderDetailsModel, FiluetOrderDetailsModel>();

            CreateMap<FiluetOrderItemModel, OrderDetailsModel.OrderItemModel>();
            CreateMap<OrderDetailsModel.OrderItemModel, FiluetOrderItemModel>();
        }

        public int Order => 0;

    }
}
