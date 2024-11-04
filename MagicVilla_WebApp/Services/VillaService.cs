using MagicVilla_WebApp.Models;
using MagicVilla_WebApp.Models.Dtos;
using MagicVilla_WebApp.Services.IServices;

namespace MagicVilla_WebApp.Services
{
    public class VillaService : BaseService, IVillaService
    {
        public VillaService(IHttpClientFactory httpClientFactory , IConfiguration configuration) : base(httpClientFactory , configuration) 
        {
            
        }

    }
}
