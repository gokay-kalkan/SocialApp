

using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Database;
using EntityLayer.Entities;

namespace DataAccessLayer.EntityFramework
{
    public class EfPostLikeRepository : GenericRepository<PostLike, DataContext>, IPostLikeRepository
    {
        public EfPostLikeRepository(DataContext context) : base(context)
        {
        }
    }
}
