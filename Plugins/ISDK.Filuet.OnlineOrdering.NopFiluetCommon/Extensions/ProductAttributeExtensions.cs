using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions
{
    public static class ProductAttributeExtensions
    {
        #region Methods

        public async static Task<Dictionary<string, string>> ToProductAttributesIdsMapAsync(this string xml, IProductAttributeService attrServ)
        {
            Dictionary<ProductAttribute, ProductAttributeValue> attrsData = await xml.ToProductAttributesAsync(attrServ);
            Dictionary<string, string> converted = new Dictionary<string, string>();
            foreach (KeyValuePair<ProductAttribute, ProductAttributeValue> ad in attrsData)
            {
                string attrId = Convert.ToString(ad.Key.Id);
                if (!converted.ContainsKey(attrId))
                {
                    converted.Add(attrId, Convert.ToString(ad.Value.Id));
                }
            }

            return converted;
        }

        public async static Task<Dictionary<ProductAttribute, ProductAttributeValue>> ToProductAttributesAsync(this ProductAttributeCombination attributeCombination, IProductAttributeService attrServ)
        {
            if (attributeCombination == null || string.IsNullOrWhiteSpace(attributeCombination.AttributesXml))
            {
                return new Dictionary<ProductAttribute, ProductAttributeValue>();
            }

            string xml = attributeCombination.AttributesXml;

            return await xml.ToProductAttributesAsync(attrServ);
        }

        public async static Task<Dictionary<ProductAttribute, ProductAttributeValue>> ToProductAttributesAsync(this string xml, IProductAttributeService attrServ)
        {
            var productAttributeService = EngineContext.Current.Resolve<IProductAttributeService>();
            var productAttributValueService = EngineContext.Current.Resolve<IProductAttributeService>();
            if (string.IsNullOrWhiteSpace(xml))
            {
                return new Dictionary<ProductAttribute, ProductAttributeValue>();
            }

            XElement xmlDoc = XElement.Parse(xml);
            Dictionary<ProductAttribute, ProductAttributeValue> attrsData = new Dictionary<ProductAttribute, ProductAttributeValue>();

            foreach (XElement attrEl in xmlDoc.Descendants("ProductAttribute"))
            {
                if (attrEl.Attribute("ID") == null)
                {
                    continue;
                }

                if (!int.TryParse(attrEl.Attribute("ID").Value, out int attrMapId))
                {
                    continue;
                }

                ProductAttributeMapping productAttrMap = await attrServ.GetProductAttributeMappingByIdAsync(attrMapId);

                if (productAttrMap != null)
                {
                    XElement attrValEl = attrEl.Descendants("Value").FirstOrDefault();

                    if (attrValEl == null || !int.TryParse((attrValEl.FirstNode as XText).Value, out int attrValId))
                    {
                        continue;
                    }
                    
                    ProductAttribute productAttr = await productAttributeService.GetProductAttributeByIdAsync(productAttrMap.ProductAttributeId);
                    ProductAttributeValue productAttrVal = (await productAttributeService.GetProductAttributeValuesAsync(productAttrMap.Id)).FirstOrDefault(x => x.Id == attrValId);

                    if (productAttrVal != null)
                    {
                        attrsData.Add(productAttr, productAttrVal);
                    }
                }
            }

            return attrsData;
        }

        #endregion
    }
}
