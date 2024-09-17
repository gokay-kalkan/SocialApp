

using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using System.Linq.Expressions;

namespace BusinessLayer.Concrete
{
    public class PostLikeManager : PostLikeService
    {
        IPostLikeRepository _likeRepository;

        public PostLikeManager(IPostLikeRepository likeRepository)
        {
            _likeRepository = likeRepository;
        }
        public void Add(PostLike p)
        {
            p.CreatedDate = DateTime.Now;
            _likeRepository.Add(p);
        }

        public void Delete(PostLike p)
        {
            var like = _likeRepository.GetById(p.PostLikeId);
            _likeRepository.Delete(like);
        }

        public PostLike GetById(int id)
        {
            return _likeRepository.GetById(id);
        }

        public List<PostLike> List()
        {
            return _likeRepository.List();
        }

        public List<PostLike> List(Expression<Func<PostLike, bool>> filter)
        {
            return _likeRepository.List(filter);
        }

        public void Update(PostLike p)
        {
            throw new NotImplementedException();
        }
    }
}
