using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.FusionIntegartion;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Payments;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Services.Payment
{
    public class FiluetPaymentService : IFiluetPaymentService
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IFusionIntegrationService _fusionIntegrationService;
        private readonly ILogger _logger;
        private readonly IShoppingCartService _shoppingCartService;

        #endregion

        #region Ctor

        public FiluetPaymentService(
            IWorkContext workContext,
            IFusionIntegrationService fusionIntegrationService,
            ILogger logger,
            IShoppingCartService shoppingCartService)
        {
            _workContext = workContext;
            _fusionIntegrationService = fusionIntegrationService;
            _logger = logger;
            _shoppingCartService = shoppingCartService;
        }

        #endregion

        #region Methods

        public async Task<ProcessPaymentResult> ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            try
            {
                Customer customer =await _workContext.GetCurrentCustomerAsync();
                ShoppingCartTotalModel cartTotal = null;
                try
                {
                    cartTotal =await _fusionIntegrationService.GetShoppingCartTotalAsync(customer);
                    var shoppingCartItems = await _shoppingCartService.GetShoppingCartAsync(customer);
                    if (cartTotal.TotalDue == 0 && shoppingCartItems != null && shoppingCartItems.Any())
                    {
                        string msgErr = $@"Error calling GetShoppingCartTotal Fusion method. Method returned TotalDue=0; 
                                        TotalAmount={cartTotal.TotalAmount}; 
                                        TotalTaxAmount={cartTotal.TotalTaxAmount}";
                       await _logger.ErrorAsync($"{nameof(FiluetPaymentService)}:{msgErr}", null, customer);

                        cartTotal = await _fusionIntegrationService.GetShoppingCartTotalOffline(customer);
                    }
                }
                catch (Exception exc)
                {
                    _logger.ErrorAsync($"{nameof(FiluetPaymentService)}: Error calling GetShoppingCartTotal Fusion method.", exc, customer).Wait();
                    cartTotal = await _fusionIntegrationService.GetShoppingCartTotalOffline(customer);
                }

                decimal total = cartTotal != null ? cartTotal.TotalDue : 0;

                processPaymentRequest.OrderTotal = total;
                var result = new ProcessPaymentResult();
                result.NewPaymentStatus = PaymentStatus.Pending;

                _logger.InformationAsync($"{nameof(FiluetPaymentService)}:NewPaymentStatus=Pending").Wait();
                return result;
            }
            catch (Exception ex)
            {
                _logger.ErrorAsync(nameof(FiluetPaymentService), ex).Wait();
                return null;
            }
        }

        #endregion
    }
}
