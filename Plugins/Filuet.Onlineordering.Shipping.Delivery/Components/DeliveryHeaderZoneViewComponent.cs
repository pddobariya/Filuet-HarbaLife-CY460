using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Components
{
    public class DeliveryHeaderZoneViewComponent : NopViewComponent
    {
        #region Methods

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.FromResult(View("~/Plugins/Filuet.Onlineordering.Shipping.Delivery/Views/Shared/Components/DeliveryHeaderZone/HeaderZone.cshtml"));
        }

        #endregion
    }
}
