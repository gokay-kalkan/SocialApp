
using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using System.Linq.Expressions;

namespace BusinessLayer.Concrete
{
    public class PostManager : PostService
    {
        IPostRepository _postRepository;

        public PostManager(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public void Add(Post p)
        {
            p.CreatedDate = DateTime.Now;
           _postRepository.Add(p);
        }

        public void Delete(Post p)
        {
            var post = _postRepository.GetById(p.PostId);
            _postRepository.Delete(post);
        }

        public Post GetById(int id)
        {
            return _postRepository.GetById(id);
        }

        public List<Post> List()
        {
            return _postRepository.List();
        }

        public List<Post> List(Expression<Func<Post, bool>> filter)
        {
            return _postRepository.List(filter);
        }

        public void Update(Post p)
        {
            var post = _postRepository.GetById(p.PostId);
            post.Content = p.Content;
            post.MediaUrl = p.MediaUrl;
            post.CreatedDate = DateTime.Now;

            _postRepository.Update(post);
            
        }
    }
}
