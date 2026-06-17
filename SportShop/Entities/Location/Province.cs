using System.ComponentModel.DataAnnotations;

namespace SportShop.Entities.Location
{
    public class Province
    {
        [Key]
        [MaxLength(10)]
        public string Code { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public virtual ICollection<Commune> Communes { get; set; } = new List<Commune>();
    }
}
