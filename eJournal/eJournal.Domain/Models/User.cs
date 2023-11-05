namespace eJournal.Domain.Models;

public partial class User
{
    public long UserId { get; set; }

    public string UserEmail { get; set; } = null!;

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Gender { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string Department { get; set; }

    public string Designation { get; set; }

    public string Phone { get; set; }

    public string UserName { get; set; }

    public string? Bio { get; set; }

    public long? ImageId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Blog> Blogs { get; } = new List<Blog>();

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

    public virtual Image? Image { get; set; }

    public virtual ICollection<Like> Likes { get; } = new List<Like>();

    public virtual ICollection<Notification> Notifications { get; } = new List<Notification>();
}
