using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Queries.GetEmployees
{
    public class EmployeePagingQueryHandler(
        AppDbContext _context
        )
    {
        public async Task<(IEnumerable<EmployeeDto> Data, int TotalCount)> Handle(EmployeePagingQuery query)
        {
            var totalCount = await _context.Employees.CountAsync();

            var data = await _context.Employees
                                     .AsNoTracking()
                                     .Skip((query.Page - 1) * query.PageSize)
                                     .Take(query.PageSize)
                                     .Select(e => new EmployeeDto
                                     {
                                         Name = e.Name,
                                         Email = e.Email,
                                         Tel = e.Tel,
                                         Joined = e.Joined
                                     })
                                    .ToListAsync();

            return (data, totalCount);
        }
    }
}
