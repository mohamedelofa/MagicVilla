
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

        public async Task<T?> GetAsync(Expression<Func<T, bool>> filter , bool tracked , string? includeProperties = null)
        {
            IQueryable<T> query = _db;
            if(includeProperties is not null)
            {
                foreach(var property in includeProperties.Split(',' , StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries))
                {
                    query = query.Include(property);
                }
            }
            if(tracked)
                return await query.FirstOrDefaultAsync(filter);
            return await query.AsNoTracking().FirstOrDefaultAsync(filter);
        }

        public Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null , string? includeProperties = null)
        {
            IQueryable<T> query = _db;
			if (includeProperties is not null)
			{
				foreach (var property in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(property);
				}
			}
			if (filter is not null) return query.Where(filter).ToListAsync();
            return query.ToListAsync();
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
