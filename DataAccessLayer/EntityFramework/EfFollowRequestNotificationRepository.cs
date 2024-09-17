
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Database;
using EntityLayer.Entities;

namespace DataAccessLayer.EntityFramework
{
    public class EfFollowRequestNotificationRepository : GenericRepository<FollowRequestNotification, DataContext>, IFollowRequestNotificationRepository
    {
        public EfFollowRequestNotificationRepository(DataContext context) : base(context)
        {
        }
    }
}
