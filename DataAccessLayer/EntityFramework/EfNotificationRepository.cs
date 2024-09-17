
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Database;
using EntityLayer.Entities;

namespace DataAccessLayer.EntityFramework
{
    public class EfNotificationRepository : GenericRepository<Notification, DataContext>, INotificationRepository
    {
        public EfNotificationRepository(DataContext context) : base(context)
        {
        }
    }
}
