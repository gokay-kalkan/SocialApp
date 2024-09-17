
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Database;
using EntityLayer.Entities;

namespace DataAccessLayer.EntityFramework
{
    public class EfMessageRepository : GenericRepository<Message, DataContext>, IMessageRepository
    {
        public EfMessageRepository(DataContext context) : base(context)
        {
        }
    }
}
