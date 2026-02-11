namespace EmployeeManagement.Api.Queries.GetEmployees
{
    public class EmployeePagingQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public EmployeePagingQuery(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }
    }
}
