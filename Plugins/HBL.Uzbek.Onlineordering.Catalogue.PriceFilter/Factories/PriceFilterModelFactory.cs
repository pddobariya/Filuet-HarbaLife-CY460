using HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Domain;
using HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Models;
using HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Services;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Factories
{
    public class PriceFilterModelFactory
    {
        #region Fields

        private readonly PriceRangeFilterService _priceRangeFilterService;

        #endregion

        #region Ctor

        public PriceFilterModelFactory(PriceRangeFilterService priceRangeFilterService)
        {
            _priceRangeFilterService = priceRangeFilterService;
        }

        #endregion

        #region Method
        public virtual async Task<PriceRangeListModel> PreparePriceRangeListModelAsync(PriceRangeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get stores
            var stores = (await _priceRangeFilterService.GetPriceRangesAsync()).ToPagedList(searchModel);

            //prepare list model
            var model =await new PriceRangeListModel().PrepareToGridAsync(searchModel, stores, () =>
            {
                //fill in model values from the entity
                return stores.SelectAwait(async store =>
                {
                    var priceRangeModel = new PriceRangeModel();
                    priceRangeModel.Id = store.Id;
                    priceRangeModel.Name = store.Name;
                    priceRangeModel.MinPrice = store.MinPrice;
                    priceRangeModel.MaxPrice = store.MaxPrice;
                    priceRangeModel.OrderNumber = store.OrderNumber;
                    return await Task.FromResult(priceRangeModel);
                });
            });

            return model;
        }

        public virtual Task<PriceRangeSearchModel> PreparePriceRangeListSearchModelAsync(PriceRangeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        public async Task<PriceRangeModel> PrepareRangeModelAsync(PriceRangeModel model, PriceRange? priceRange)
        {
            if (priceRange != null)
            {
                if (model == null)
                {
                    //fill in model values from the entity
                    model = new PriceRangeModel();
                    model.Id = priceRange.Id;
                    model.Name = priceRange.Name;
                    model.MinPrice = priceRange.MinPrice;
                    model.MaxPrice = priceRange.MaxPrice;
                    model.OrderNumber = priceRange.OrderNumber;
                    
                }
            }
            return await Task.FromResult(model);
        }

        #endregion
    }
}
