namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Helpers
{
    public interface IApfExtendedFunctionsHelper
    {
        #region Methods

        bool IsApfAdded();

        int GetAPFDueDateWarningPeriodDays();

        #endregion
    }
}
