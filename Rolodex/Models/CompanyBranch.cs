using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rolodex.Models
{
    public class CompanyBranch : IEntity
    {
        [Key]
        [Column("CompanyBranchId")]
        public int Id { get; set; }
        [StringLength(255)]
        public string Name { get; set; } = null!;

        [StringLength(255)]
        public string City { get; set; } = null!;

        [StringLength(255)]
        public string State { get; set; } = null!;
    }
}
