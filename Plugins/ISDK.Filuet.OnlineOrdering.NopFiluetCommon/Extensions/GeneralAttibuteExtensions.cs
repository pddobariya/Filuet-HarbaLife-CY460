using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions
{
    public static class GeneralAttibuteExtensions
    {
        #region Methods

        /// <summary>
        /// Gets attribute by key
        /// </summary>
        public async static Task<GenericAttribute> GetCustomAttributeAsync(this IGenericAttributeService attributeService, BaseEntity entity, string key)
        {
            //List<GenericAttribute> attrs = attributeService.GetAttributesForEntityAsync(entity.Id, entity.GetUnproxiedEntityType().Name).ToList();
            List<GenericAttribute> attrs = (await attributeService.GetAttributesForEntityAsync(entity.Id, entity.GetType().Name)).ToList();
            return attrs.Find(x => x.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Validates if the custom attribute exists.
        /// </summary>
        public async static Task<bool> HasCustomAttributeAsync(this IGenericAttributeService attributeService, BaseEntity entity, string key)
        {
            return await attributeService.GetCustomAttributeAsync(entity, key) != null;
        }

        /// <summary>
        /// Searches an attribute value in <paramref name="props"/> list and makes conversion.
        /// For enum type value <paramref name="props"/> must store a string value.
        /// </summary>
        // TODO: it is not the extension method actually.
        public static TPropType GetCustomAttributeValue<TPropType>(this IGenericAttributeService attributeService, IList<GenericAttribute> props, string key)
        {
            if (!props.Any())
            {
                return default(TPropType);
            }

            var prop = props.FirstOrDefault(ga => ga.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)); //should be culture invariant

            if (prop == null || string.IsNullOrEmpty(prop.Value))
            {
                return default(TPropType);
            }

            return CommonHelper.To<TPropType>(prop.Value);
        }

        #endregion
    }
}
