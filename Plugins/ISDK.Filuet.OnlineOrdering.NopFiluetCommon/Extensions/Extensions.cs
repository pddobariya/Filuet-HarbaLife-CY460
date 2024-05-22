using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.FusionIntegartion;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Shipping;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Services.Authentication.External;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ErrorMessages = ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants.ErrorMessages;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions
{
    public static class Extensions
    {
        #region Methods

        public async static Task<string> SerializeCustomValuesAsync(this Dictionary<string, object> data)
        {
            if (data == null || !data.Any())
            {
                return null;
            }

            var ds = new DictionarySerializer(data);
            var xs = new XmlSerializer(typeof(DictionarySerializer));

            using (StringWriter textWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter))
                {
                    xs.Serialize(xmlWriter, ds);
                }
                string result = textWriter.ToString();
                return await Task.FromResult(result);
            }
        }

        public async static Task<CategoryTypeEnum> GetOrderTypeAsync(this IEnumerable<Product> products)
        {
            products = products.ToArray();
            if (await products.AllAwaitAsync(async x =>  await x.GetProductTypeAsync() != CategoryTypeEnum.Product))
            {
                return await products.FirstOrDefault()?.GetProductTypeAsync() ?? CategoryTypeEnum.Maintenance;
            }
            return CategoryTypeEnum.Product;
        }

        public async static Task<string> GetFusionOrderNumberAsync(this Order order)
        {
            IGenericAttributeService genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
            string fusionOrderNumber = "--";
            if (await genericAttributeService.HasCustomAttributeAsync(order, OrderAttributeNames.FusionOrderNumber))
            {
                fusionOrderNumber = await genericAttributeService.GetAttributeAsync<string>(order, OrderAttributeNames.FusionOrderNumber);
            }
            return fusionOrderNumber;
        }

        public async static Task<CategoryTypeEnum> GetOrderTypeAsync(this Order order)
        {
            var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
            IOrderService orderService = null;
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                orderService = serviceScope.ServiceProvider.GetService<IOrderService>();
            }
            IProductService productService = null;
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                productService = serviceScope.ServiceProvider.GetService<IProductService>();
            }

            return await orderService.GetOrderItemsAsync(order.Id).Result.Select(x => productService.GetProductByIdAsync(x.ProductId).Result).GetOrderTypeAsync();
        }

        public async static Task<CategoryTypeEnum> GetOrderTypeAsync(this List<ShoppingCartItem> cartItems)
        {
            var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
            IProductService productService = null;
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                productService = serviceScope.ServiceProvider.GetService<IProductService>();
            }
            return await (cartItems.Select(x => productService.GetProductByIdAsync(x.ProductId).Result)).GetOrderTypeAsync();
        }

        public static string FormatPercent(this double price)
        {
            return string.Format("{0}%", price);
        }

        public async static Task<string> FormatPriceAsync(this double price, bool withoutCurrency = false)
        {
            if (withoutCurrency)
            {
                return string.Format("{0:0.00}", price);
            }
            Currency currency = await EngineContext.Current.Resolve<IWorkContext>().GetWorkingCurrencyAsync();

            string priceStr = price.ToString(currency.CustomFormatting);
            return priceStr;
        }

        public async static Task<string> FormatPriceAsync(this decimal price, bool withoutCurrency = false)
        {
            return await Convert.ToDouble(price).FormatPriceAsync(withoutCurrency);
        }

        public async static Task<ProductAttributeCombination> GetProductAttributeCombinationByAttributesAsync(this Product product, Dictionary<string, string> attrsData, IProductAttributeService attrServ)
        {
            if (product == null || attrsData == null || attrsData.Count == 0 || attrServ == null)
            {
                return null;
            }
            ProductAttributeCombination combination = null;
            var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
            IProductAttributeService productAttributeService = null;
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                productAttributeService = serviceScope.ServiceProvider.GetService<IProductAttributeService>();
            }
            foreach (ProductAttributeCombination comb in productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id).Result)
            {
                Dictionary<ProductAttribute, ProductAttributeValue> combAttrs = await comb.ToProductAttributesAsync(attrServ);
                if (combAttrs == null || combAttrs.Count != attrsData.Count)
                {
                    continue;
                }
                bool isMatch = true;
                foreach (KeyValuePair<ProductAttribute, ProductAttributeValue> attr in combAttrs)
                {
                    string attrId = Convert.ToString(attr.Key.Id);
                    string attrValId = Convert.ToString(attr.Value.Id);
                    if (!attrsData.Any(a => a.Key == attrId && a.Value == attrValId))
                    {
                        isMatch = false;
                        break;
                    }
                    if (!isMatch)
                    {
                        break;
                    }
                }
                if (isMatch)
                {
                    combination = comb;
                    break;
                }
            }

            return combination;
        }

        public static bool IsEqualAttributes(this Dictionary<string, string> thisAttrs, Dictionary<string, string> otherAttrs)
        {
            if (thisAttrs == null || thisAttrs.Count == 0 || otherAttrs == null || otherAttrs.Count == 0 || thisAttrs.Count != otherAttrs.Count)
            {
                return false;
            }
            int matchCount = 0;
            foreach (KeyValuePair<string, string> thisAttr in thisAttrs)
            {
                KeyValuePair<string, string> otherAttr = otherAttrs.FirstOrDefault(x => x.Key == thisAttr.Key && x.Value == thisAttr.Value);
                if (otherAttr.Equals(default(KeyValuePair<string, string>)))
                {
                    return false;
                }
                else
                {
                    matchCount++;
                }
            }
            return matchCount == thisAttrs.Count;
        }

        public static async Task<IList<string>> GetShoppingCartMessages(this StockBalanceModel stockBalance, Customer currentCustomer, MessageSeverityEnum severity,
                bool notUpdatedMessage = false)
        {
            if (stockBalance == null || stockBalance.IsValid)
            {
                return null;
            }

            IProductService productService = EngineContext.Current.Resolve<IProductService>();
            IFusionIntegrationService fusionIntegrationService = EngineContext.Current.Resolve<IFusionIntegrationService>();
            IFiluetShippingService filuetShippingService = EngineContext.Current.Resolve<IFiluetShippingService>();

            List<StockBalanceItemModel> blockedItems = stockBalance.StockBalances.Where(x => x.IsBlocked).ToList();
            List<StockBalanceItemModel> invalidItems = stockBalance.StockBalances.Where(x => !x.IsValid).ToList();
            List<StockBalanceItemModel> outOfStockItems = stockBalance.StockBalances.Where(x => !x.IsAvailable).ToList();
            List<string> messages = new List<string>();

            if (severity == MessageSeverityEnum.Error)
            {
                if (invalidItems.Any())
                {
                    //check for HMP case to display more appropriate message
                    string userCountryHmpSku = null;//filuetShippingService.GetFreeShippingSku(currentCustomer);
                    List<string> otherCountriesHmpSkus = (await filuetShippingService.GetFreeShippingSkusAsync()).FindAll(x => x != userCountryHmpSku);
                    List<StockBalanceItemModel> hmpMatches = invalidItems.FindAll(x => otherCountriesHmpSkus.Any(s => s == x.Sku.Name));
                    bool outPutOtherSkus = (invalidItems.Count - hmpMatches.Count > 0);

                    if (hmpMatches.Any())
                    {
                        messages.Add(await ErrorMessages.HmpSkuWrongCountryInCart.ToLocalizedStringAsync());
                    }

                    if (outPutOtherSkus)
                    {
                        messages.Add(await ErrorMessages.SkuInvalidStart.ToLocalizedStringAsync());
                        foreach (StockBalanceItemModel sbi in invalidItems.FindAll(x => !hmpMatches.Any(hmp => hmp.Sku.Name == x.Sku.Name)))
                        {
                            messages.Add(string.Format(await ErrorMessages.SkuInvalidEntry.ToLocalizedStringAsync(), sbi.Sku.Name));
                        }
                        messages.Add(await ErrorMessages.SkuInvalidEnd.ToLocalizedStringAsync());
                    }
                }
            }
            if (severity == MessageSeverityEnum.Error)
            {
                blockedItems = blockedItems.FindAll(sbi => !invalidItems.Any(x => x.Sku.Name == sbi.Sku.Name));
                if (blockedItems.Any())
                {
                    messages.Add(await ErrorMessages.SkuBlockedStart.ToLocalizedStringAsync());
                    foreach (StockBalanceItemModel sbi in blockedItems)
                    {
                        Product product = await productService.GetProductBySkuAsync(sbi.Sku.Name);
                        messages.Add(string.Format(await ErrorMessages.SkuBlockedEntry.ToLocalizedStringAsync(), sbi.Sku.Name, product == null ? "" : product.Name));
                    }
                    messages.Add(await ErrorMessages.SkuBlockedEnd.ToLocalizedStringAsync());
                }
            }
            if ((severity == MessageSeverityEnum.Warning && notUpdatedMessage) || (severity == MessageSeverityEnum.Error && !notUpdatedMessage))
            {
                outOfStockItems = outOfStockItems.FindAll(sbi => !blockedItems.Any(x => x.Sku.Name == sbi.Sku.Name) && !invalidItems.Any(x => x.Sku.Name == sbi.Sku.Name));
                if (outOfStockItems.Any())
                {
                    messages.Add(await ErrorMessages.OutOfStockStart.ToLocalizedStringAsync());
                    foreach (StockBalanceItemModel sbi in outOfStockItems)
                    {
                        Product product = await productService.GetProductBySkuAsync(sbi.Sku.Name);
                        messages.Add(string.Format(await ErrorMessages.OutOfStockEntry.ToLocalizedStringAsync(), sbi.Sku.Name, product == null ? "" : product.Name));
                    }
                    messages.Add(notUpdatedMessage ? await ErrorMessages.OutOfStockNotUpdatedEnd.ToLocalizedStringAsync() : await ErrorMessages.OutOfStockEnd.ToLocalizedStringAsync());
                }
            }

            return messages;
        }

        /// <summary>
        /// Formats Product Attributes
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="productAttributeParser">Product attribute service (used when attributes are specified)</param>
        /// <returns>Product Attributes</returns>
        public static ProductAttributeModel FormatProductAttributes(this Product product, string attributesXml = null,
            IProductAttributeParser productAttributeParser = null)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            string sku;
            string manufacturerPartNumber;
            string gtin;
            decimal? vp;

            product.GetSkuMpnGtinVp(attributesXml, productAttributeParser,
                out sku, out manufacturerPartNumber, out gtin, out vp);

            return new ProductAttributeModel()
            {
                Sku = sku,
                ManufacturerPartNumber = manufacturerPartNumber,
                Gtin = gtin,
                Vp = vp
            };
        }


        /// <summary>
        /// Gets SKU, Manufacturer part number, GTIN, Volume Points
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="productAttributeParser">Product attribute service (used when attributes are specified)</param>
        /// <param name="sku">SKU</param>
        /// <param name="manufacturerPartNumber">Manufacturer part number</param>
        /// <param name="gtin">GTIN</param>
        /// <param name="vp">Volume Points</param>
        private static void GetSkuMpnGtinVp(this Product product, string attributesXml, IProductAttributeParser productAttributeParser,
             out string sku, out string manufacturerPartNumber, out string gtin, out decimal? vp)
        {
            IGenericAttributeService genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
            if (product == null)
                throw new ArgumentNullException("product");

            sku = null;
            manufacturerPartNumber = null;
            gtin = null;
            vp = null;

            if (!string.IsNullOrEmpty(attributesXml) &&
                product.ManageInventoryMethod == ManageInventoryMethod.ManageStockByAttributes)
            {
                // manage stock by attribute combinations
                if (productAttributeParser == null)
                    throw new ArgumentNullException("productAttributeParser");

                // let's find appropriate record
                var combination = productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml).Result;
                if (combination != null)
                {
                    sku = combination.Sku;
                    manufacturerPartNumber = combination.ManufacturerPartNumber;
                    gtin = combination.Gtin;
                    vp =  genericAttributeService.GetAttributeAsync<decimal>(combination,ProductAttributeNames.OverriddenVolumePoints).Result;
                }
            }

            if (string.IsNullOrEmpty(sku))
            {
                sku = product.Sku;
            }

            if (string.IsNullOrEmpty(manufacturerPartNumber))
            {
                manufacturerPartNumber = product.ManufacturerPartNumber;
            }

            if (string.IsNullOrEmpty(gtin))
            {
                gtin = product.Gtin;
            }

            if (!vp.HasValue)
            {
                vp =  genericAttributeService.GetAttributeAsync<decimal>(product,ProductAttributeNames.VolumePoints).Result;
            }
        }

        public async static Task<bool> IsDebtorAsync(this Customer customer)
        {
            var settingsLoader = EngineContext.Current.Resolve<ISettingsLoader>();
 
            if (await settingsLoader?.IsDeptorEnabled() != true)
                return false;
            var apfDueDate = await customer.GetApfDueDateAsync();
            return apfDueDate < DateTime.UtcNow.AddHours(await settingsLoader.GetHoursShift());
        }

        public static IPagedList<Customer> GetAllCustomers(this ICustomerService customerService, string searchExternalIdentifier, DateTime? createdFromUtc = null,
            DateTime? createdToUtc = null, DateTime? lastActivityFromUtc = null, DateTime? lastActivityToUtc = null, int affiliateId = 0, int vendorId = 0,
            int[] customerRoleIds = null, string email = null, string username = null,
            string firstName = null, string lastName = null,
            int dayOfBirth = 0, int monthOfBirth = 0,
            string company = null, string phone = null, string zipPostalCode = null,
            string ipAddress = null, bool loadOnlyWithShoppingCart = false, ShoppingCartType? sct = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var externalAuthenticationService = EngineContext.Current.Resolve<IExternalAuthenticationService>();
            var customers = customerService.GetAllCustomersAsync(createdFromUtc,
                createdToUtc, lastActivityFromUtc, lastActivityToUtc, affiliateId, vendorId,
                customerRoleIds, email, username,
                firstName, lastName,
                dayOfBirth, monthOfBirth,
                company, phone, zipPostalCode,
                ipAddress,
                pageIndex, pageSize, getOnlyTotalCount).Result;
            //IQueryable<Customer> query = null;
            if (!string.IsNullOrWhiteSpace(searchExternalIdentifier))
            {
                var source = customers.Where(customer =>
                    externalAuthenticationService.GetCustomerExternalAuthenticationRecordsAsync(customer).Result.FirstOrDefault()?.ExternalIdentifier == searchExternalIdentifier).ToList();
                customers = new PagedList<Customer>(source, pageIndex, pageSize);
            }
            return customers;
        }

        public static string ToHtmlString(this IHtmlContent htmlContent)
        {
            if (htmlContent is HtmlString htmlString)
            {
                return htmlString.Value;
            }

            using (var writer = new StringWriter())
            {
                htmlContent.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
                return writer.ToString();
            }
        }

        #endregion
    }
}