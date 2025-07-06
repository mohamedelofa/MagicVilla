using MagicVilla_VillaAPI.Services.Interface;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace MagicVilla_VillaAPI.Services.Implementation
{
	public class CacheService(IDistributedCache cache) : ICacheService
	{
		private readonly JsonSerializerOptions _options = new()
		{
			PropertyNamingPolicy = null,
			WriteIndented = true,
			AllowTrailingCommas = true
		};

		public async Task RemoveAsync(string key)
		{
			await cache.RemoveAsync(key);
		}

		public async Task SetAsync<T>(string key, T value, DateTimeOffset expireTime)
		{
			TimeSpan expirespan = expireTime.DateTime.Subtract(DateTimeOffset.Now.DateTime);
			if (expirespan < TimeSpan.Zero)
			{
				throw new ArgumentException("Expiration time must be in the future.", nameof(expireTime));
			}
			var serializedValue = JsonSerializer.Serialize(value, _options);
			var cacheOptions = new DistributedCacheEntryOptions()
			{
				AbsoluteExpirationRelativeToNow = expirespan,
			};
			await cache.SetStringAsync(key, serializedValue, cacheOptions);
		}

		public async Task<T?> GetAsync<T>(string key)
		{
			var serializedValue = await cache.GetStringAsync(key);
			var value = serializedValue is null ? default : JsonSerializer.Deserialize<T>(serializedValue, _options);
			return value;
		}

		public async Task RemoveAllAsync(IEnumerable<string> keys)
		{
			foreach (var key in keys)
			{
				await cache.RemoveAsync(key);
			}
		}
	}
}
