using System.ComponentModel.DataAnnotations;

namespace CollectionManagement.Models.Domain
{
    public class Collection
    {
        [Key]
        public int CollectionId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; }

        public string ImageUrl { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        public User User { get; set; }
        public ICollection<Item> Items { get; set; }
        public ICollection<ItemCustomField> CustomFields { get; set; }
    }
}
