using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace HBL.Baltic.OnlineOrdering.Payments.Payeezy
{
    public class PayeezyClient
    {
        #region Fields

        private String _bankUrl;
        private String _certFile;
        private string _certStoreKey;

        #endregion

        #region Ctor

        public PayeezyClient(String bankUrl, String certFile, string certStoreKey)
        {
            this._bankUrl = bankUrl;
            this._certFile = "Plugins/Payments.Payeezy/3838521.pfx";//certFile;
            _certStoreKey = certStoreKey;
           
        }

        #endregion

        #region Methods

        public String CloseBusinessDay()
        {
            string postData = "command=b";
            return SendPost(postData);
        }



        public String Reverse(String trans_id, String amount)
        {
            string postData = "command=r" + "&trans_id=" + System.Web.HttpUtility.UrlEncode(trans_id) +
                        "&amount=" + amount;
            return SendPost(postData);
        }



        public String GetTransResult(String trans_id, String client_ip_addr)
        {
            string postData = "command=c" + "&trans_id=" + System.Web.HttpUtility.UrlEncode(trans_id) +
                    "&client_ip_addr=" + client_ip_addr;
            return SendPost(postData);
        }

        public String MakeDMSTrans(String trans_id, String amount, String currency, String client_ip_addr)
        {
            return MakeDMSTrans(trans_id, amount, currency, client_ip_addr, null);
        }

        public String MakeDMSTrans(String trans_id, String amount, String currency, String client_ip_addr, String description)
        {

            if (description != null)
            {
                description = "&description=" + System.Web.HttpUtility.UrlEncode(description);
            }
            else
            {
                description = "";
            }

            string postData = "command=t" + "&trans_id=" + System.Web.HttpUtility.UrlEncode(trans_id) +
                "&amount=" + amount + "&currency=" + currency +
                "&client_ip_addr=" + client_ip_addr + "&msg_type=DMS" + description;

            return SendPost(postData);

        }



        public String StartSMSTrans(String amount, String currency, String client_ip_addr)
        {
            return StartSMSTrans(amount, currency, client_ip_addr, null, null);
        }

        public String StartSMSTrans(String amount, String currency, String client_ip_addr, String description)
        {
            return StartSMSTrans(amount, currency, client_ip_addr, description, null);
        }

        public String StartSMSTrans(String amount, String currency, String client_ip_addr, String description, String language)
        {
            return StartTrans("v", amount, currency, client_ip_addr, description, language);
        }

        public String StartDMSAuth(String amount, String currency, String client_ip_addr)
        {
            return StartDMSAuth(amount, currency, client_ip_addr, null, null);
        }

        public String StartDMSAuth(String amount, String currency, String client_ip_addr, String description)
        {
            return StartDMSAuth(amount, currency, client_ip_addr, description, null);
        }

        public String StartDMSAuth(String amount, String currency, String client_ip_addr, String description, String language)
        {
            return StartTrans("a", amount, currency, client_ip_addr, description, language);
        }



        private String StartTrans(String command, String amount, String currency, String client_ip_addr, String description, String language)
        {

            if (description != null)
            {
                description = "&description=" + System.Web.HttpUtility.UrlEncode(description);
            }
            else
            {
                description = "";
            }
            if (language != null)
            {
                language = "&language=" + System.Web.HttpUtility.UrlEncode(language);
            }
            else
            {
                language = "";
            }

            String msg_type;

            if (command.Equals("a"))
            {
                msg_type = "DMS";
            }
            else
            {
                msg_type = "SMS";
            }
            
            string postData = "command=" + command + "&amount=" + amount + "&currency=" + currency +
                "&client_ip_addr=" + client_ip_addr + "&msg_type=" + msg_type + description + language;

            return SendPost(postData);
        }

        private String SendPost(String postData)
        {
            String respStr;
            HttpWebResponse resp = null;

            try
            {
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.CheckCertificateRevocationList = false;
                 
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_bankUrl);
                Console.WriteLine("Connecting to URL: " + _bankUrl);
                // read DER encoded client certificate and attach it to request object 
                // so it can be passed to the gateway as part of the SSL handshake

                X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                certStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, _certStoreKey, false);

                X509Certificate2 clientcert = (certCollection != null && certCollection.Count > 0) ? certCollection[0] : null;
                if (clientcert == null && !string.IsNullOrWhiteSpace(_certFile))
                {
                    clientcert = new X509Certificate2(Path.GetFullPath(_certFile), "qwerty", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
                }
                else if(clientcert == null)
                {
                    throw new Exception("Unable to find cerificate in Windows store for thumbprint: " + _certStoreKey);
                }
                
                request.ClientCertificates.Add(clientcert);
                

                // encode post data and set up the request
                byte[] postDataBytes = Encoding.UTF8.GetBytes(postData);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postDataBytes.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(postDataBytes, 0, postDataBytes.Length);
                requestStream.Close();

                // get response 
                resp = (HttpWebResponse)request.GetResponse();
                StreamReader responseReader = new StreamReader(resp.GetResponseStream(), Encoding.UTF8);
                respStr = responseReader.ReadToEnd();
                resp.Close();

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                Console.Error.WriteLine("\r\nThe request URI could not be found or was malformed");
                respStr = "The request URI could not be found or was malformed. File specified: " + " \r\n" + e.ToString();
            }
            finally
            {
                if (resp != null)
                {
                    resp.Close();
                }
            }
            return respStr;
        }

        #endregion
    }
}