using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace APIWithRabbitMQ.Domain.Models.ViewModels
{
    public class PagedViewModel<T>
          where T : class
    {
        const int maxPaginationButtonCount = 10;

        public IEnumerable<T> Items { get; set; }
        public int CurrentIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int MaxPageIndex
        {
            get
            {
                return (int)Math.Ceiling(TotalCount * 1.0 / PageSize);
            }
        }

        public PagedViewModel(IQueryable<T> query, int pageIndex, int pageSize)
        {
            this.PageSize = pageSize;

            query = query.Where(CheckParentId).AsQueryable();

            this.TotalCount = query.Count();

            if (pageIndex > MaxPageIndex && pageIndex > 1)
                pageIndex = MaxPageIndex;

            this.Items = query.Skip((pageIndex > 0 ? pageIndex - 1 : 0) * pageSize)
                .Take(pageSize)
                .ToList();

            this.CurrentIndex = pageIndex;
        }

        private bool CheckParentId(T arg)
        {
            var prop = typeof(T).GetProperty("ParentId");

            if (prop != null)
            {
                return prop.GetValue(arg) == null;
            }

            return true;
        }

        public IHtmlContent GetPagenation(IUrlHelper urlHelper, string action, string area = "", string paginateFunction = "")
        {
            if (PageSize >= TotalCount)
                return HtmlString.Empty;


            StringBuilder builder = new();
            bool hasPaginationFunction = !string.IsNullOrWhiteSpace(paginateFunction);

            builder.Append("<ul class='pagination'>");

            if (CurrentIndex > 1)
            {
                var link = hasPaginationFunction
                    ? $"javascript:{paginateFunction}({CurrentIndex - 1},{PageSize})"
                    : urlHelper.Action(action, values: new
                    {
                        pageindex = CurrentIndex - 1,
                        pagesize = PageSize,
                        area
                    });

                builder.Append($@"<li class='prev'>
                                <a href='{link}'><i class='fas fa-chevron-left'></i></a>
                                </li>");
            }
            else
            {
                builder.Append("<li class='prev disabled'><a><i class='fas fa-chevron-left'></i></a></li>");
            }

            int min = 1, max = MaxPageIndex;

            if (CurrentIndex > (int)Math.Floor(maxPaginationButtonCount / 2D))
            {
                min = CurrentIndex - (int)Math.Floor(maxPaginationButtonCount / 2D);
            }

            max = min + maxPaginationButtonCount - 1;

            if (max > MaxPageIndex)
            {
                max = MaxPageIndex;
                min = max - maxPaginationButtonCount + 1;
            }

            for (int i = (min < 1 ? 1 : min); i <= max; i++)
            {
                if (i == CurrentIndex)
                {
                    builder.Append($"<li class='active'><a>{i}</a></li>");
                    continue;
                }

                var link = hasPaginationFunction
                    ? $"javascript:{paginateFunction}({i},{PageSize})"
                    : urlHelper.Action(action, values: new
                    {
                        pageindex = i,
                        pagesize = PageSize,
                        area
                    });

                builder.Append($"<li><a href='{link}'>{i}</a></li>");

            }


            if (CurrentIndex < MaxPageIndex)
            {
                var link = hasPaginationFunction
                    ? $"javascript:{paginateFunction}({CurrentIndex + 1},{PageSize})"
                    : urlHelper.Action(action, values: new
                    {
                        pageindex = CurrentIndex + 1,
                        pagesize = PageSize,
                        area
                    });

                builder.Append($@"<li class='next'>
                                <a href='{link}'><i class='fas fa-chevron-right'></i></a>
                                </li>");
            }
            else
            {
                builder.Append("<li class='next disabled'><a><i class='fas fa-chevron-right'></i></a></li>");
            }


            builder.Append("</ul>");

            return new HtmlString(builder.ToString());
        }
    }
}
