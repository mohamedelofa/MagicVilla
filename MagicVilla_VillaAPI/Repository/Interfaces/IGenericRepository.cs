namespace MagicVilla_VillaAPI.Repository.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<bool> CreateAsync(T entity);
        Task<List<T>> GetAllAsync(Expression<Func<T,bool>>? filter = null);
        Task<T?> GetAsync(Expression<Func<T,bool>> filter,bool tracked);
        Task<bool> DeleteAsync(T entity);
        Task<bool> UpdateAsync(T entity);


    }
}
