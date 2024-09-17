
using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using System.Linq.Expressions;

namespace BusinessLayer.Concrete
{
    public class FollowerManager : FollowerService
    {
        IFollowerRepository _followerRepository;

        public FollowerManager(IFollowerRepository followerRepository)
        {
            _followerRepository = followerRepository;
        }
        public void Add(Follower p)
        {
            
            _followerRepository.Add(p);
        }

        public void Delete(Follower p)
        {
            var follower = _followerRepository.GetById(p.FollowerId);
            _followerRepository.Delete(follower);
        }

        public Follower GetById(int id)
        {
            return _followerRepository.GetById(id);
        }

        public Follower GetByPredicate(Expression<Func<Follower, bool>> predicate)
        {
            return _followerRepository.GetByPredicate(predicate);
        }

        public List<Follower> List()
        {
            return _followerRepository.List();
        }

        public List<Follower> List(Expression<Func<Follower, bool>> filter)
        {
            return _followerRepository.List(filter);
        }

        public void Update(Follower p)
        {
            throw new NotImplementedException();
        }
    }
}
