using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.UI.Paging;
using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Models.Order
{
    public record OrderPagingFilteringModel : BasePageableModel
    {
        #region Ctor

        public OrderPagingFilteringModel()
        {
            AvailableSortOptions = new List<SelectListItem>();
            PageSizeOptions = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Query string
        /// </summary>
        [NopResourceDisplayName("Search.SearchTerm")]
        public string q { get; set; }

        public int? OrderBy { get; set; }

        public bool AllowProductSorting { get; set; }

        public IList<SelectListItem> AvailableSortOptions { get; set; }

        public bool AllowCustomersToSelectPageSize { get; set; }

        public IList<SelectListItem> PageSizeOptions { get; set; }

        public string SearchButtonLocation { get; set; }

        #endregion
    }
}
