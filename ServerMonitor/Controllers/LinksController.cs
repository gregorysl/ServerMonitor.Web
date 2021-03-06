﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using ServerMonitor.Entities;

namespace ServerMonitor.Controllers
{
    [RoutePrefix("Links")]
    public class LinksController : BaseApi
    {

        [Route]
        public Response Get()
        {
            var response = new Response();
            var links = Settings.Data.Links;
            if (links == null)
            {
                response.ResponseStatus = Status.Error;
                response.AddErrorNotification("Configuration of links missing");
                return response;
            }

            response.Data = links;
            return response;
        }

        [Route]
        public async Task<Response> Post(UncheckedLink link)
        {
            var response = new Response();
            var links = Settings.Data.Links;
            if (links == null)
            {
                response.ResponseStatus = Status.Error;
                response.AddErrorNotification("Configuration of links missing");
                return response;
            }

            var resultLink = new Link(link);
            try
            {

                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(5);

                    if (!string.IsNullOrWhiteSpace(link.Username) && !string.IsNullOrWhiteSpace(link.Password))
                    {
                        var encoded =
                            Convert.ToBase64String(Encoding.ASCII.GetBytes($"{link.Username}:{link.Password}"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encoded);
                    }

                    var request = new HttpRequestMessage(HttpMethod.Head, new Uri(link.Url));
                    var linkResponse = await client.SendAsync(request);

                    resultLink.Message = linkResponse.StatusCode.ToString();
                    resultLink.Working = linkResponse.IsSuccessStatusCode;
                }
            }
            catch (Exception e)
            {
                resultLink.Working = false;
                resultLink.Message = GatherExceptions(e);
            }

            response.Data = resultLink;
            return response;
        }

        private static string GatherExceptions(Exception e)
        {
            var exception = $"{e.Message}\\r\\n";
            return e.InnerException != null ? exception + GatherExceptions(e.InnerException) : exception;
        }

    }
}