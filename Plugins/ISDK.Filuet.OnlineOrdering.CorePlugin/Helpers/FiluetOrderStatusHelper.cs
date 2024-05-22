namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Helpers
{
    public static class FiluetOrderStatusHelper
    {
        public static string GetOrderStatusClass(string statusName)
        {
            var orderStatusClass = "orange";

            switch (statusName.ToLower())
            {
                case "оплачен":
                    orderStatusClass = "orange";
                    break;
                case "передан в обработку":
                    orderStatusClass = "blue";
                    break;
                case "передан в доставку":
                    orderStatusClass = "grey";
                    break;
                case "доставлен":
                    orderStatusClass = "green";
                    break;
            }

            return orderStatusClass;
        }
    }
}
