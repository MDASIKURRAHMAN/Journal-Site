namespace eJournal.Web.Models
{
    public class BlogUpdateViewModel
    {
        public long BlogId { get; set; }
        public string BlogTitle { get; set; }
        public string BlogText { get; set; }
        public ICollection<IFormFile> Images { get; set; }
    }
}
