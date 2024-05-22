using HBL.Baltic.OnlineOrdering.Payments.Payeezy;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Payments.Payeezy.Dto
{
    public static class PayeezyHelpers
    {
        #region Methods

        public static string GetResultMessage(this string resultCode)
        {
            ValidateStatus(resultCode);
            DisplayAttribute attr = resultCode.GetPropertyDisplayAttribute(typeof(PayeezyTransactionResults));
            string message = attr.Name;
            if (string.IsNullOrWhiteSpace(message))
            {
                message = resultCode.GetResultDescription();
            }
            return message;
        }

        public static string Get3DSecureMessage(this string status)
        {
            ValidateStatus(status);
            string message = status.GetPropertyDisplayAttribute(typeof(PayeezyTransaction3DSecureStatuses)).Name;
            if (string.IsNullOrWhiteSpace(message))
            {
                message = status.GetResultDescription();
            }
            return message;
        }

        public static string GetResultDescription(this string resultCode)
        {
            ValidateStatus(resultCode);
            return resultCode.GetPropertyDisplayAttribute(typeof(PayeezyTransactionResults)).Description;
        }

        public static string Get3DSecureDescription(this string status)
        {
            ValidateStatus(status);
            return status.GetPropertyDisplayAttribute(typeof(PayeezyTransaction3DSecureStatuses)).Description;
        }

        private static DisplayAttribute GetPropertyDisplayAttribute(this string prop, Type type)
        {

            DisplayAttribute attr = type.GetAttributeFrom<DisplayAttribute>(prop);
            if (attr == null)
            {
                throw new Exception("DisplayAttribute is missing for: " + prop);
            }
            return attr;
        }

        private static void ValidateStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                throw new Exception("Status is empty");
            }
        }

        #endregion
    }
}