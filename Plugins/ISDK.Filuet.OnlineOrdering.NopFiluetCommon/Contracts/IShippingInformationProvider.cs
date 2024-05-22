using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.DTO;
using Nop.Services.Plugins;
using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts
{
    public interface IShippingInformationProvider : IPlugin
    {
        #region Properties

        IEnumerable<FormFieldMeta> GetAdditionalShippingFields();

        IEnumerable<string> GetHiddenShippingFields();

        #endregion
    }
}
