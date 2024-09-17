
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Database;
using EntityLayer.Entities;

namespace DataAccessLayer.EntityFramework
{
    public class EfMessageNotificationRepository : GenericRepository<MessageNotification, DataContext>, IMessageNotificationRepository
    {
        public EfMessageNotificationRepository(DataContext context) : base(context)
        {
        }
    }
}
