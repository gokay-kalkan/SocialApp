

using EntityLayer.Entities;
using System.Linq.Expressions;

namespace BusinessLayer.Abstract
{
    public interface FollowerService:GenericService<Follower>
    {
        public Follower GetByPredicate(Expression<Func<Follower, bool>> predicate);
    }
}
