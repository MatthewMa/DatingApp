using System.ComponentModel.DataAnnotations;

namespace API.DTOS
{
    public class MessageCreateDTO
    {
        [Required]
        public string RecipientUserName { get; set; }
        [Required]
        public string Content { get; set; }
    }
}
