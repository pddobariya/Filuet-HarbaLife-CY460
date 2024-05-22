using Nop.Core;
using System.Collections.Generic;


namespace Filuet.Onlineordering.Shipping.Delivery.Domain
{
    public class PickupPointsCost : BaseEntity
    {
        #region Properties

        public string PointId { get; set; }

        public bool Blocked { get; set; }

        public string WarehouseCode { get; set; }

        public int PickupPointsOperatorId { get; set; }

        public virtual List<PickupPointsLanguage> PickupPointsLanguages { get; set; }

        public virtual PickupPointsOperator PickupPointsOperator { get; set; }

        #endregion
    }
}
