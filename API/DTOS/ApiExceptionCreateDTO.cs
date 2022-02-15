namespace API.DTOS
{
    public class ApiExceptionCreateDTO
    {
        public int? StatusCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }
}
