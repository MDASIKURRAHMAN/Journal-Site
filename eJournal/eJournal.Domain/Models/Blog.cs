namespace eJournal.Domain.Models;

public partial class Blog
{
    public long BlogId { get; set; }

    public string BlogTitle { get; set; } = null!;

    public string BlogText { get; set; } = null!;

    public long UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

    public virtual ICollection<Image> Images { get; } = new List<Image>();

    public virtual ICollection<Like> Likes { get; } = new List<Like>();

    public virtual ICollection<Notification> Notifications { get; } = new List<Notification>();

    public virtual User User { get; set; } = null!;
}
