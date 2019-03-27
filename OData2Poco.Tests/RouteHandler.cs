using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OData2Poco.Tests
{
    
    public class RouteHandler : DelegatingHandler
    {

        Action<HttpRequestMessage> _testingAction;

        //public SampleRouteHandler(HttpMessageHandler innerHandler,
        //    Action<HttpRequestMessage> testingAction) : base(innerHandler)
        //{
        //    _testingAction = testingAction;
            
        //}

        public RouteHandler(Action<HttpRequestMessage> testingAction)
        {
            _testingAction = testingAction;
        }

        protected override   Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            _testingAction(request);
            var resp = new HttpResponseMessage(HttpStatusCode.OK)
            {

                Content = new JsonContent(new
                {
                    Success = true,
                    Message = "Success"
                })
            };

            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(resp);
            return tsc.Task;

            //var response = await base.SendAsync(request, ct);
            //Console.WriteLine(response.Dump());
            //if (!response.IsSuccessStatusCode)
            //{
            //    Console.WriteLine("{0}\t{1}\t{2}", request.RequestUri,
            //        (int)response.StatusCode, response.Headers.Date);
            //}
            //return response;
        }
    }

    internal class JsonContent : HttpContent
    {
        private readonly MemoryStream _stream = new MemoryStream();
        public JsonContent(object value)
        {

            Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var jw = new JsonTextWriter(new StreamWriter(_stream)) { Formatting = Formatting.Indented };
            var serializer = new JsonSerializer();
            serializer.Serialize(jw, value);
            jw.Flush();
            _stream.Position = 0;

        }
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return _stream.CopyToAsync(stream);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _stream.Length;
            return true;
        }
    }
}
