using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;
using ModernRecrut.Documents.API.Interfaces;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;

namespace FuncTP4ex1
{

    public class FuncPostDocument
    {
        private static readonly IGenererNom _genererNom;

        [Function("FuncPostDocument")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string Connection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string containerName = Environment.GetEnvironmentVariable("acremploistp4");
            Stream myBlob = new MemoryStream();
            var file = req.Form.Files["File"];
            myBlob = file.OpenReadStream();
            var blobClient = new BlobContainerClient(Connection, containerName);
            var blob = blobClient.GetBlobClient(file.FileName);
            await blob.UploadAsync(myBlob);
            return new OkObjectResult("file uploaded successfylly");
        }

    }
}

