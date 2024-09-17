using EntityLayer.Entities;
using System.Linq.Expressions;

namespace BusinessLayer.Abstract
{
    public interface FollowRequestService:GenericService<FollowRequest>
    {
        FollowRequest GetByPredicate(Expression<Func<FollowRequest, bool>> predicate);
    }
}
