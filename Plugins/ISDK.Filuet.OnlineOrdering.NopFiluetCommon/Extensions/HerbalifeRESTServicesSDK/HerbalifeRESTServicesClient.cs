using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions.HerbalifeRESTServicesSDK
{
    public class HerbalifeRESTServicesClient
    {
        #region Fields

        public string _apiUrl;

        #endregion

        #region Ctor

        public HerbalifeRESTServicesClient(string apiUrl)
        {
            _apiUrl = apiUrl;
        }

        #endregion

        #region Methods

        public async Task<string> GetAsync(string accessToken, string path, NameValueCollection nvc)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = string.Format("{0}/{1}{2}", _apiUrl, path, nvc.ToQueryString());
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage resp =await client.GetAsync(url);
                string resultContent =await resp.Content.ReadAsStringAsync();
                return resultContent;
            }
        }

        #endregion
    }
}
