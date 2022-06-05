using APIWithRabbitMQ.Domain.Models.Entities;
using APIWithRabbitMQ.WebAPI.AppCode.Infrastructure;
using APIWithRabbitMQ.WebAPI.AppCode.Modules.NewsModule;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace APIWithRabbitMQ.WebAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class NewsController : Controller
    {
        readonly IMediator mediator;

        public NewsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        async public Task<IActionResult> GetNewsList()
        {
            IEnumerable<News> news = await mediator.Send(new NewsGetAllActiveQuery());

            return Ok(news);
        }

        [HttpGet("ById/{Id}")]
        async public Task<IActionResult> GetNews([FromRoute] NewsSingleQuery query)
        {
            News news = await mediator.Send(query);

            if (news == null)
            {
                return NotFound();
            }

            return Ok(news);
        }

        [HttpPost]
        async public Task<IActionResult> PostSubscription(NewsCreateCommand command)
        {
            CommandJsonResponse response = await mediator.Send(command);

            if (response.Error)
                return BadRequest();

            return Ok(response);
        }

        [HttpDelete("{Id}")]
        async public Task<IActionResult> DeleteSubscription([FromRoute] NewsRemoveCommand command)
        {
            CommandJsonResponse response = await mediator.Send(command);

            if (response.Error)
                return BadRequest();

            StatusCodeResult result = new(StatusCodes.Status204NoContent);

            return Ok(result);
        }
    }
}
