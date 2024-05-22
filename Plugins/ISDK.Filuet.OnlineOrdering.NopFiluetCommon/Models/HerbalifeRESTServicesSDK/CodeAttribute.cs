using System;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.HerbalifeRESTServicesSDK
{
    public class CodeAttribute : Attribute
    {
        #region Properties

        private readonly string _codeValue;

        public CodeAttribute(string codeValue)
        {
            _codeValue = codeValue;
        }

        public string DisplayCode => _codeValue;

        #endregion
    }
}
