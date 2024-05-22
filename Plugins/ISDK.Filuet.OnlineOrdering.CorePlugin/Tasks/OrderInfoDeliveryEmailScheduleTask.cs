using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.ExportImport.Help;
using Nop.Services.Localization;
using Nop.Services.ScheduleTasks;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Tasks
{
    public class OrderInfoDeliveryEmailScheduleTask : IScheduleTask
    {
        public const string Name = "Delivers oreder info to admins";

        #region Fields 

        private readonly IWorkContext _workContext;
        private readonly ILanguageService _languageService;
        private readonly ICustomerService _customerService;
        private readonly IAddressService _addressService;
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Ctor

        public OrderInfoDeliveryEmailScheduleTask(
            IWorkContext workContext,
            ILanguageService languageService,
            ICustomerService customerService,
            IAddressService addressService,
            IGenericAttributeService genericAttributeService)
        {
            _workContext = workContext;
            _languageService = languageService;
            _customerService = customerService;
            _addressService = addressService;
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        public static string TaskType => typeof(OrderInfoDeliveryEmailScheduleTask).AssemblyQualifiedName;

        #region Methods

        public async Task ExecuteAsync()
        {
            var ignore = await _workContext.GetCurrentVendorAsync() != null;
            var languages = (await _languageService.GetAllLanguagesAsync(showHidden: true)).ToList();
            //property array
            var properties = new[]
            {
                new PropertyByName<Order,Language>("дата внесния", (p, l) => p.PaidDateUtc),
                new PropertyByName<Order,Language>("FC", async (p, l) =>await _genericAttributeService.GetAttributeAsync<string>(p,OrderAttributeNames.FreightCode), ignore),
                new PropertyByName<Order,Language>("VP", async (p, l) =>await _genericAttributeService.GetAttributeAsync<string>(p,OrderAttributeNames.VolumePoints), ignore),
                new PropertyByName<Order,Language>("Общая сумма", (p, l) => p.OrderTotal, ignore),
                new PropertyByName<Order,Language>("АРТ", (p, l) => null),
                new PropertyByName<Order,Language>("№", (p, l) => null, ignore),
                new PropertyByName<Order,Language>("№ договора", (p, l) => null, ignore),
                new PropertyByName<Order,Language>("дата договора", (p, l) => null, ignore),
                new PropertyByName<Order,Language>("ID номер", async ( p, l) => (await  _customerService.GetCustomerByIdAsync(p.CustomerId)).GetDistributorIdAsync(), ignore),
                new PropertyByName<Order,Language>("ИНН", async (p, l) =>
                {
                     var customer = await _customerService.GetCustomerByIdAsync(p.CustomerId);
                     var attribute = await _genericAttributeService.GetAttributeAsync<string>(customer, CoreGenericAttributes.CustomerInnAttribute);
                     return attribute;
                },ignore),
                new PropertyByName<Order,Language>("OrderShippingInclTax", (p, l) => p.OrderShippingInclTax, ignore),
                new PropertyByName<Order,Language>("OrderShippingExclTax", (p, l) => p.OrderShippingExclTax, ignore),
                new PropertyByName<Order,Language>("PaymentMethodAdditionalFeeInclTax", (p, l)=> p.PaymentMethodAdditionalFeeInclTax, ignore),
                new PropertyByName<Order,Language>("PaymentMethodAdditionalFeeExclTax", (p, l)=> p.PaymentMethodAdditionalFeeExclTax, ignore),
                new PropertyByName<Order,Language>("TaxRates",(p, l)=> p.TaxRates, ignore),
                new PropertyByName<Order,Language>("OrderTax", (p, l) => p.OrderTax, ignore),
                new PropertyByName<Order,Language>("OrderTotal", (p, l) => p.OrderTotal, ignore),
                new PropertyByName<Order,Language>("RefundedAmount", (p, l)=> p.RefundedAmount, ignore),
                new PropertyByName<Order,Language>("OrderDiscount", (p, l) => p.OrderDiscount, ignore),
                new PropertyByName<Order,Language>("CurrencyRate",(p, l) => p.CurrencyRate),
                new PropertyByName<Order,Language>("CustomerCurrencyCode",(p, l) => p.CustomerCurrencyCode),
                new PropertyByName<Order,Language>("AffiliateId",(p, l) => p.AffiliateId, ignore),
                new PropertyByName<Order,Language>("PaymentMethodSystemName", (p, l) => p.PaymentMethodSystemName, ignore),
                new PropertyByName<Order,Language>("ShippingPickUpInStore", (p, l) => p.PickupInStore, ignore),
                new PropertyByName<Order,Language>("ShippingMethod", (p, l)=> p.ShippingMethod),
                new PropertyByName<Order,Language>("ShippingRateComputationMethodSystemName", (p, l) => p.ShippingRateComputationMethodSystemName, ignore),
                new PropertyByName<Order,Language>("CustomValuesXml", (p, l) => p.CustomValuesXml, ignore),
                new PropertyByName<Order,Language>("VatNumber", (p, l) => p.VatNumber, ignore),
                new PropertyByName<Order,Language>("CreatedOnUtc", (p, l) => p.CreatedOnUtc.ToOADate()),
                new PropertyByName<Order,Language>("BillingFirstName",async (p, l) => (await _addressService.GetAddressByIdAsync(p.BillingAddressId))?.FirstName ?? string.Empty),
                new PropertyByName<Order,Language>("BillingLastName",async (p, l) => (await _addressService.GetAddressByIdAsync(p.BillingAddressId))?.LastName ?? string.Empty),
                new PropertyByName<Order,Language>("BillingEmail", async(p, l) => (await _addressService.GetAddressByIdAsync(p.BillingAddressId))?.Email ?? string.Empty),
                new PropertyByName<Order,Language>("BillingCompany",async (p, l)=> (await _addressService.GetAddressByIdAsync(p.BillingAddressId))?.Company ?? string.Empty),
                new PropertyByName<Order,Language>("BillingCounty", async(p, l) => (await _addressService.GetAddressByIdAsync(p.BillingAddressId))?.County ?? string.Empty),
                new PropertyByName<Order,Language>("BillingCity", async(p, l) => (await _addressService.GetAddressByIdAsync(p.BillingAddressId))?.City ?? string.Empty),
                new PropertyByName<Order,Language>("BillingAddress1",async (p, l) =>(await _addressService.GetAddressByIdAsync(p.BillingAddressId))?.Address1 ?? string.Empty),
                new PropertyByName<Order,Language>("BillingAddress2", async(p, l) => (await _addressService.GetAddressByIdAsync(p.BillingAddressId))?.Address2 ?? string.Empty),
                new PropertyByName<Order,Language>("BillingZipPostalCode",async (p, l) =>(await _addressService.GetAddressByIdAsync(p.BillingAddressId))?.ZipPostalCode ?? string.Empty),
                new PropertyByName<Order,Language>("BillingPhoneNumber", async(p, l) =>(await _addressService.GetAddressByIdAsync(p.BillingAddressId))?.PhoneNumber ?? string.Empty),
                new PropertyByName<Order,Language>("BillingFaxNumber",async(p, l) => (await _addressService.GetAddressByIdAsync(p.BillingAddressId))?.FaxNumber ?? string.Empty),
                new PropertyByName<Order,Language>("ShippingFirstName", async(p, l)=>p.ShippingAddressId is null ? null : (await _addressService.GetAddressByIdAsync(p.ShippingAddressId.Value))?.FirstName ?? string.Empty),
                new PropertyByName<Order,Language>("ShippingLastName", async(p, l) => p.ShippingAddressId is null ? null : (await _addressService.GetAddressByIdAsync(p.ShippingAddressId.Value))?.LastName ?? string.Empty),
                new PropertyByName<Order,Language>("ShippingEmail", async(p, l) =>p.ShippingAddressId is null ? null : (await _addressService.GetAddressByIdAsync(p.ShippingAddressId.Value))?.Email ?? string.Empty),
                new PropertyByName<Order,Language>("ShippingCompany", async(p, l) => p.ShippingAddressId is null ? null : (await _addressService.GetAddressByIdAsync(p.ShippingAddressId.Value))?.Company ?? string.Empty),
                new PropertyByName<Order,Language>("ShippingCounty",async(p, l)=> p.ShippingAddressId is null ? null : (await _addressService.GetAddressByIdAsync(p.ShippingAddressId.Value))?.County ?? string.Empty),
                new PropertyByName<Order,Language>("ShippingCity", async(p, l) => p.ShippingAddressId is null ? null : (await _addressService.GetAddressByIdAsync(p.ShippingAddressId.Value))?.City ?? string.Empty),
                new PropertyByName<Order,Language>("ShippingAddress1", async(p, l)=> p.ShippingAddressId is null ? null : (await _addressService.GetAddressByIdAsync(p.ShippingAddressId.Value))?.Address1 ?? string.Empty),
                new PropertyByName<Order,Language>("ShippingAddress2", async(p, l) => p.ShippingAddressId is null ? null : (await _addressService.GetAddressByIdAsync(p.ShippingAddressId.Value))?.Address2 ?? string.Empty),
                new PropertyByName<Order,Language>("ShippingZipPostalCode", async(p, l) => p.ShippingAddressId is null ? null : (await _addressService.GetAddressByIdAsync(p.ShippingAddressId.Value))?.ZipPostalCode ?? string.Empty),
                new PropertyByName<Order,Language>("ShippingPhoneNumber", async(p, l)=> p.ShippingAddressId is null ? null : (await _addressService.GetAddressByIdAsync(p.ShippingAddressId.Value))?.PhoneNumber ?? string.Empty),
                new PropertyByName<Order,Language>("ShippingFaxNumber", async(p, l) => p.ShippingAddressId is null ? null : (await _addressService.GetAddressByIdAsync(p.ShippingAddressId.Value))?.FaxNumber ?? string.Empty)
            };
        }

        #endregion

    }
}
