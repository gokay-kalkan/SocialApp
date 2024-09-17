using System.ComponentModel.DataAnnotations;

namespace BlazorServerUI.Data.UserDtos
{
    public class UpdateUserNameDto
    {
       
        public string Name { get; set; }

    
        public string Surname { get; set; }
        public string UserName { get; set; }
    }
}
