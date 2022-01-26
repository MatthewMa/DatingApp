namespace API.Tools
{
    public class LikeParams: PaginationParams
    {        
        /**
         * Internal User
         */
        public string Predicate { get; set; } = "liked";
        public string OrderBy { get; set; } = "age";
        public int UserId { get; set; }
    }
}
