using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DtoLayer.Dtos.FollowerDtos
{
    public class FollowerDto
    {
        public int FollowerId { get; set; }
        public string FollowerUserId { get; set; }
        public string FollowingUserId { get; set; }

        public string FollowingUserName { get; set; }  // Takip edilen kullanıcının adı
    }
}
