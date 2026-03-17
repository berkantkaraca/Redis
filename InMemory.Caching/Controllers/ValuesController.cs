using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace InMemory.Caching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        readonly IMemoryCache _memoryCache;

        public ValuesController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        [HttpGet("set-name/{name}")]
        public void SetName(string name)
        {
            _memoryCache.Set("name", name);
        }

        [HttpGet("get-name")]
        public string GetName()
        {
            if (_memoryCache.TryGetValue<string>("name", out string name) && _memoryCache.Get<string>("name").Length > 3)
            {
                return name;
            }

            return "Cache verisi 3 karakterden az veya yok";
        }

        [HttpGet("remove-name")]
        public void RemoveName()
        {
            _memoryCache.Remove("name");
        }

        [HttpGet("setDate")]
        public void SetDate()
        {
            _memoryCache.Set<DateTime>("date", DateTime.Now, options: new()
            {
                //AbsoluteExpiration: Verinin oluşturulduktan tam 30 saniye sonra, erişilse de erişilmese de silineceğini belirtir.
                AbsoluteExpiration = DateTime.Now.AddSeconds(30),

                //SlidingExpiration: Veriye her erişildiğinde ömrünü 5 saniye uzatır (ancak toplam süre AbsoluteExpiration sınırını geçemez). 5 saniye boyunca erişilmezse, veri silinir.
                SlidingExpiration = TimeSpan.FromSeconds(5)
            });
        }

        [HttpGet("getDate")]
        public DateTime GetDate()
        {
            return _memoryCache.Get<DateTime>("date");
        }
    }
}
