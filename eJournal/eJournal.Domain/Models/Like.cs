namespace eJournal.Domain.Models;

public partial class Like
{
    public long LikeId { get; set; }

    public long UserId { get; set; }

    public long? CommentId { get; set; }

    public long? BlogId { get; set; }

    public virtual Blog? Blog { get; set; }

    public virtual Comment? Comment { get; set; }

    public virtual User User { get; set; } = null!;
}
