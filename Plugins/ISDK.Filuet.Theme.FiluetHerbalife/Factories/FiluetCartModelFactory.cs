using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.FusionIntegartion;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using ISDK.Filuet.Theme.FiluetHerbalife.Models;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Factories
{
    public class FiluetCartModelFactory : IFiluetCartModelFactory
    {
        #region Fields

        private readonly IFusionIntegrationService _fusionIntegrationService;
        private readonly IWorkContext _workContext;
        private readonly IRepository<CustomerGenericAttribute> _customerGenericAttributeRepository;

        #endregion

        #region Ctor

        public FiluetCartModelFactory(
            IFusionIntegrationService fusionIntegrationService, 
            IWorkContext workContext, 
            IRepository<CustomerGenericAttribute> customerGenericAttributeRepository)
        {
            _fusionIntegrationService = fusionIntegrationService;
            _workContext = workContext;
            _customerGenericAttributeRepository = customerGenericAttributeRepository;
        }

        #endregion

        #region Methods

        public async Task<CartSummaryBarModel> PrepareCartSummaryBarModel()
        {
            Customer customer = await _workContext.GetCurrentCustomerAsync();
            ShoppingCartTotalModel cartTotal = await _fusionIntegrationService.GetShoppingCartTotalOffline(customer);

            CustomerGenericAttribute customerGenericAttribute = _customerGenericAttributeRepository.Table.FirstOrDefault(x => x.CustomerId == customer.Id);
            decimal discountValue = (customerGenericAttribute != null && customerGenericAttribute.Discount != null) ? (decimal)customerGenericAttribute.Discount.Value / (decimal)100 : 0;
            CartSummaryBarModel model = new CartSummaryBarModel();
            model.Discount = Convert.ToString(Convert.ToInt32(discountValue * 100));
            model.TotalVPs = await cartTotal.VolumePoints.FormatPriceAsync(true);
            model.TotalDue = await cartTotal.TotalDue.FormatPriceAsync();
            return model;
        }

        #endregion
    }
}
