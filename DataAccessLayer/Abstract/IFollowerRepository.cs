

using EntityLayer.Entities;
using System.Linq.Expressions;

namespace DataAccessLayer.Abstract
{
    public interface IFollowerRepository:IRepository<Follower>
    {
        Follower GetByPredicate(Expression<Func<Follower, bool>> predicate);
    }
}
