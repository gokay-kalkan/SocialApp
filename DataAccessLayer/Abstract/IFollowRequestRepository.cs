using EntityLayer.Entities;
using System.Linq.Expressions;

namespace DataAccessLayer.Abstract
{
    public interface IFollowRequestRepository:IRepository<FollowRequest>
    {
        FollowRequest GetByPredicate(Expression<Func<FollowRequest, bool>> predicate);
    }
}
