using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DtoLayer.Dtos.UserDtos
{
    public class UpdateUserNameDto
    {
       
        public string Name { get; set; }

    
        public string Surname { get; set; }

    
        public string UserName { get; set; }
    }
}
