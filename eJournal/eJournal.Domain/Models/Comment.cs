namespace eJournal.Domain.Models;

public partial class Comment
{
    public long CommentId { get; set; }

    public long UserId { get; set; }

    public string CommentText { get; set; } = null!;

    public long? ParentCommentId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? BlogId { get; set; }

    public virtual Blog? Blog { get; set; }

    public virtual ICollection<Image> Images { get; } = new List<Image>();

    public virtual ICollection<Comment> InverseParentComment { get; } = new List<Comment>();

    public virtual ICollection<Like> Likes { get; } = new List<Like>();

    public virtual Comment? ParentComment { get; set; }

    public virtual User User { get; set; } = null!;
}
