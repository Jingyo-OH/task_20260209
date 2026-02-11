using EmployeeManagement.Api.Controllers;
using EmployeeManagement.Api.Dtos;
using EmployeeManagement.Api.Queries.GetEmployees;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Api.Tests
{
    public class EmployeeControllerTests
    {
        [Fact]
        public async Task GetEmployeeByName_EmptyName_ReturnsBadRequest()
        {
            // Arrange(준비)
            var controller = new EmployeeController(null!, null!, null!);

            // Act(실행)
            var result = await controller.GetEmployeeByName("");

            // Assert(검증)
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("name is required", badRequest.Value!.ToString()!);
        }

        [Fact]
        public async Task GetEmployees_InvalidPage_ReturnsBadRequest()
        {
            // Arrange(준비)
            var controller = new EmployeeController(null!, null!, null!);
            var request = new PagingRequest { Page = 0, PageSize = 10 };

            // Act(실행)
            var result = await controller.GetEmployees(request);

            // Assert(검증)
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Page must be greater than 0", badRequest.Value!.ToString()!);
        }

        [Fact]
        public async Task Import_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange(준비)
            var controller = new EmployeeController(null!, null!, null!);
            var request = new EmployeeImportRequest { File = null };

            // Act(실행)
            var result = await controller.Import(request);

            // Assert(검증)
            // 로직 상의 request.Validate()가 false를 반환하여 BadRequest가 나오는지 확인
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
