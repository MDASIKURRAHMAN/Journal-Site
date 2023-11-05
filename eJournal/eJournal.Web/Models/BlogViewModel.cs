namespace eJournal.Web.Models
{
    public class BlogViewModel
    {
        public long BlogId { get; set; }
        public string BlogTitle { get; set;}
        public string BlogText { get; set;}
        public DateTime CreatedAt { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public bool IsLikedByMe { get; set; }
        public int TotalLikes { get; set; }
        public int TotalComments { get; set; }
    }
}
