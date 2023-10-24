using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ModernRecrut.Documents.API.Interfaces;
using ModernRecrut.Documents.API.Models;
using ModernRecrut.Documents.Interfaces;
using System.Reflection.Metadata;

namespace ModernRecrut.Documents.API.Helpers
{
    public class StorageServiceHelper : IStorageServiceHelper
    {
        private readonly IGenererNom _genererNom;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IConfiguration _config;
        public StorageServiceHelper(BlobServiceClient blobClient, IConfiguration config, IGenererNom genererNom)
        {
            _blobServiceClient = blobClient;
            _config = config;
            _genererNom = genererNom;
        }

        public async Task EnregistrerFichier(Fichier fichier)
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

        public async Task<IEnumerable<string>> ObtenirCheminFichiers(string idUtilisateur)
        {
            var conteneur = _config.GetSection("StorageAccount").GetValue<string>("ConteneurDocuments");
            var sasToken = _config.GetSection("StorageAccount").GetValue<string>("SasToken");

            List<string> urlDocuments = new List<string>();

            //Obtention d'un conteneur blob
            var containerClient = _blobServiceClient.GetBlobContainerClient(conteneur);

            //Lecture des bloc dans le conteneur
            await foreach (BlobItem blob in containerClient.GetBlobsAsync(prefix:idUtilisateur))
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
