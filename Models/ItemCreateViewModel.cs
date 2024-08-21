using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CollectionManagement.Models.ViewModels
{
    public class ItemCreateViewModel
    {
        [Required]
        public string Name { get; set; }

        //public string Tags { get; set; }

        [Display(Name = "Tags")]
        public List<string> Tags { get; set; } = new List<string>();

        [Required]
        public int CollectionId { get; set; }

        public IDictionary<int, string> CustomFieldValues { get; set; } = new Dictionary<int, string>();
    }
}
