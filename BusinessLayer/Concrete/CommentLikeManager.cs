

using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using System.Linq.Expressions;

namespace BusinessLayer.Concrete
{
    public class CommentLikeManager : CommentLikeService
    {
        ICommentLikeRepository _commentLikeRepository;

        public CommentLikeManager(ICommentLikeRepository commentLikeRepository)
        {
            _commentLikeRepository = commentLikeRepository;
        }

        public void Add(CommentLike p)
        {
            _commentLikeRepository.Add(p);
        }

        public void Delete(CommentLike p)
        {
            var value = _commentLikeRepository.GetById(p.CommentLikeId);
            _commentLikeRepository.Delete(value);
        }

        public CommentLike GetById(int id)
        {
            return _commentLikeRepository.GetById(id);
        }

        public List<CommentLike> List()
        {
            return _commentLikeRepository.List();
        }

        public List<CommentLike> List(Expression<Func<CommentLike, bool>> filter)
        {
            return _commentLikeRepository.List(filter);
        }

        public void Update(CommentLike p)
        {
            throw new NotImplementedException();
        }
    }
}
