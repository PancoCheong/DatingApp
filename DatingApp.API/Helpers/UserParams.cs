namespace DatingApp.API.Helpers
{
    public class UserParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1; //default to page 1 if no page number value
        private int pageSize = 10;
        public int PageSize  //propfull snippet
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; } //not to exceed Max Pagesize
        }

        public int UserId { get; set; }
        public string Gender { get; set; }
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 99;
        public string OrderBy { get; set; }
        public bool Likees { get; set; } = false;
        public bool Likers { get; set; } = false;
    }
}