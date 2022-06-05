using Microsoft.AspNetCore.Identity;

namespace APIWithRabbitMQ.Domain.Models.Entities.Membership
{
    public class AppRole : IdentityRole<int>
    {
        public virtual ICollection<AppUserRole> UserRoles { get; set; }
    }
}
