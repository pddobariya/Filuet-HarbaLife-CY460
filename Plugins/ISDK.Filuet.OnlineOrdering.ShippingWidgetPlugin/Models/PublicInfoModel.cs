using Nop.Web.Framework.Models;

namespace ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Models
{
    public record PublicInfoModel : BaseNopModel
    {
        #region Properties

        public string Merchant { get; set; }

        public string Carrier { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string DeliveryTime { get; set; }
        public string PostamatId { get; set; }

        public bool IsPickupShipping { get; set; }

        public bool IsInited => IsFilled(FirstName) && IsFilled(LastName) && IsFilled(PhoneNumber)
                                && (IsFilled(PostamatId)
                                    ||
                                    IsFilled(Country)
                                    && IsFilled(City)
                                    && (IsFilled(PostalCode) || IsPickupShipping)
                                    && IsFilled(Address));

        /// <summary>
        /// Returns true if string is not null and not empty.
        /// Wrapper to improve readability.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsFilled(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        public string WidgetAddress { get; set; }
        public string Comment { get; set; }

        #endregion
    }
}
