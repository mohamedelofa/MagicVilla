namespace MagicVilla_VillaAPI.Services.Interface
{
	public interface ICacheService
	{
		Task SetAsync<T>(string key, T value, DateTimeOffset expireTime);
		Task<T?> GetAsync<T>(string key);
		Task RemoveAsync(string key);
		Task RemoveAllAsync(IEnumerable<string> keys);
	}
}
