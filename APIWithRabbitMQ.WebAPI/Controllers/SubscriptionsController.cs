using APIWithRabbitMQ.Domain.Models.Entities;
using APIWithRabbitMQ.WebAPI.AppCode.Infrastructure;
using APIWithRabbitMQ.WebAPI.AppCode.Modules.SubscriptionsModule;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIWithRabbitMQ.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        readonly IMediator mediator;

        public SubscriptionsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Authorize(Policy = "subscriptions.get.all")]
        async public Task<IActionResult> GetSubscriptions()
        {
            IEnumerable<Subscription> subscriptions = await mediator.Send(new SubscriptionGetAllActiveQuery());

            return Ok(subscriptions);
        }

        [HttpGet("ById/{Id}")]
        [Authorize(Policy = "subscriptions.get.single")]
        async public Task<IActionResult> GetSubscription([FromRoute] SubscriptionSingleQuery query)
        {
            SubscriptionViewModel subscription = await mediator.Send(query);

            if (subscription == null)
            {
                return NotFound();
            }

            return Ok(subscription);
        }

        [HttpGet("ByEmail/{Email}")]
        async public Task<IActionResult> GetSubscriptionByEmail([FromRoute] SubscriptionGetByEmailQuery query)
        {
            SubscriptionViewModel subscription = await mediator.Send(query);

            if (subscription == null)
            {
                return NotFound();
            }

            return Ok(subscription);
        }

        [HttpPost]
        async public Task<IActionResult> PostSubscription(SubscriptionCreateCommand command)
        {
            CommandJsonResponse response = await mediator.Send(command);

            if (response.Error)
                return BadRequest();

            return Ok(response);
        }

        [HttpGet("confirm-subscription")]
        [AllowAnonymous]
        async public Task<IActionResult> ConfirmSubscription([FromQuery] SubscriptionConfirmCommand command)
        {
            CommandJsonResponse response = await mediator.Send(command);

            if (response.Error)
                return BadRequest();

            return Ok(response);
        }

        [HttpDelete("{Id}")]
        async public Task<IActionResult> DeleteSubscription([FromRoute] SubscriptionRemoveCommand command)
        {
            CommandJsonResponse response = await mediator.Send(command);

            if (response.Error)
                return BadRequest();

            StatusCodeResult result = new(StatusCodes.Status204NoContent);

            return Ok(result);
        }
    }
}
