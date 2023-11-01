using System.Collections.Generic;
using System.Net;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ModernRecrut.Documents.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FuncTP4ex1
{
    public class FuncGetURLFichier
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private readonly BlobServiceClient _blobServiceClient;
        public FuncGetURLFichier(ILoggerFactory loggerFactory, BlobServiceClient blobClient, IConfiguration config, IGenererNom genererNom)
        {
            _logger = loggerFactory.CreateLogger<FuncGetURLFichier>();
            _config = config;
            _blobServiceClient = blobClient;

        }
        //teset
        [Function("FuncGetURLFichier")]
        public async Task<IEnumerable<string>> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "FuncGetURLFichier")] HttpRequestData req,string idUtilisateur)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var conteneur = _config.GetSection("StorageAccount").GetValue<string>("ConteneurDocuments");
            var sasToken = _config.GetSection("StorageAccount").GetValue<string>("SasToken");

            List<string> urlDocuments = new List<string>();

            //Obtention d'un conteneur blob
            var containerClient = _blobServiceClient.GetBlobContainerClient(conteneur);

            //Lecture des bloc dans le conteneur
            await foreach (BlobItem blob in containerClient.GetBlobsAsync(prefix: idUtilisateur))
            {
                //on veut aller chercher le Uri des blobs
                BlobClient blobClient = containerClient.GetBlobClient(blob.Name);
                var uri = blobClient.Uri;
                urlDocuments.Add(uri.ToString() + "?" + sasToken);
            }
            return urlDocuments;
        }
    }
}
