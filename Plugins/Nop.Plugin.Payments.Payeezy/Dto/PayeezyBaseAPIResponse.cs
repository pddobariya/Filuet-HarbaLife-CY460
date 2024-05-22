using System;
using System.Collections.Generic;

namespace HBL.Baltic.OnlineOrdering.Payments.Payeezy
{
    public class PayeezyBaseAPIResponse
    {
        #region Methods

        public bool HasError
        {
            get
            {
                if (string.IsNullOrWhiteSpace(RawResponse))
                {
                    return false;
                }
                return RawResponse.ToLower().Trim().StartsWith("error:");
            }
        }

        public bool HasWarning
        {
            get
            {
                if (string.IsNullOrWhiteSpace(RawResponse))
                {
                    return false;
                }
                return RawResponse.ToLower().Trim().StartsWith("warning:");
            }
        }

        public bool IsFailed
        {
            get
            {
                return HasError || HasWarning;
            }
        }

        public string ServiceMessage
        {
            get
            {
                if (HasError || HasWarning)
                {
                    return HasError ? RawResponse.Substring(6) : RawResponse.Substring(8);
                }
                return string.Empty;
            }
        }


        public Dictionary<string, string> Data { get; set; }

        public string RawResponse { get; set; }

        protected string GetValue(string key)
        {
            if (Data.ContainsKey(key))
            {
                return Data[key];
            }
            return null;
        }

        protected int? GetIntValue(string key)
        {
            string strVal = GetValue(key);
            try
            {
                return Convert.ToInt32(strVal);
            }
            catch (Exception)
            {
            }
            return null;
        }

        protected double? GetDoubleValue(string key)
        {
            string strVal = GetValue(key);
            try
            {
                return Convert.ToDouble(strVal);
            }
            catch (Exception)
            {
            }
            return null;
        }

        public PayeezyBaseAPIResponse(string rawData)
        {
            Data = new Dictionary<string, string>();
            RawResponse = rawData;
            if (!HasError && !HasWarning)
            {
                foreach (string line in RawResponse.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] lineParts = line.Trim().Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    if (lineParts != null && lineParts.Length == 2)
                    {
                        Data.Add(lineParts[0].Trim(), lineParts[1].Trim());
                    }
                }
            }
        }

        public override string ToString()
        {
            string fullResponse = RawResponse != null ? RawResponse : string.Empty;
            string shortResponse = ServiceMessage;

            return string.Format("Message: {0}; Details: {1}", shortResponse, fullResponse);
        }

        #endregion
    }
}