using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using Newtonsoft.Json;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Fusion
{
    public static class FusionHelpers
    {
        #region Methods
        public async static Task<OrderItemModel> GetOrderItemAsync(int qty, Product product, string attrsXml, IProductAttributeService attrServ, string warehouse)
        {
            string sku = product.Sku;
            if (!string.IsNullOrEmpty(attrsXml))
            {

                Dictionary<string, string> attrsMap = await attrsXml.ToProductAttributesIdsMapAsync(attrServ);
                ProductAttributeCombination selectedCombination = await product.GetProductAttributeCombinationByAttributesAsync(attrsMap, attrServ);
                if (selectedCombination != null)
                {
                    sku = selectedCombination.Sku;
                }
            }
            return new OrderItemModel()
            {
                Count = qty,
                Sku = new SkuItemModel()
                {
                    Name = sku,
                    Warehouse = warehouse
                },
                ProductType = product.ProductType
            };
        }

        public static string ToCountryCode(this string country)
        {
            if (string.IsNullOrWhiteSpace(country))
            {

                return null;
            }
            country = country.Trim().ToLower();
            switch (country)
            {
                case "latvia":
                    return CountryCodes.LV;
                case "lithuania":
                    return CountryCodes.LT;
                case "estonia":
                    return CountryCodes.EE;
            }
            return country.ToUpper().Trim();
        }

        public static void LogFusionCall(string message, object data)
        {
            LogFusionCall(message, JsonConvert.SerializeObject(data));
        }

        public static async void LogFusionCall(string message, string json)
        {
            var logger = EngineContext.Current.Resolve<ILogger>();
            await logger.InformationAsync($"{message}: {json}");
        }

        #endregion
    }
}
