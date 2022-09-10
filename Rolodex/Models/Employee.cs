using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rolodex.Models
{
    public class Employee : IEntity
    {
        [Key]
        [Column("EmployeeID")]
        public int Id { get; set; }
        [StringLength(255)]
        public string FirstName { get; set; }
        [StringLength(255)]
        public string LastName { get; set; }
        [StringLength(255)]
        public string JobTitle { get; set; }
        [StringLength(255)]
        public string Email { get; set; }
    }
}
