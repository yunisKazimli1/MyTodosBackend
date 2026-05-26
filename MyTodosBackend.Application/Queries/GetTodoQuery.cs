using MyTodosBackend.Domain.Enums;

namespace MyTodosBackend.Application.Queries
{
    public class GetTodosQuery
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public TodoFilterEnum FilterBy { get; set; }
        public TodoSortingEnum SortBy { get; set; }
    }
}
