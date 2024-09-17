
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Database;
using EntityLayer.Entities;

namespace DataAccessLayer.EntityFramework
{
    public class EfCommentRepository : GenericRepository<Comment, DataContext>, ICommentRepository
    {
        public EfCommentRepository(DataContext context) : base(context)
        {
        }
    }
}
