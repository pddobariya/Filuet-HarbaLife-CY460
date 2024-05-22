using ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Constants;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        #region Properties

        [NopResourceDisplayName("ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Fields.OmnivaUrl")]
        public string OmnivaUrl { get; set; }

        [NopResourceDisplayName("ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Fields.DpdFtpUrl")]
        public string DpdFtpUrl { get; set; }

        [NopResourceDisplayName("ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Fields.DpdFtpLogin")]
        public string DpdFtpLogin { get; set; }

        //[NoTrim]
        [DataType(DataType.Password)]
        [NopResourceDisplayName("ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Fields.DpdFtpPwd")]
        public string DpdFtpPwd { get; set; }

        #endregion
    }
}
