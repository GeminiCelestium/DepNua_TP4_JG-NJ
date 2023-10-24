using Microsoft.AspNetCore.Mvc;
using ModernRecrut.Documents.API.Interfaces;
using ModernRecrut.Documents.API.Models;
using ModernRecrut.Documents.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ModernRecrut.Documents.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GestionDocumentsController : ControllerBase
    {
        private IWebHostEnvironment _env;

        private readonly IStorageServiceHelper _storageHelper;

        public GestionDocumentsController(IWebHostEnvironment env, IStorageServiceHelper storageHelper)
        {
            _env = env;
            _storageHelper = storageHelper;
        }

        // GET: api/<GestionDocumentsController>
        [HttpGet("{id}")]
        public async Task<IEnumerable<string>> Get(string id)
        {
          return await _storageHelper.ObtenirCheminFichiers(id);
        }

        // POST api/<GestionDocumentsController>
        [HttpPost]
        public async Task<IActionResult> EnregistrementDocument(Fichier fichierRecu)
        {
            await _storageHelper.EnregistrerFichier(fichierRecu);
            return CreatedAtAction(nameof(EnregistrementDocument), fichierRecu.Id);
        }

        [HttpGet("any/{id}")]
        public bool AnyTypeDocumentPourUtilisateur(int id)
        {
            return true;
        }
    }
}
