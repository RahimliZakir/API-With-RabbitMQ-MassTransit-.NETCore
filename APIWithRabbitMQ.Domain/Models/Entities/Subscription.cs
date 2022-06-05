using APIWithRabbitMQ.Domain.Models.Entities.Membership;
using System;
using System.Collections.Generic;

namespace APIWithRabbitMQ.Domain.Models.Entities
{
    public class Subscription
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public bool IsConfirmed { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? DeletedByUserId { get; set; }
        public DateTime? DeletedDate { get; set; }
        public virtual AppUser? DeletedByUser { get; set; }
    }
}
