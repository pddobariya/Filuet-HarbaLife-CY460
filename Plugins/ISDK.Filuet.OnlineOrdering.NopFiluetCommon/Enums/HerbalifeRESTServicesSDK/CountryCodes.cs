using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.HerbalifeRESTServicesSDK;
using System.ComponentModel;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums.HerbalifeRESTServicesSDK
{
    [TypeConverter(typeof(CodeEnumConverter<CountryCodes>))]
    public enum CountryCodes
    {
        [Code("RU")]
        Russia = 1,

        [Code("LV")]
        Latvia = 2,

        [Code("LT")]
        Lithuania = 3,

        [Code("EE")]
        Estonia = 4
    }
}