using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Models;
using EmployeeManagement.Api.Queries.GetEmployees;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Api.Tests
{
    public class EmployeePagingTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Theory] // 하나의 테스트 메서드에 여러 파라미터를 넣어 테스트할 때 사용합니다.
        [InlineData(1, 2, 2)] // 1페이지, 사이즈 2 -> 데이터 2개 반환 예상
        [InlineData(2, 2, 1)] // 2페이지, 사이즈 2 -> 남은 데이터 1개 반환 예상
        public async Task Handle_PagingRequest_ReturnsCorrectPageData(int page, int pageSize, int expectedCount)
        {
            // Arrange(준비)
            using var context = GetDbContext();
            // 테스트용 데이터 3건 저장
            context.Employees.AddRange(new List<Employee>
            {
                new Employee { Name = "직원1", Email = "1@test.com" },
                new Employee { Name = "직원2", Email = "2@test.com" },
                new Employee { Name = "직원3", Email = "3@test.com" }
            });
            await context.SaveChangesAsync();

            var handler = new EmployeePagingQueryHandler(context);
            var query = new EmployeePagingQuery(page, pageSize);

            // Act(실행)
            var (data, totalCount) = await handler.Handle(query);

            // Assert(검증)
            totalCount.Should().Be(3); // 전체 개수는 페이지와 상관없이 항상 3이어야 함
            data.Should().HaveCount(expectedCount); // 현재 페이지의 데이터 개수 확인

            if (page == 1)
            {
                data.First().Name.Should().Be("직원1"); // 첫 페이지 첫 데이터 확인
            }
            else if (page == 2)
            {
                data.First().Name.Should().Be("직원3"); // 두 번째 페이지의 첫 데이터(Skip 2개 이후) 확인
            }
        }

        [Fact]
        public async Task Handle_EmptyDatabase_ReturnsZeroCount()
        {
            // Arrange(준비)
            using var context = GetDbContext(); // 빈 DB
            var handler = new EmployeePagingQueryHandler(context);
            var query = new EmployeePagingQuery(1, 10);

            // Act(실행)
            var (data, totalCount) = await handler.Handle(query);

            // Assert(검증)
            totalCount.Should().Be(0);
            data.Should().BeEmpty();
        }
    }
}
