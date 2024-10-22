
namespace MagicVilla_VillaAPI.Repository.implementation
{
    public class VillaNumberRepository : GenericRepository<VillaNumber> , IVillaNumberRepository
    {
        public VillaNumberRepository(AppDbContext context) : base(context) { }
    }
}
