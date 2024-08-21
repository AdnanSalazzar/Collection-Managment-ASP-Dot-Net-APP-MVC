using System.ComponentModel.DataAnnotations;

namespace CollectionManagement.Models.Domain
{
    public class Item
    {
        [Key]
        public int ItemId { get; set; }

        [Required]
        public int CollectionId { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        //public string Tags { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        public Collection Collection { get; set; }
        public ICollection<ItemCustomFieldValue> CustomFieldValues { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<ItemTag> Tags { get; set; } //= new List<ItemTag>();// New navigation property for many-to-many relationship
    }
}
