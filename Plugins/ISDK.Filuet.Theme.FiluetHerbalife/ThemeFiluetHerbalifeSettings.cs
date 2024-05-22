using Nop.Core.Configuration;

namespace ISDK.Filuet.Theme.FiluetHerbalife
{
    public class ThemeFiluetHerbalifeSettings : ISettings
    {
        #region Properties

        public int CategoryIdForCatalogue { get; set; }
        public int CategoryIdForProgramm { get; set; }
        public string OmnivaCarrierUrl { get; set; }
        public string DPDLatviaCarrierUrl { get; set; }
        public string DPDLithuaniaCarrierUrl { get; set; }
        public string DPDEstoniaCarrierUrl { get; set; }

        #endregion
    }
}
