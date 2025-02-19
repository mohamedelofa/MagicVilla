using MagicVilla_WebApp.Services.IServices;

namespace MagicVilla_WebApp.Services
{
	public class VillaNumberService : BaseService, IVillaNumberService
	{
		public VillaNumberService(IConfiguration configuration, IConsumeService consumeService)
			: base(configuration, consumeService)
		{

		}
	}
}
