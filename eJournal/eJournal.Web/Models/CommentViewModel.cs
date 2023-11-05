namespace eJournal.Web.Models
{
    public class CommentViewModel
    {
        public int CommentId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserImage { get; set; }
        public string CommentText { get; set; }
        public int LoggedInUserId { get; set; } 
        public int TotalLikes { get; set; }
        public bool IsLikedByMe { get; set; }
    }
}
