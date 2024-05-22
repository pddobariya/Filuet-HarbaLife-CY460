using Nop.Core;
using Nop.Core.Domain.Localization;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain
{
    public partial class FiluetFusionShippingComputationOption : BaseEntity, ILocalizedEntity
    {
        #region Properties

        public string Name { get; set; }

        public string CountryCode { get; set; }

        public string WarehouseCode { get; set; }

        public string ProcessingLocationCode { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsSalesCenter { get; set; }

        public int? SalesCenterId { get; set; }

        public bool Deleted { get; set; }

        #endregion
    }
}
