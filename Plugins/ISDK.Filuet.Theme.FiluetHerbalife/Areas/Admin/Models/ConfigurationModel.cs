using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;


namespace ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        #region Properties

        [NopResourceDisplayName("ISDK.Filuet.Theme.FiluetHerbalife.CategoryIdForCatalogueTitle")]
        public int CategoryIdForCatalogue { get; set; }

        [NopResourceDisplayName("ISDK.Filuet.Theme.FiluetHerbalife.CategoryIdForProgrammTitle")]
        public int CategoryIdForProgramm { get; set; }

        [NopResourceDisplayName("ISDK.Filuet.Theme.FiluetHerbalife.Settings.OmnivaCarrierUrl")]
        public string OmnivaCarrierUrl { get; set; }

        [NopResourceDisplayName("ISDK.Filuet.Theme.FiluetHerbalife.Settings.DPDLatviaCarrierUrl")]
        public string DPDLatviaCarrierUrl { get; set; }

        [NopResourceDisplayName("ISDK.Filuet.Theme.FiluetHerbalife.Settings.DPDLithuaniaCarrierUrl")]
        public string DPDLithuaniaCarrierUrl { get; set; }

        [NopResourceDisplayName("ISDK.Filuet.Theme.FiluetHerbalife.Settings.DPDEstoniaCarrierUrl")]
        public string DPDEstoniaCarrierUrl { get; set; }

        #endregion
    }
}
