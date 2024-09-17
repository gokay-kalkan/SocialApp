
using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using System.Linq.Expressions;

namespace BusinessLayer.Concrete
{
    public class MessageManager : MessageService
    {
        IMessageRepository _messageRepository;

        public MessageManager(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public void Add(Message p)
        {
            p.CreatedDate= DateTime.Now;
            p.IsRead = false;
            _messageRepository.Add(p);
        }

        public void Delete(Message p)
        {
            var message = _messageRepository.GetById(p.MessageId);
            _messageRepository.Delete(message);
        }

        public Message GetById(int id)
        {
            return _messageRepository.GetById(id);
        }

        public List<Message> List()
        {
            return _messageRepository.List();
        }

        public List<Message> List(Expression<Func<Message, bool>> filter)
        {
            return _messageRepository.List(filter);
        }

        public void Update(Message p)
        {
            var message = _messageRepository.GetById(p.MessageId);
            message.SenderId = p.SenderId;
            message.ReceiverId = p.ReceiverId;
            message.Content = p.Content;
            message.CreatedDate = DateTime.Now;
            _messageRepository.Update(message);
        }
    }
}
