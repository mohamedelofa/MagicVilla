namespace MagicVilla_VillaAPI.Helpers
{
	public static class RedisKeys
	{
		public const string VillaKey = "Villa_";
		public static string VillasKey(int occupancy, int pageSize, int pageNumber)
		{
			AllVillasKeys.Add($"Villas_Occupancy_{occupancy}_PageSize_{pageSize}_PageNumber_{pageNumber}");
			return $"Villas_Occupancy_{occupancy}_PageSize_{pageSize}_PageNumber_{pageNumber}";
		}
		public static List<string> AllVillasKeys = new List<string>();
	}
}
