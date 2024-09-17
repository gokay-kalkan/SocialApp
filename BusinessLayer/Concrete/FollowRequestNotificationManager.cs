
using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using System.Linq.Expressions;

namespace BusinessLayer.Concrete
{
    public class FollowRequestNotificationManager : FollowRequestNotificationService
    {
        IFollowRequestNotificationRepository _repository;

        public FollowRequestNotificationManager(IFollowRequestNotificationRepository repository)
        {
            _repository = repository;
        }

      
        public void Add(FollowRequestNotification p)
        {
            _repository.Add(p);
        }

       
        public void Delete(FollowRequestNotification p)
        {
            var value = _repository.GetById(p.FollowRequestNotificationId);
            _repository.Delete(value);
        }

        public FollowRequestNotification GetById(int id)
        {
            return _repository.GetById(id);
        }

        public List<FollowRequestNotification> List(Expression<Func<FollowRequestNotification, bool>> filter)
        {
            return _repository.List(filter);
        }

        public List<FollowRequestNotification> List()
        {
            return _repository.List();
        }

        public void Update(FollowRequestNotification p)
        {
            var value = _repository.GetById(p.FollowRequestNotificationId);
            value.CreatedDate = DateTime.Now;
            value.Type = p.Type;
            
            _repository.Update(value);
        }

       

       
    }
}
