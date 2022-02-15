using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class ApiException
    {
        [Key]
        public int Id { get; set; }
        public int? StatusCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
    }
}
