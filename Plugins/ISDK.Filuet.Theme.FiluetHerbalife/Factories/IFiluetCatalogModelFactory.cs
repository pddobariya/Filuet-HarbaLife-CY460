using ISDK.Filuet.Theme.FiluetHerbalife.Models;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Factories
{
    public interface IFiluetCatalogModelFactory : ICatalogModelFactory
    {
        #region Method

        Task<FiluetSearchModel> PrepareFiluetSearchModelAsync(FiluetSearchModel model, CatalogProductsCommand command);
        
        #endregion
    }
}
