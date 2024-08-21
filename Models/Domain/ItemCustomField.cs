using System.ComponentModel.DataAnnotations;

namespace CollectionManagement.Models.Domain
{
    public class ItemCustomField
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

        public Collection Collection { get; set; }
    }
}
