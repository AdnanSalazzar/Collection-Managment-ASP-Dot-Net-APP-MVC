using System.ComponentModel.DataAnnotations;

namespace CollectionManagement.Models
{
    public class CollectionCreateViewModel
    {
        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; }

        public IFormFile ImageUrl { get; set; }
    }
}
