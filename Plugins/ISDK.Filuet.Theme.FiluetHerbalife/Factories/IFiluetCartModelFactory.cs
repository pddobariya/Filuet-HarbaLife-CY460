using ISDK.Filuet.Theme.FiluetHerbalife.Models;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Factories
{
    public interface IFiluetCartModelFactory
    {
        #region Method

        Task<CartSummaryBarModel> PrepareCartSummaryBarModel();
        
        #endregion
    }
}
