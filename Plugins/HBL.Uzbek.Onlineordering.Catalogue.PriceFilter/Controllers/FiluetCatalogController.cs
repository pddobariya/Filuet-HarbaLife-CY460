using HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Factories;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models.Catalog;
using System.Threading.Tasks;

namespace HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Controllers
{
    public class FiluetCatalogController : BasePluginController
    {
        #region Fields

        private readonly FiluetCatalogModelFactory _filuetCatalogModelFactory;

        #endregion

        #region Ctor

        public FiluetCatalogController(
            FiluetCatalogModelFactory filuetCatalogModelFactory)
        {
            _filuetCatalogModelFactory = filuetCatalogModelFactory;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> PriceFilter(int priceFilterId, CatalogProductsCommand command)
        {
            var model =await _filuetCatalogModelFactory.PreparePriceFilterModel(priceFilterId, command);
            return View("~/Plugins/Catalogue.PriceFilter/Views/FiluetCatalog/PriceFilter.cshtml", model);
        }

        #endregion
    }
}
