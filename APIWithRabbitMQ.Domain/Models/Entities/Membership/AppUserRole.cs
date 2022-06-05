using Microsoft.AspNetCore.Identity;

namespace APIWithRabbitMQ.Domain.Models.Entities.Membership
{
    public class AppUserRole : IdentityUserRole<int>
    {
        public virtual AppUser User { get; set; }
        public virtual AppRole Role { get; set; }
    }
}
