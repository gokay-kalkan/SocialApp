
using System.Linq.Expressions;

namespace DataAccessLayer.Abstract
{
    public interface IRepository<T> where T : class, new()
    {
        List<T> List();

        void Add(T p);
        void Delete(T p);
        void Update(T p);
        T GetById(int id);
        Task<T> AddAsync(T entity);
        List<T> List(Expression<Func<T, bool>> filter);

    }
}
