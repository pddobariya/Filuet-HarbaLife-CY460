using ISDK.Filuet.Theme.FiluetHerbalife.Models.Checkout;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Models.Checkout;
using Nop.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Factories
{
    public class CustomCheckoutModelFactory : ICustomCheckoutModelFactory
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly ICustomerService _customerService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly TaxSettings _taxSettings;
        private readonly IProductService _productService;

        #endregion

        #region Ctor

        public CustomCheckoutModelFactory(
            AddressSettings addressSettings,
            IAddressModelFactory addressModelFactory,
            IAddressService addressService,
            ICountryService countryService,
            ICustomerService customerService,
            IShoppingCartService shoppingCartService,
            IStoreMappingService storeMappingService,
            IWorkContext workContext,
            OrderSettings orderSettings,
            ShippingSettings shippingSettings,
            TaxSettings taxSettings,
            IProductService productService)
        {
            _addressSettings = addressSettings;
            _addressModelFactory = addressModelFactory;
            _addressService = addressService;
            _countryService = countryService;
            _customerService = customerService;
            _shoppingCartService = shoppingCartService;
            _storeMappingService = storeMappingService;
            _workContext = workContext;
            _orderSettings = orderSettings;
            _shippingSettings = shippingSettings;
            _taxSettings = taxSettings;
            _productService = productService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare billing address model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="selectedCountryId">Selected country identifier</param>
        /// <param name="prePopulateNewAddressWithCustomerFields">Pre populate new address with customer fields</param>
        /// <param name="overrideAttributesXml">Override attributes xml</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the billing address model
        /// </returns>
        public virtual async Task<CheckoutBillingAddressModel> PrepareBillingAddressModelAsync(IList<ShoppingCartItem> cart,
            int? selectedCountryId = null,
            bool prePopulateNewAddressWithCustomerFields = false,
            string overrideAttributesXml = "")
        {
            var model = new CheckoutBillingAddressModel
            {
                ShipToSameAddressAllowed = _shippingSettings.ShipToSameAddress && await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart),
                //allow customers to enter (choose) a shipping address if "Disable Billing address step" setting is enabled
                ShipToSameAddress = !_orderSettings.DisableBillingAddressCheckoutStep
            };

            var customer = await _workContext.GetCurrentCustomerAsync();
            if (await _customerService.IsGuestAsync(customer) && _taxSettings.EuVatEnabled)
            {
                model.VatNumber = customer.VatNumber;
                model.EuVatEnabled = true;
                model.EuVatEnabledForGuests = _taxSettings.EuVatEnabledForGuests;
            }

            //existing addresses
            var addresses = await (await _customerService.GetAddressesByCustomerIdAsync(customer.Id))
                .WhereAwait(async a => !a.CountryId.HasValue || await _countryService.GetCountryByAddressAsync(a) is Country country &&
                    (//published
                    country.Published &&
                    //allow billing
                    country.AllowsBilling &&
                    //enabled for the current store
                    await _storeMappingService.AuthorizeAsync(country)))
                .ToListAsync();
            foreach (var address in addresses)
            {
                var addressModel = new AddressModel();
                await _addressModelFactory.PrepareAddressModelAsync(addressModel,
                    address: address,
                    excludeProperties: false,
                    addressSettings: _addressSettings);

                if (await _addressService.IsAddressValidAsync(address))
                {
                    model.ExistingAddresses.Add(addressModel);
                }
                else
                {
                    model.InvalidExistingAddresses.Add(addressModel);
                }
            }

            //new address
            model.BillingNewAddress.CountryId = selectedCountryId;
            await _addressModelFactory.PrepareAddressModelAsync(model.BillingNewAddress,
                address: null,
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: async () => await _countryService.GetAllCountriesForBillingAsync((await _workContext.GetWorkingLanguageAsync()).Id),
                prePopulateWithCustomerFields: prePopulateNewAddressWithCustomerFields,
                customer: customer,
                overrideAttributesXml: overrideAttributesXml);

            return model;
        }
        private async Task<decimal> CalculateWeightOfProducts(IList<ShoppingCartItem> cart)
        {
            var dictionaryOfProducts = cart.GroupBy(x => x.ProductId)
                                .Select(x =>
                                new
                                {
                                    ProductId = x.Key,
                                    Quantity = x.Sum(y => y.Quantity)
                                })
                                .ToDictionary(x => x.ProductId, x => x.Quantity);
            var cartProducts = await _productService.GetProductsByIdsAsync(dictionaryOfProducts.Keys.ToArray());
            var totalWeight = cartProducts.Sum(x => x.Weight * dictionaryOfProducts[x.Id]);
            return totalWeight;
        }

        /// <summary>
        /// Prepare one page checkout model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the one page checkout model
        /// </returns>
        public virtual async Task<CustomOnePageCheckoutModel> PrepareOnePageCheckoutModelAsync(IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            var model = new CustomOnePageCheckoutModel
            {
                ShippingRequired = await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart),
                DisableBillingAddressCheckoutStep = _orderSettings.DisableBillingAddressCheckoutStep && (await _customerService.GetAddressesByCustomerIdAsync((await _workContext.GetCurrentCustomerAsync()).Id)).Any(),
                BillingAddress = await PrepareBillingAddressModelAsync(cart, prePopulateNewAddressWithCustomerFields: true),
                TotalOrderWeight = await CalculateWeightOfProducts(cart)
            };
            return model;
        }

        #endregion

    }
}
