using System.ComponentModel.DataAnnotations;

namespace CollectionManagement.Models.Domain
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public Item Item { get; set; }
        public User User { get; set; }
    }
}
