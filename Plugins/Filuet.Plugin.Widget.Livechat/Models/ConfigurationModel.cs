using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Filuet.Plugin.Widget.Livechat.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        #region Properties
        public int ActiveStoreScopeConfiguration { get; set; } 

        [NopResourceDisplayName("Plugins.Widgets.Livechat.TrackingScript")]
        public string TrackingScript { get; set; }
        public bool TrackingScript_OverrideForStore { get; set; }

        #endregion
    }
}