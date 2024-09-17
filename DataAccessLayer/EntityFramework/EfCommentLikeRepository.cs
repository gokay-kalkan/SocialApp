

using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Database;
using EntityLayer.Entities;

namespace DataAccessLayer.EntityFramework
{
    public class EfCommentLikeRepository : GenericRepository<CommentLike, DataContext>, ICommentLikeRepository
    {
        public EfCommentLikeRepository(DataContext context) : base(context)
        {
        }
    }
}
