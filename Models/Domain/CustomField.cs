using System.ComponentModel.DataAnnotations;

namespace CollectionManagement.Models.Domain
{
    public class CustomField
    {
        [Key]
        public int CustomFieldId { get; set; }

        [Required]
        public int CollectionId { get; set; }

        [Required]
        [StringLength(256)]
        public string FieldName { get; set; }

        [Required]
        [StringLength(50)]
        public string FieldType { get; set; }

        // Navigation property to the related collection
        public Collection Collection { get; set; }

        // Optionally, a collection of ItemCustomFieldValues if needed
        public ICollection<ItemCustomFieldValue> ItemCustomFieldValues { get; set; }
    }
}
