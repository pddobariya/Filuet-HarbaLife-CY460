using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models
{
    [TypeScriptModel]
    public class ShippingMethodModel
    {
        #region Ctor
        public ShippingMethodModel()
        {
            AdditionalShippingFields = new List<FormFieldMetaModel>();
            HiddenShippingFields = new List<string>();
        }
        #endregion

        #region Properties

        [JsonProperty(PropertyName = "methodFriendlyName")]
        public string MethodFriendlyName { get; set; }

        [JsonProperty(PropertyName = "methodSystemName")]
        public string MethodSystemName { get; set; }

        [JsonProperty(PropertyName = "isSelected")]
        public bool IsSelected { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "countryId")]
        public int? CountryId { get; set; }

        [JsonProperty(PropertyName = "countryName")]
        public string CountryName { get; set; }

        [JsonProperty(PropertyName = "zipPostalCode")]
        public string ZipPostalCode { get; set; }

        [JsonProperty(PropertyName = "pickupPointId")]
        public int? PickupPointId { get; set; }

        [JsonProperty(PropertyName = "additionalShippingFields")]
        public IEnumerable<FormFieldMetaModel> AdditionalShippingFields { get; set; }

        [JsonProperty(PropertyName = "hiddenShippingFields")]
        public IEnumerable<string> HiddenShippingFields { get; set; }

        [JsonProperty(PropertyName = "displayOrder")]
        public int DisplayOrder { get; set; }

        [JsonProperty(PropertyName = "isSalesCenter")]
        public bool IsSalesCenter { get; set; }

        #endregion

    }
}
