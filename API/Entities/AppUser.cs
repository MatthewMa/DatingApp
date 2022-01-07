using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("AppUsers")]
    public class AppUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }        
    }
}
