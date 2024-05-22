namespace Filuet.Onlineordering.Shipping.Delivery.Extensions
{
    public static class JavaScriptStringExtensions
    {
        #region Methods

        public static string EscapeJavaScriptString(this string str)
        {
            return str.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\"", "\\\"").Trim();
        }

        #endregion
    }
}



