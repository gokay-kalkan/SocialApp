

using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using System.Linq.Expressions;

namespace BusinessLayer.Concrete
{
    public class MessageNotificationmanager : MessageNotificationService
    {
        IMessageNotificationRepository _repository;

        public MessageNotificationmanager(IMessageNotificationRepository repository)
        {
            _repository = repository;
        }

        public void Add(MessageNotification p)
        {
            _repository.Add(p);
        }

        public void Delete(MessageNotification p)
        {
            
            var value = _repository.GetById(p.MessageNotificationId);

            _repository.Delete(value);
        }

        public MessageNotification GetById(int id)
        {
            return _repository.GetById(id);
        }

        public List<MessageNotification> List()
        {
            return _repository.List();
        }

        public List<MessageNotification> List(Expression<Func<MessageNotification, bool>> filter)
        {
            return _repository.List(filter);
        }

        public void Update(MessageNotification p)
        {
            var value = _repository.GetById(p.MessageNotificationId);
            value.Content= p.Content;
            value.CreatedDate = DateTime.Now;
            _repository.Update(value);
        }
    }
}
