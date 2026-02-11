using EmployeeManagement.Api.Commands.ImportEmployees;
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Dtos;
using EmployeeManagement.Api.Infrastructure;
using EmployeeManagement.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Api.Tests
{
    public class ImportEmployeeTests
    {
        private readonly Mock<IEmployeeCsvReader> _csvReaderMock;

        public ImportEmployeeTests()
        {
            _csvReaderMock = new Mock<IEmployeeCsvReader>();
        }

        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task Handle_ValidRawJsonData_SavesToDatabase()
        {
            // Arrange(준비)
            using var context = GetDbContext();
            var handler = new ImportEmployeeCommandHandler(context, _csvReaderMock.Object);

            var rawData = "[{\"Name\": \"홍길동\", \"Email\": \"hong@test.com\", \"Tel\": \"010-1234-5678\", \"Joined\": \"hong@test.com\"}]";
            var command = new ImportEmployeeCommand(null, rawData);

            // MapToEntities가 호출될 때 가짜 엔티티 반환하도록 설정
            _csvReaderMock.Setup(x => x.MapToEntities(It.IsAny<IEnumerable<EmployeeImportDto>>()))
                .Returns(new List<Employee> { new Employee { Name = "홍길동", Email = "hong@test.com", Tel = "010-1234-5678", Joined = "hong@test.com" } });

            // Act(실행)
            await handler.Handle(command);

            // Assert(검증)
            Assert.Equal(1, context.Employees.Count());
            Assert.Equal("홍길동", context.Employees.First().Name);
        }

        [Fact]
        public async Task Handle_CsvFileWithoutHeader_ImportsMultipleEmployees()
        {
            // Arrange(준비)
            using var context = GetDbContext();
            var handler = new ImportEmployeeCommandHandler(context, _csvReaderMock.Object);

            // .csv 파일 설정
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("employees.csv");

            // 데이터 스트림 생성
            var csvContent = "홍길동,hong@test.com,010-1234-5678,2023-01-01\n" +
                             "김길동,kim@test.com,010-5678-1234,2023-01-02";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);

            // Mock 설정: CSV 리더가 두 개의 DTO를 반환하도록 설정
            var importDtos = new List<EmployeeImportDto>
            {
                new EmployeeImportDto { Name = "홍길동", Email = "hong@test.com", Tel = "010-1234-5678", Joined = "2023-01-01" },
                new EmployeeImportDto { Name = "김길동", Email = "kim@test.com", Tel = "010-5678-1234", Joined = "2023-01-02" }
            };
            _csvReaderMock.Setup(x => x.Read(It.IsAny<Stream>())).Returns(importDtos);

            // Mock 설정: DTO 리스트를 엔티티 리스트로 변환
            _csvReaderMock.Setup(x => x.MapToEntities(importDtos))
                .Returns(new List<Employee>
                {
                    new Employee { Name = "홍길동", Email = "hong@test.com", Tel = "010-1234-5678", Joined = "2023-01-01" },
                    new Employee { Name = "김길동", Email = "kim@test.com", Tel = "010-5678-1234", Joined = "2023-01-02" }
                });

            var command = new ImportEmployeeCommand(fileMock.Object, null);

            // Act(실행)
            await handler.Handle(command);

            // Assert(검증)
            // 데이터베이스에 총 2명이 저장되었는지 확인
            Assert.Equal(2, context.Employees.Count());

            // 특정 데이터가 정확히 들어갔는지 확인
            var savedEmployees = await context.Employees.ToListAsync();
            Assert.Contains(savedEmployees, e => e.Name == "홍길동");
            Assert.Contains(savedEmployees, e => e.Name == "김길동");
        }
    }
}
