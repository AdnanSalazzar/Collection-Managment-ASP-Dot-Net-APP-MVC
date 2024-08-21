using System.ComponentModel.DataAnnotations;

namespace CollectionManagement.Models.Domain
{
    public class ItemCustomFieldValue
    {
        [Key]
        public int ValueId { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        public int CustomFieldId { get; set; }

        public string FieldValue { get; set; }

        public Item Item { get; set; }
        public ItemCustomField CustomField { get; set; }
    }
}
