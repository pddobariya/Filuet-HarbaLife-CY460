using Nop.Web.Framework.Models;
using System.Collections.Generic;

namespace Filuet.Onlineordering.Shipping.Delivery.Models
{
    public partial record PriceDtoModel : BaseNopEntityModel
    {
        #region Ctor

        public PriceDtoModel()
        {
            PriceDtoAddModels = new List<PriceDtoAddModel>();
            PriceDtoSearchModels = new List<PriceDtoSearchModel>();
        }

        #endregion

        #region Properties

        public IList<PriceDtoAddModel> PriceDtoAddModels { get; set; }
        public IList<PriceDtoSearchModel> PriceDtoSearchModels { get; set; }

        #endregion
    }
}
