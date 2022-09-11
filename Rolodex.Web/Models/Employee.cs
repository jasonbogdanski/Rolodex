using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Rolodex.Web.Pages.Employee;

namespace Rolodex.Web.Models
{
    public class Employee : IEntity
    {
        [Key]
        [Column("EmployeeID")]
        public int Id { get; set; }
        [StringLength(255)]
        public string FirstName { get; set; } = null!;

        [StringLength(255)]
        public string LastName { get; set; } = null!;

        [StringLength(255)]
        public string JobTitle { get; set; } = null!;

        [StringLength(255)]
        public string Email { get; set; } = null!;

        [Required]
        public CompanyBranch CompanyBranch { get; set; } = null!;

        public void Handle(CreateEdit.Command message)
        {
            FirstName = message.FirstName;
            LastName = message.LastName;
            JobTitle = message.JobTitle;
            Email = message.Email;
            CompanyBranch = message.CompanyBranch;
        }
    }
}
