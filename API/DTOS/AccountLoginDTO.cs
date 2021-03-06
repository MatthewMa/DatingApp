using System.ComponentModel.DataAnnotations;

namespace API.DTOS
{
    public class AccountLoginDTO
    {
        [Required]
        [MaxLength(20)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(16)]
        public string Password { get; set; }
    }
}
