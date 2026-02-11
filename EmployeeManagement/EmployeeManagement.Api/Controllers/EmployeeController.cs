using EmployeeManagement.Api.Commands.ImportEmployees;
using EmployeeManagement.Api.Dtos;
using EmployeeManagement.Api.Queries.GetEmployeeByName;
using EmployeeManagement.Api.Queries.GetEmployees;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController (
        EmployeePagingQueryHandler _pagingHandler,
        GetEmployeeByNameQueryHandler _nameQueryHandler,
        ImportEmployeeCommandHandler _importHandler
        ) : ControllerBase
    {
        #region[Employee Paging]
        [HttpGet]
        public async Task<IActionResult> GetEmployees([FromQuery] PagingRequest paging)
        {
            var query = new EmployeePagingQuery(paging.Page, paging.PageSize);
            var (data, totalCount) = await _pagingHandler.Handle(query);

            var response = new
            {
                Page = paging.Page,
                PageSize = paging.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)paging.PageSize),
                Data = data
            };

            return Ok(response);
        }
        #endregion

        #region[Employee Detail By Name]
        [HttpGet("{name}")]
        public async Task<IActionResult> GetEmployeeByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { message = "name is required" });

            var query = new GetEmployeeByNameQuery(name);
            var employee = await _nameQueryHandler.Handler(query);

            if (employee == null)
                return NotFound(new { message = "Employee not found" });

            return Ok(employee);
        }
        #endregion

        #region[Employee Import]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Import([FromForm] EmployeeImportRequest request)
        {
            var (isValid, errorMessage) = request.Validate();
            if (!isValid)
                return BadRequest(new { message = errorMessage });

            var command = new ImportEmployeeCommand(request.File, request.RawData);
            await _importHandler.Handle(command);

            return StatusCode(201);
        }
        #endregion
    }
}
