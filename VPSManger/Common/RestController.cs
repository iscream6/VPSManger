using MaterialDesignThemes.Wpf;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using VPSManger.DTO;

namespace VPSManger.Common
{
    enum RequestType
    {
        GET,
        POST, 
        PUT,
        DELETE
    }

    class RestController : Singleton<RestController>
    {
        /// <summary>
        /// GET
        /// </summary>
        /// <param name="apiURL"></param>
        /// <returns></returns>
        public string RequestHttpsGET(string apiURL)
        {
            string sResponseValue = "";

            try
            {
                using (HttpClientHandler clientHandler = new() { ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true })
                using (var client = new HttpClient(clientHandler))
                {
                    client.BaseAddress = new Uri(apiURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    Task<string> response = client.GetStringAsync(apiURL);
                    
                    response.Wait();

                    if (response.Exception != null)
                    {
                        Console.WriteLine(response.Exception.ToString());
                    }

                    sResponseValue = response.Result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return sResponseValue;
        }

        /// <summary>
        /// POST
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiURL"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool RequestHttpsPOST<T>(string apiURL, T t)
        {
            bool bReturnVal = false;

            try
            {
                using (HttpClientHandler clientHandler = new() { ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true })
                using (var client = new HttpClient(clientHandler))
                {
                    client.BaseAddress = new Uri(apiURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = client.PostAsJsonAsync(apiURL, t).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        bReturnVal = true;
                    }
                    else
                    {
                        Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                        bReturnVal = false;

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                bReturnVal = false;
            }

            return bReturnVal;
        }

        /// <summary>
        /// DELETE
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiURL"></param>
        /// <returns></returns>
        public bool RequestHttpsDelete(string apiURL)
        {
            bool bReturnVal = false;

            try
            {
                using (HttpClientHandler clientHandler = new() { ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true })
                using (var client = new HttpClient(clientHandler))
                {
                    client.BaseAddress = new Uri(apiURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = client.DeleteAsync(apiURL).Result;
                    //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);

                    if (response.IsSuccessStatusCode)
                    {
                        bReturnVal = true;
                    }
                    else
                    {
                        Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                        bReturnVal = false;

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                bReturnVal = false;
            }

            return bReturnVal;
        }
    }
}
