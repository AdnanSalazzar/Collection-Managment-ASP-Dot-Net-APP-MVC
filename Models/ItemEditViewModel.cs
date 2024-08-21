using CollectionManagement.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace CollectionManagement.Models
{
    public class ItemEditViewModel
    {
        public int ItemId { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        public string Tags { get; set; }

        public int CollectionId { get; set; }

        public DateTime UpdatedAt { get; set; }

        public IDictionary<int, string> CustomFieldValues { get; set; }

        public ICollection<ItemCustomField> CustomField { get; set; }
    }
}
