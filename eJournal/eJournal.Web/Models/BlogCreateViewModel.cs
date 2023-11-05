using System.ComponentModel.DataAnnotations;

namespace eJournal.Web.Models
{
    public class BlogCreateViewModel
    {
        [Required]
        public string BlogTitle { get; set; }
        [Required]
        public string BlogText { get; set;}

        public ICollection<IFormFile> Images { get; set; }
    }
}
