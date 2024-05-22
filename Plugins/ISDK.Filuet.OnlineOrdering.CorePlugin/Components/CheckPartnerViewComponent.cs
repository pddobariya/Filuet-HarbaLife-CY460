using ISDK.Filuet.OnlineOrdering.CorePlugin.Services;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Orders;
using Nop.Web.Framework.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Components
{
    [ViewComponent(Name = CommonConstants.CheckPartnerViewComponentName)]
    public class CheckPartnerViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly _1SServiceWrapper _1sServiceWrapper;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICategoryService _categoryService;
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Ctor

        public CheckPartnerViewComponent(
            IWorkContext workContext,
            _1SServiceWrapper _1SServiceWrapper, 
            IShoppingCartService shoppingCartService,
            ICategoryService categoryService,
            IGenericAttributeService genericAttributeService)
        {
            _workContext = workContext;
            _1sServiceWrapper = _1SServiceWrapper;
            _shoppingCartService = shoppingCartService;
            _categoryService = categoryService;
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Methods

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            var shoppingCart = await _shoppingCartService.GetShoppingCartAsync(currentCustomer);
            List<ShoppingCartItem> cart = shoppingCart
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .ToList();
            
            if (await cart.AnyAwaitAsync(async item => await(await _categoryService.GetProductCategoriesByProductIdAsync(item.ProductId)).AnyAwaitAsync(async x =>
                await _genericAttributeService.GetAttributeAsync<Category, CategoryTypeEnum>(x.CategoryId, CategoryAttributeNames.CategoryType) ==
                CategoryTypeEnum.Ticket)))
                return Content(string.Empty);
            var distributorId =await currentCustomer.GetDistributorIdAsync();
            var flag = _1sServiceWrapper.CheckPartner(distributorId);
            return View("~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Views/Shared/Components/CheckPartner/Default.cshtml", flag);
        }

        #endregion
    }
}
