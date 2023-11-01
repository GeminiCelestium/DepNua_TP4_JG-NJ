using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ModernRecrut.Documents.API.Interfaces;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using ModernRecrut.Documents.API.Models;

public class FuncPostDocument
{
    private readonly IGenererNom _genererNom;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IConfiguration _config;
    public FuncPostDocument(BlobServiceClient blobClient, IConfiguration config, IGenererNom genererNom)
    {
        _blobServiceClient = blobClient;
        _config = config;
        _genererNom = genererNom;
    }

    public string GenererNomFichier(string codeUtilisateur, string typeDocument, string fileName)
    {
        //On genere un numéro aléatoire
        string numAleatoire = Guid.NewGuid().ToString();

        string extention = Path.GetExtension(fileName);
        // on prepare le nouveau nom du fichier 
        return codeUtilisateur + "_" + typeDocument + "_" + numAleatoire + $"{extention}";
    }

    [Function("FuncPostDocument")]
    public async Task Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "FuncPostDocument")] HttpRequestData req, Fichier fichier)
    {
        string nomFichier = _genererNom.GenererNomFichier(fichier.Id, fichier.TypeDocument.ToString(), fichier.FileName);

        var conteneur = _config.GetSection("StorageAccount").GetValue<string>("ConteneurDocuments");

        byte[] bytes = Convert.FromBase64String(fichier.DataFile);
        MemoryStream stream = new MemoryStream(bytes);

        IFormFile file = new FormFile(stream, 0, bytes.Length, fichier.Name, fichier.FileName);

        var blob = file.OpenReadStream();

        //Obtention d'un conteneur
        var containerClient = _blobServiceClient.GetBlobContainerClient(conteneur);


        BlobClient blobClient = containerClient.GetBlobClient(nomFichier);


        await blobClient.UploadAsync(blob, true);

    }
}
