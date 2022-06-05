using APIWithRabbitMQ.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIWithRabbitMQ.Domain.Models.FormModels
{
    public class SubscribeFormModel
    {
        public Subscription? Subscription { get; set; }

        public News? News { get; set; }
    }
}
