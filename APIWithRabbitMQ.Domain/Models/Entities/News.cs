using APIWithRabbitMQ.Domain.Models.Entities.Membership;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIWithRabbitMQ.Domain.Models.Entities
{
    public class News
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }
        public int CreatedByUserId { get; set; }
        public virtual AppUser CreatedByUser { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? DeletedByUserId { get; set; }
        public virtual AppUser? DeletedByUser { get; set; }
    }
}
