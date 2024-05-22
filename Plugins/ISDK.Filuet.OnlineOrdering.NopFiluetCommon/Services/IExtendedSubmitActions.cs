using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services
{
    public interface IExtendedSubmitActions
    {
        #region Methods

        // string GetAditionalNotes();
        Task<string> GetAditionalNotes();

        #endregion
    }
}