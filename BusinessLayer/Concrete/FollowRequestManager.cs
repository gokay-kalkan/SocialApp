

using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using System.Linq.Expressions;

namespace BusinessLayer.Concrete
{
    public class FollowRequestManager : FollowRequestService
    {
        IFollowRequestRepository _requestRepository;

        public FollowRequestManager(IFollowRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public void Add(FollowRequest p)
        {
            p.RequestDate = DateTime.Now;
            _requestRepository.Add(p);
        }

        public void Delete(FollowRequest p)
        {
            var data = _requestRepository.GetById(p.FollowRequestId);
            _requestRepository.Delete(data);
        }

        public FollowRequest GetById(int id)
        {
            return _requestRepository.GetById(id);
        }

        public FollowRequest GetByPredicate(Expression<Func<FollowRequest, bool>> predicate)
        {
            return _requestRepository.GetByPredicate(predicate);
        }

        public List<FollowRequest> List()
        {
            return _requestRepository.List();
        }

        public List<FollowRequest> List(Expression<Func<FollowRequest, bool>> filter)
        {
            return _requestRepository.List(filter);
        }

        public void Update(FollowRequest p)
        {
            var data = _requestRepository.GetById(p.FollowRequestId);
            data.RequestDate = DateTime.Now;
           
            _requestRepository.Update(data);
        }


    }
}
