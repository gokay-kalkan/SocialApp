

using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using System.Linq.Expressions;

namespace BusinessLayer.Concrete
{
    public class CommentManager : CommentService
    {
        ICommentRepository _commentRepository;

        public CommentManager(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }
        public void Add(Comment p)
        {
            p.CreatedDate = DateTime.Now;
           _commentRepository.Add(p);
        }

        public void Delete(Comment p)
        {
            var comment = _commentRepository.GetById(p.CommentId);
            _commentRepository.Delete(comment);
        }

        public Comment GetById(int id)
        {
            return _commentRepository.GetById(id);
        }

        public List<Comment> List()
        {
            return _commentRepository.List();
        }

        public List<Comment> List(Expression<Func<Comment, bool>> filter)
        {
            return _commentRepository.List(filter);
        }

        public void Update(Comment p)
        {
            var comment = _commentRepository.GetById(p.CommentId);
            comment.CreatedDate = DateTime.Now;
            comment.Content = p.Content;
            comment.PostId = p.PostId;
            _commentRepository.Update(comment);
        }
    }
}
