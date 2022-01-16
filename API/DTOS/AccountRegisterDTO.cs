using System.ComponentModel.DataAnnotations;

namespace API.DTOS
{
    public class AccountRegisterDTO
    {
        [Required]
        [MaxLength(16)]
        [MinLength(6)]
        public string UserName { get; set; }
        [Required]       
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,16}$")]
        public string Password { get; set; }
    }
}
