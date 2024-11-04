using MagicVilla_WebApp.Models;
using MagicVilla_WebApp.Models.Dtos;
using MagicVilla_WebApp.Services.IServices;

namespace MagicVilla_WebApp.Services
{
    public class VillaService : BaseService, IVillaService
    {
        //private readonly string _url = null!;
        public VillaService(IHttpClientFactory httpClientFactory , IConfiguration configuration) : base(httpClientFactory , configuration) 
        {
            //_url = configuration.GetValue<string>("ServiceUrls:VillaAPI");
        }



        //public async Task<T> CreateAsync<T>(CreateVillaDto dto)
        //{
        //    return await SendAsync<T>(new ApiRequest()
        //    {
        //        apiType = StaticDetails.ApiType.POST,
        //        Data = dto,
        //        Url = $"{_url}/api/VillaAPI"
        //    });

        //}

        //public async Task<T> DeleteAsync<T>(int id)
        //{
        //    return await SendAsync<T>(new ApiRequest()
        //    {
        //        apiType = StaticDetails.ApiType.DELETE,
        //        Url = $"{_url}/api/VillaAPI/{id}"
        //    });
        //}

        //public async Task<T> GetAllAsync<T>()
        //{
        //    return await SendAsync<T>(new ApiRequest()
        //    {
        //        apiType = StaticDetails.ApiType.GET,
        //        Url = $"{_url}/api/VillaAPI"
        //    });
        //}

        //public async Task<T> GetAsync<T>(int id)
        //{
        //    return await SendAsync<T>(new ApiRequest()
        //    {
        //        apiType = StaticDetails.ApiType.GET,
        //        Url = $"{_url}/api/VillaAPI/{id}"
        //    });
        //}

        //public async Task<T> UpdateAsync<T>(UpdateVillaDto dto , int id)
        //{
        //    return await SendAsync<T>(new ApiRequest()
        //    {
        //        apiType = StaticDetails.ApiType.PUT,
        //        Data = dto,
        //        Url = $"{_url}/api/VillaAPI/{id}"
        //    });
        //}
    }
}
