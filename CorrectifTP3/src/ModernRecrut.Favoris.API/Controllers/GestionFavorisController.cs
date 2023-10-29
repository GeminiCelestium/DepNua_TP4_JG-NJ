using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using ModernRecrut.Favoris.API.Helpers;
using ModernRecrut.Favoris.API.Interfaces;
using ModernRecrut.Favoris.API.Models;
using Newtonsoft.Json;

namespace ModernRecrut.Favoris.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GestionFavorisController : Controller
    {        

        private readonly IUtilitaireService _utilitaireService;

        private readonly IDistributedCache _distributedCache;

        private string _cacheKey;

        public GestionFavorisController(IUtilitaireService utilitaireService, IDistributedCache distributedCache)
        {
            _utilitaireService = utilitaireService;
            _distributedCache = distributedCache;
            _cacheKey = IpAdresse.GetIpAdress();
        }       

        [HttpGet]
        public async Task<ActionResult<List<OffreEmploi>>> ObtenirFavoris()
        {
            // Verifies if the information exists in the cache
            var cachedData = await _distributedCache.GetStringAsync(_cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                var cache = JsonConvert.DeserializeObject<OffreFavoris>(cachedData);
                return cache.Favoris.ToList();
            }
            else
            {
                return NotFound();
            }
        }

        /*[HttpPost]
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
        }*/

        [HttpPost]
        public async Task<ActionResult> Ajouter(OffreEmploi offreEmploi)
        {
            var cachedData = await _distributedCache.GetStringAsync(_cacheKey);
            OffreFavoris offreFavoris;

            if (!string.IsNullOrEmpty(cachedData))
            {
                offreFavoris = JsonConvert.DeserializeObject<OffreFavoris>(cachedData);
            }
            else
            {
                offreFavoris = new OffreFavoris();
                offreFavoris.Favoris = new List<OffreEmploi>();
            }

            if (!offreFavoris.Favoris.Any(o => o.Id == offreEmploi.Id))
            {
                offreFavoris.Favoris.Add(offreEmploi);
                int tailleTotal = _utilitaireService.ObtenirTailleListOffreEmploi(offreFavoris.Favoris);

                var cacheEntryOptions = new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(6),
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                };

                await _distributedCache.SetStringAsync(_cacheKey, JsonConvert.SerializeObject(offreFavoris), cacheEntryOptions);
                return Ok();
            }
            else
            {
                return Ok();
            }
        }

        /*[HttpDelete("{id}")]
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
        }*/
        [HttpDelete("{id}")]
        public async Task<ActionResult> Supprimer(int id)
        {
            var cachedData = await _distributedCache.GetStringAsync(_cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                OffreFavoris offreFavoris = JsonConvert.DeserializeObject<OffreFavoris>(cachedData);

                if (offreFavoris.Favoris.Any(o => o.Id == id))
                {
                    var offreAEnlever = offreFavoris.Favoris.FirstOrDefault(o => o.Id == id);
                    offreFavoris.Favoris.Remove(offreAEnlever);
                    int tailleTotal = _utilitaireService.ObtenirTailleListOffreEmploi(offreFavoris.Favoris);

                    var cacheEntryOptions = new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromHours(6),
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                    };

                    await _distributedCache.SetStringAsync(_cacheKey, JsonConvert.SerializeObject(offreFavoris), cacheEntryOptions);
                    return Ok();
                }
            }

            return BadRequest();
        }
    }
}
