namespace APIWithRabbitMQ.WebAPI.AppCode.Infrastructure
{
    public class CommandJsonResponse
    {
        public string Message { get; set; }

        public string Temp { get; set; }

        public string ReturnUrl { get; set; }

        public bool Error { get; set; }
    }
}
