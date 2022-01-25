using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    public class UserLike
    {
        public virtual AppUser SourceUser { get; set; }
        public int SourceUserId { get; set; }
        public virtual AppUser LikedUser { get; set; }
        public int LikedUserId { get; set; }
    }
}
