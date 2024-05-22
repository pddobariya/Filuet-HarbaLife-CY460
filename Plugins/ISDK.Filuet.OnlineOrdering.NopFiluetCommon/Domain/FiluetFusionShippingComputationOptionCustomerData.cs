using Nop.Core;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain
{
    public partial class FiluetFusionShippingComputationOptionCustomerData : BaseEntity
    {
        #region Properties
        public int FiluetFusionShippingComputationOptionId { get; set; }

        public int AddressId { get; set; }

        public int CustomerId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        public string PhoneNumber { get; set; }

        public bool IsSelected { get; set; }

        public virtual FiluetFusionShippingComputationOption FiluetFusionShippingComputationOption { get; set; }

        #endregion
    }
}
