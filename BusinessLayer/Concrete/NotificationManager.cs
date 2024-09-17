using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class NotificationManager : NotificationService
    {
        INotificationRepository _notificationRepository;

        public NotificationManager(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public void Add(Notification p)
        {
            p.CreatedDate = DateTime.Now;
            _notificationRepository.Add(p);
        }

        public void Delete(Notification p)
        {
            var noti = _notificationRepository.GetById(p.NotificationId);
            _notificationRepository.Delete(noti);
        }

        public Notification GetById(int id)
        {
            return _notificationRepository.GetById(id);
        }

        public List<Notification> List()
        {
            return _notificationRepository.List();
        }

        public List<Notification> List(Expression<Func<Notification, bool>> filter)
        {
            return _notificationRepository.List(filter);
        }

        public void Update(Notification p)
        {
            var value = _notificationRepository.GetById(p.NotificationId);
            _notificationRepository.Update(value);

        }
    }
}
