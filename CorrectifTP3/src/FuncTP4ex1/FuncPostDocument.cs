using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;
using ModernRecrut.Documents.API.Interfaces;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Functions.Worker.Http;

namespace FuncTP4ex1
{

    public class FuncPostDocument
    {
        private static readonly IGenererNom _genererNom;
        private readonly ILogger _logger;

        public FuncPostDocument(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<FuncPostDocument>();
        }

        [Function("FuncPostDocument")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }
    }
}

