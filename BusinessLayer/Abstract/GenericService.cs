

using System.Linq.Expressions;

namespace BusinessLayer.Abstract
{
    public interface GenericService<T>
    {
        List<T> List();

        void Add(T p);
        void Delete(T p);
        void Update(T p);
        T GetById(int id);

        List<T> List(Expression<Func<T, bool>> filter);
    }
}
