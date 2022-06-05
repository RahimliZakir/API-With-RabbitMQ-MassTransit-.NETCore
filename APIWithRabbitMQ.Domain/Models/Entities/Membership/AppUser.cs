using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace APIWithRabbitMQ.Domain.Models.Entities.Membership
{
    public class AppUser : IdentityUser<int>
    {
        public string? Name { get; set; } 

        public string? Surname { get; set; } 

        public virtual ICollection<AppUserRole> UserRoles { get; set; }
    }
}
