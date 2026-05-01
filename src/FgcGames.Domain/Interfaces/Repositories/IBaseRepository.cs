using System.Linq.Expressions;

public interface IBaseRepository<T> where T : class
{
    void Add(T entity);

    void Update(T entity);

    void Delete(T entity);

    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

    Task<IReadOnlyList<T>> GetPagedAsync(
        int pagina,
        int tamanhoPagina,
        Expression<Func<T, bool>>? predicate = null
    );

    Task<T?> GetByIdAsync(Guid id);

    //Task<IReadOnlyList<T>> GetAllAsync();

    Task<int> SaveChangesAsync();
}
