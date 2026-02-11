using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Models;
using EmployeeManagement.Api.Queries.GetEmployeeByName;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Api.Tests
{
    public class GetEmployeeByNameTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task Handle_ExistingName_ReturnsEmployeeDto()
        {
            // Arrange(준비)
            using var context = GetDbContext();
            context.Employees.Add(new Employee { Name = "김철수", Email = "charles@clovf.com", Tel = "010-7531-2468", Joined = "2012-01-05" });
            await context.SaveChangesAsync();

            var handler = new GetEmployeeByNameQueryHandler(context);
            var query = new GetEmployeeByNameQuery("김철수");

            // Act(실행)
            var result = await handler.Handler(query);

            // Assert(검증)
            Assert.NotNull(result);
            Assert.Equal("김철수", result.Name);
        }

        [Fact]
        public async Task Handle_NonExistingName_ReturnsNull()
        {
            // Arrange(준비)
            using var context = GetDbContext();
            var handler = new GetEmployeeByNameQueryHandler(context);
            var query = new GetEmployeeByNameQuery("Unknown");

            // Act(실행)
            var result = await handler.Handler(query);

            // Assert(검증)
            Assert.Null(result);
        }
    }
}
