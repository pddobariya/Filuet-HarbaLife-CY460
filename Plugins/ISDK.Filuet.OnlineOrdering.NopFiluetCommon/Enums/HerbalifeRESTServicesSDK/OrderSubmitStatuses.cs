using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.HerbalifeRESTServicesSDK;
using System.ComponentModel;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums.HerbalifeRESTServicesSDK
{
    [TypeConverter(typeof(CodeEnumConverter<OrderSubmitStatuses>))]
    public enum OrderSubmitStatuses
    {
        [Code("Success")]
        Success,

        [Code("Error")]
        Error
    }
}
