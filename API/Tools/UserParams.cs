namespace API.Tools
{
    public class UserParams: PaginationParams
    {
        /**
         * Internal User
         */
        public string CurrentUserName { get; set; }
        public string Gender { get; set; }
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 150;
        public string OrderBy { get; set; } = "age";
    }
}
