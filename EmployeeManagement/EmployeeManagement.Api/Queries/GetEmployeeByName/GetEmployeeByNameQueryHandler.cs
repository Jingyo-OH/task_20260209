using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Queries.GetEmployeeByName
{
    public class GetEmployeeByNameQueryHandler(
        AppDbContext _context
        )
    {
        public async Task<EmployeeDto?> Handler(GetEmployeeByNameQuery query)
        {
            return await _context.Employees
                                 .AsNoTracking()
                                 .Where(e => e.Name == query.Name)
                                 .Select(e => new EmployeeDto
                                 {
                                     Name = e.Name,
                                     Email = e.Email,
                                     Tel = e.Tel,
                                     Joined = e.Joined,
                                 })
                                 .FirstOrDefaultAsync();
        }
    }
}
