using System.ComponentModel.DataAnnotations;

namespace eJournal.Web.Models
{
    public class UserViewModel
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
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ImagePath { get; set; }
        public IFormFile Image { get; set; }
        public long ImageId { get; set; }

    }
}
