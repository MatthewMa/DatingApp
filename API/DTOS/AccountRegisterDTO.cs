using System.ComponentModel.DataAnnotations;

namespace API.DTOS
{
    public class AccountRegisterDTO
    {
        [Required]
        [MaxLength(16)]
        [MinLength(4)]
        public string UserName { get; set; }
        [Required]       
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,16}$")]
        public string Password { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        [MaxLength(50)]
        [MinLength(2)]
        public string KnownAs { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [MaxLength(40)]
        [MinLength(1)]
        public string City { get; set; }
        [Required]
        [MaxLength(40)]
        [MinLength(1)]
        public string Country { get; set; }
    }
}
