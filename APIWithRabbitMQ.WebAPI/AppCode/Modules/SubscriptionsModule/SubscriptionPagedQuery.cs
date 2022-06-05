using APIWithRabbitMQ.Domain.Models.DataContexts;
using APIWithRabbitMQ.Domain.Models.Entities;
using APIWithRabbitMQ.Domain.Models.ViewModels;
using MediatR;

namespace APIWithRabbitMQ.WebAPI.AppCode.Modules.SubscriptionsModule
{
    public class SubscriptionPagedQuery : IRequest<PagedViewModel<Subscription>>
    {
        int pageIndex, pageSize;

        #region PaginationSettings
        public int PageIndex
        {
            get
            {
                if (pageIndex > 0)
                    return pageIndex;

                return 1;
            }
            set
            {
                if (value > 0)
                    pageIndex = value;
                else
                {
                    pageIndex = 1;
                }
            }
        }

        public int PageSize
        {
            get
            {
                if (pageSize > 0)
                    return pageSize;

                return 10;
            }
            set
            {
                if (value > 0)
                    pageSize = value;
                else
                {
                    pageSize = 10;
                }
            }
        }

        public SubscriptionPagedQuery() { }

        public SubscriptionPagedQuery(int pageIndex = 1, int pageSize = 10)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
        }
        #endregion

        public PagedViewModel<Subscription> Response { get; set; }
        public string Email { get; set; }

        public class SubscriptionPagedQueryHandler : IRequestHandler<SubscriptionPagedQuery, PagedViewModel<Subscription>>
        {
            readonly RabbitDbContext db;

            public SubscriptionPagedQueryHandler(RabbitDbContext db)
            {
                this.db = db;
            }

            async public Task<PagedViewModel<Subscription>> Handle(SubscriptionPagedQuery request, CancellationToken cancellationToken)
            {
                IQueryable<Subscription> query = db.Subscriptions.Where(f => f.DeletedDate == null).AsQueryable();

                request.Email = request.Email?.Trim();
                if (!string.IsNullOrWhiteSpace(request.Email))
                    query = query.Where(q => q.Email.Contains(request.Email));

                query = query.OrderBy(q => q.Id);

                PagedViewModel<Subscription> viewModel = new(query, request.PageIndex, request.PageSize);

                return viewModel;
            }
        }
    }
}
