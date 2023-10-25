using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using ModernRecrut.Favoris.API.Helpers;
using ModernRecrut.Favoris.API.Interfaces;
using ModernRecrut.Favoris.API.Models;

namespace ModernRecrut.Favoris.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GestionFavorisController : Controller
    {        
        private readonly IMemoryCache? _memoryCache;

        private readonly IUtilitaireService _utilitaireService;

        private readonly IDistributedCache _distributedCache;

        private string _cacheKey;

        public GestionFavorisController(IMemoryCache memoryCache, IUtilitaireService utilitaireService, IDistributedCache distributedCache)
        {
            _memoryCache = memoryCache;
            _utilitaireService = utilitaireService;
            _distributedCache = distributedCache;
            _cacheKey = IpAdresse.GetIpAdress();
        }

        public async Task CacheRedis()
        {
            //Obtention des données du cache
            string cacheDatetime = await _distributedCache.GetStringAsync("cacheDateTime");

            if (cacheDatetime == null)
            {
                cacheDatetime = DateTime.Now.ToString();

                //Définition des options de configurations du stockage des données dans le cache
                var options = new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromSeconds(10),
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(120),
                };

                //Ajout des données dans le cache
                _ = _distributedCache.SetStringAsync("cacheDateTime", cacheDatetime, options);
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<OffreEmploi>>> ObtenirFavoris()
        {
            //Verifie si l'information existe dans le cache
            var cache = (OffreFavoris)_memoryCache.Get(_cacheKey);
            if (cache != null)
            {
                return cache.Favoris.ToList();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Ajouter(OffreEmploi offreEmploi)
        {
            OffreFavoris offreFavoris = (OffreFavoris)_memoryCache.Get(_cacheKey);
            if (offreFavoris != null && !offreFavoris.Favoris.Any(o => o.Id == offreEmploi.Id))
            {
                offreFavoris.Favoris.Add(offreEmploi);
                int tailleTotal = _utilitaireService.ObtenirTailleListOffreEmploi(offreFavoris.Favoris);
                var cacheEntryOptionsTotal = new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(6),
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
                    Size = tailleTotal
                };
                _memoryCache.Set(_cacheKey, offreFavoris, cacheEntryOptionsTotal);
                return Ok();
            }
            else
            {
                offreFavoris = new OffreFavoris();
                offreFavoris.Favoris = new List<OffreEmploi>
                {
                    offreEmploi
                };
                int tailleTotal = _utilitaireService.ObtenirTailleListOffreEmploi(offreFavoris.Favoris);
                var cacheEntryOptionsTotal = new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(6),
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
                    Size = tailleTotal
                };
                _memoryCache.Set(_cacheKey, offreFavoris, cacheEntryOptionsTotal);
                return Ok();
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Supprimer(int id)
        {
            OffreFavoris offreFavoris = (OffreFavoris)_memoryCache.Get(_cacheKey);
            if (offreFavoris != null && offreFavoris.Favoris.Any(o => o.Id == id))
            {
                var offreAEnlever = offreFavoris.Favoris.FirstOrDefault(o => o.Id == id);
                offreFavoris.Favoris.Remove(offreAEnlever);
                int tailleTotal = _utilitaireService.ObtenirTailleListOffreEmploi(offreFavoris.Favoris);
                var cacheEntryOptionsTotal = new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(6),
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
                    Size = tailleTotal
                };
                _memoryCache.Set(_cacheKey, offreFavoris, cacheEntryOptionsTotal);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
