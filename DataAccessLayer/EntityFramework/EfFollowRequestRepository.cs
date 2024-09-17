

using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Database;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccessLayer.EntityFramework
{
    public class EfFollowRequestRepository : GenericRepository<FollowRequest, DataContext>, IFollowRequestRepository
    {
        DataContext context;

        public EfFollowRequestRepository(DataContext context) : base(context)
        {
            this.context = context;
        }

        public FollowRequest GetByPredicate(Expression<Func<FollowRequest, bool>> predicate)
        {
            return context.FollowRequests.FirstOrDefault(predicate);
        }
    }
}
