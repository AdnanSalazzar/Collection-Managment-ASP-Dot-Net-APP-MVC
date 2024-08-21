using System.ComponentModel.DataAnnotations;

namespace CollectionManagement.Models.Domain
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        public ICollection<ItemTag> ItemTags { get; set; } // New navigation property for many-to-many relationship
    }
}
