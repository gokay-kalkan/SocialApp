using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Database;
using EntityLayer.Entities;
using System.Linq.Expressions;

namespace DataAccessLayer.EntityFramework
{
    public class EfFollowerRepository : GenericRepository<Follower, DataContext>, IFollowerRepository
    {
        DataContext context;
        public EfFollowerRepository(DataContext context) : base(context)
        {
            this.context = context;
        }

        public Follower GetByPredicate(Expression<Func<Follower, bool>> predicate)
        {
            return context.Followers.FirstOrDefault(predicate);
        }
    }
}
