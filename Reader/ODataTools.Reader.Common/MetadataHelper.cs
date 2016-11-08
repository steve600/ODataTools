using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ODataTools.Reader.Common
{
    public static class MetadataHelper
    {
        public static string GetMetadata(Uri uri)
        {
            string result = string.Empty;

            using (HttpClientHandler handler = new HttpClientHandler())
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    client.BaseAddress = uri;

                    using (HttpResponseMessage response = client.GetAsync("$metadata").Result)
                    {
                        switch (response.StatusCode)
                        {
                            case HttpStatusCode.Unauthorized:
                                //result = GetMetadata(uri, );
                                break;
                            case HttpStatusCode.OK:
                            default:
                                result = response.Content.ReadAsStringAsync().Result;
                                break;
                        }
                    }
                }
            }

            return result;
        }

        public static string GetMetadata(Uri uri, ICredentials credentials)
        {
            string result = string.Empty;

            using (HttpClientHandler handler = new HttpClientHandler())
            {
                handler.Credentials = credentials;

                using (HttpClient client = new HttpClient(handler))
                {
                    // http://services.odata.org/V3/Northwind/Northwind.svc/

                    client.BaseAddress = uri;

                    using (HttpResponseMessage response = client.GetAsync("$metadata").Result)
                    {
                        //response.EnsureSuccessStatusCode();
                        result = response.Content.ReadAsStringAsync().Result;
                    }
                }
            }

            return result;
        }
    }
}
