namespace eJournal.Domain.Models;

public partial class Notification
{
    public long NotificationId { get; set; }

    public long BlogId { get; set; }

    public long UserId { get; set; }

    public string NotificationText { get; set; } = null!;

    public bool IsChecked { get; set; }

    public DateTime CreateAt { get; set; }

    public virtual Blog Blog { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
