using Microsoft.Build.Framework;

namespace eJournal.Web.Models
{
    public class CommentCreateViewModel
    {
        [Required]
        public string CommentText { get; set; }
    }
}
