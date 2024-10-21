namespace MagicVilla_VillaAPI.Repository.implementation
{
    public class VillaRepository : GenericRepository<Villa> ,IVillaRepository
    {
        public VillaRepository(AppDbContext _context):base(_context) { }

        public override Task<bool> UpdateAsync(Villa entity)
        {
            entity.UpdatedDate = DateTime.Now;
            return base.UpdateAsync(entity);
        }
    }
}
