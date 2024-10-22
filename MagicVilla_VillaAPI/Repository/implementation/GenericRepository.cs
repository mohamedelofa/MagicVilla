
namespace MagicVilla_VillaAPI.Repository.implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        private readonly DbSet<T> _db;
        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }

        public async Task<bool> CreateAsync(T entity)
        {
            await _db.AddAsync(entity);
            return await SaveAsync();
            
        }


        public async Task<bool> DeleteAsync(T entity)
        {
            _db.Remove(entity);
            return await SaveAsync();
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> filter , bool tracked)
        {
            if(tracked)
                return await _db.FirstOrDefaultAsync(filter);
            return await _db.AsNoTracking().FirstOrDefaultAsync(filter);
        }

        public Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
        {
            if (filter is not null) return _db.Where(filter).ToListAsync();
            return _db.ToListAsync();
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            _db.Update(entity);
            return await SaveAsync();
        }

        protected virtual async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        
    }
}
