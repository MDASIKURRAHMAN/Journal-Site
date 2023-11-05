namespace eJournal.Domain.Models;

public partial class Image
{
    public long ImageId { get; set; }

    public string ImageName { get; set; } = null!;

    public string ImagePath { get; set; } = null!;

    public long? BlogId { get; set; }

    public long? CommentId { get; set; }

    public virtual Blog? Blog { get; set; }

    public virtual Comment? Comment { get; set; }

    public virtual ICollection<User> Users { get; } = new List<User>();
}
