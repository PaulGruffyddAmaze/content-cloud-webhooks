using EPiServer;
using EPiServer.Logging;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http.Json;
using System.Net.Http;
using ILogger = EPiServer.Logging.ILogger;
using Azure;

namespace DeaneBarker.Optimizely.Webhooks.HttpProcessors
{
    public class WebhookHttpProcessor : IWebhookHttpProcessor
    {
        private readonly ILogger logger = LogManager.GetLogger(typeof(WebhookHttpProcessor));
        protected static readonly ConcurrentDictionary<string, HttpClient> HttpClientCache = new ConcurrentDictionary<string, HttpClient>();

        public virtual async Task<WebhookAttempt> ProcessAsync(HttpRequestMessage request)
        {
            var sw = Stopwatch.StartNew();
            var responseContent = string.Empty;
            try
            {
                logger.Debug($"Sending {request.Method} request to {(request.RequestUri?.AbsoluteUri?.Quoted() ?? string.Empty)}");
                var httpClient = GetHttpClientForHost(request.RequestUri);
                var response = await httpClient.SendAsync(request);
                sw.Stop();
                logger.Debug($"Response received in {sw.ElapsedMilliseconds}ms");
                return new WebhookAttempt(sw.ElapsedMilliseconds, (int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException e)
            { 
                logger.Error($"Exception encountered. {e.Message.Quoted()}");
                return new WebhookAttempt(sw.ElapsedMilliseconds, (int?)e.StatusCode ?? 0, e.Message);
            }
            catch (Exception e)
            {
                logger.Error($"Exception encountered. {e.Message.Quoted()}");
                return GetUnknownExceptionResponse(e.Message);
            }
        }

        public virtual WebhookAttempt Process(HttpWebRequest request)
        {
            HttpWebResponse response;
            var sw = Stopwatch.StartNew();
            try
            {
                logger.Debug($"Sending {request.Method} request to {request.RequestUri.AbsoluteUri.Quoted()}");
                response = (HttpWebResponse)request.GetResponse(); // This makes the actual HTTP call
                logger.Debug($"Response received in {sw.ElapsedMilliseconds}ms");
                return new WebhookAttempt(sw.ElapsedMilliseconds, (int)response.StatusCode, GetResponseContent(response));
            }
            catch (WebException e)
            {
                logger.Error($"Exception encountered. {e.Message.Quoted()}");
                if (e.Response == null)
                {
                    return GetUnknownExceptionResponse("NULL response");
                }

                try
                {
                    response = (HttpWebResponse)e.Response;
                    return new WebhookAttempt(sw.ElapsedMilliseconds, (int)response.StatusCode, GetResponseContent(response));
                }
                catch(Exception nestedException)
                {
                    logger.Error($"Exception encountered when handling exception response. {nestedException.Message.Quoted()}");
                    return GetUnknownExceptionResponse(nestedException.Message);
                }
            }
        }

        // I broke this out to its own method so that if someone inherits this class and overrides Process, they don't have to figure out this code
        protected string GetResponseContent(HttpWebResponse response)
        {
            var responseStream = new StreamReader(response.GetResponseStream());
            return responseStream.ReadToEndAsync().Result;
        }

        // Sometimes we have to manufacture a webhook response...
        protected WebhookAttempt GetUnknownExceptionResponse(string text)
        {
            return new WebhookAttempt(0, 500, text);
        }

        private HttpClient GetHttpClientForHost(Uri uri)
        {
            var key = $"{uri.Scheme}://{uri.DnsSafeHost}:{uri.Port}";

            return HttpClientCache.GetOrAdd(key, k =>
            {
                var handler = new SocketsHttpHandler()
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    PooledConnectionLifetime = TimeSpan.FromMinutes(2)
                };

                var client = new HttpClient(handler)
                {
                    BaseAddress = new Uri(uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped))
                };
                var sp = ServicePointManager.FindServicePoint(uri);
                sp.ConnectionLeaseTimeout = 60 * 1000; // 1 minute
                return client;
            });
        }

    }
}