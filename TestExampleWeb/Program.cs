using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestExampleWeb
{
    class HttpServer
    {
        private static HttpListener _listener;
        private static string url = "http://localhost:8000/";



        public static async Task HandleIncomingConnections()
        {
            while (true)
            {
                var ctx = await _listener.GetContextAsync(); //Hangs 

                var req = ctx.Request;
                var resp = ctx.Response;

                var content = "";
                if (req.Url.AbsolutePath == "/index.html")
                {
                    //Ex read from file...
                    content = System.IO.File.ReadAllText(".//index.html");
                    resp.StatusCode = 200;
                }
                else
                {
                    content = "<html><body>Finns ingen...</body></html>";
                    resp.StatusCode = 404;
                }

                // Write the response info
                byte[] data = Encoding.UTF8.GetBytes(content);
                resp.ContentType = "text/html";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                // Write out to the response stream (asynchronously), then close it
                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }

        public static  void Main()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(url);
            _listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            _listener.Close();
        }
    }


}
