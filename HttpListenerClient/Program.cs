using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Http.Listener
{
    class Program
    {
        static readonly string _myName = "ZHANTAI NURADINOV";
        static async Task Main(string[] args)
        {
            using(var listener = new HttpListener())
            {
                var prefixes = new[] { "http://localhost:8888/" };

                foreach (string s in prefixes)
                    listener.Prefixes.Add(s);

                await ListenAsync(listener);
            }
        }

        public static async Task ListenAsync(HttpListener listener)
        {
            listener.Start();
            Console.WriteLine("Listening...");
            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerResponse response = context.Response;

                var requestUrl = context.Request.RawUrl;
                if (requestUrl == "/Exit")
                {
                    await SendAsync(response, "The app is stopped!", HttpStatusCode.OK);
                    break;
                }
                else if (requestUrl == "/favicon.ico")
                    continue;

                string body = null;
                HttpStatusCode code = HttpStatusCode.OK;
                switch (requestUrl)
                {
                    case "/MyName":
                        body = _myName;
                        break;

                    case "/Information":
                        code = HttpStatusCode.NonAuthoritativeInformation; // If 1*** status code is passed - the request is awaited forever in the client-side
                        body = code.ToString();
                        break;

                    case "/Success":
                        body = code.ToString();
                        break;

                    case "/Redirection":
                        code = HttpStatusCode.Redirect;
                        body = code.ToString();
                        break;

                    case "/ClientError":
                        code = HttpStatusCode.BadRequest;
                        body = code.ToString();
                        break;
                    case "/ServerError":
                        code = HttpStatusCode.InternalServerError;
                        body = code.ToString();
                        break;

                    case "/MyNameByHeader":
                        response.Headers.Add("X-MyName", _myName);
                        body = _myName;
                        break;

                    case "/MyNameByCookies":
                        response.AppendCookie(new Cookie() 
                        { 
                            Name = "MyName", 
                            Value = _myName
                        });
                        body = _myName;
                        break;
                }

                await SendAsync(response, body, code);
            }

            Console.WriteLine("Stopping...");
            listener.Stop();
        }

        public static async Task SendAsync(HttpListenerResponse response, string body, HttpStatusCode code)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(body);
            response.StatusCode = (int)code;
            response.ContentType = "text/plain";
            response.ContentLength64 = bytes.Length;
            
            await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
            response.OutputStream.Close();
        }
    }
}
