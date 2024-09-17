

using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Database;
using EntityLayer.Entities;

namespace DataAccessLayer.EntityFramework
{
    public class EfPostRepository : GenericRepository<Post, DataContext>, IPostRepository
    {
        public EfPostRepository(DataContext context) : base(context)
        {
        }
    }
}
