using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Http.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                CookieContainer cookies = new CookieContainer();
                HttpClientHandler handler = new HttpClientHandler { CookieContainer = cookies };

                using (var client = new HttpClient(handler))
                {
                    await GetMyNameAsync(client);

                    await GetStatusCodeAsync(client, HttpStatusCode.NonAuthoritativeInformation);
                    await GetStatusCodeAsync(client, HttpStatusCode.OK);
                    await GetStatusCodeAsync(client, HttpStatusCode.Redirect);
                    await GetStatusCodeAsync(client, HttpStatusCode.BadRequest);
                    await GetStatusCodeAsync(client, HttpStatusCode.InternalServerError);

                    await GetMyNameByHeaderAsync(client);

                    await GetMyNameByCookiesAsync(client, cookies);

                    await client.GetAsync("http://localhost:8888/Exit");
                    Console.WriteLine("The app is stopped");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

        private static async Task GetMyNameAsync(HttpClient client)
        {
            HttpResponseMessage response = await client.GetAsync("http://localhost:8888/MyName");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Result of /MyName method is ====>    " + responseBody);
        }

        private static async Task GetStatusCodeAsync(HttpClient client, HttpStatusCode code)
        {
            string method = "";
            switch (code)
            {
                case HttpStatusCode.NonAuthoritativeInformation:
                    method = "Information";
                    break;
                case HttpStatusCode.OK:
                    method = "Success";
                    break;
                case HttpStatusCode.Redirect:
                    method = "Redirection";
                    break;
                case HttpStatusCode.BadRequest:
                    method = "ClientError";
                    break;
                case HttpStatusCode.InternalServerError:
                    method = "ServerError";
                    break;
            }

            var fullUrl = "http://localhost:8888/" + method;
            HttpResponseMessage response = await client.GetAsync(fullUrl);
            var status = response.StatusCode;
            Console.WriteLine($"Status code of /{method} method is ====>    " + status.ToString());
        }

        private static async Task GetMyNameByHeaderAsync(HttpClient client)
        {
            HttpResponseMessage response = await client.GetAsync("http://localhost:8888/MyNameByHeader");
            response.EnsureSuccessStatusCode();
            var responseBody = response.Headers.GetValues("X-MyName");
            Console.WriteLine("Result of /MyNameByHeader method is ====>    " + string.Join(";", responseBody));
        }

        private static async Task GetMyNameByCookiesAsync(HttpClient client, CookieContainer cookies)
        {
            var uri = new Uri("http://localhost:8888/MyNameByCookies");
            HttpResponseMessage response = await client.GetAsync(uri);
            var result = cookies.GetCookies(uri).Cast<Cookie>().FirstOrDefault(x => x.Name == "MyName");
            Console.WriteLine("Result of /MyNameByCookie method is ====>    " + result.Value);
        }
    }
}
