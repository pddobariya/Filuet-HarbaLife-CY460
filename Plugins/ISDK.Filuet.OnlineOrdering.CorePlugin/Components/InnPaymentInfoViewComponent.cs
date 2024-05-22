using ISDK.Filuet.OnlineOrdering.NopFiluetCommon;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants.AttributeNames;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Web.Framework.Components;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Components
{
    [ViewComponent(Name = CommonConstants.InnPaymentInfoViewComponentName)]
    public class InnPaymentInfoViewComponent : NopViewComponent
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly IGenericAttributeService _genericAttributeService;


        #endregion

        #region Ctor

        public InnPaymentInfoViewComponent(
            ISettingService settingService,
            IWorkContext workContext, 
            IGenericAttributeService genericAttributeService)
            
        {
            _settingService = settingService;
            _workContext = workContext;
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Methods

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int numberOfDigitsInn = 0;

            if (string.IsNullOrEmpty(await _genericAttributeService.GetAttributeAsync<string>(await _workContext.GetCurrentCustomerAsync(), CoreGenericAttributes.CustomerInnAttribute)))
            {
                numberOfDigitsInn = (await _settingService.LoadSettingAsync<FiluetCorePluginSettings>()).NumberOfDigitsInn;
            }
            return View("~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Views/Shared/Components/InnPaymentInfo/Default.cshtml", numberOfDigitsInn);
        }

        #endregion
    }
}
