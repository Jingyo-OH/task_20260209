using EmployeeManagement.Api.Commands.ImportEmployees;
using EmployeeManagement.Api.Dtos;
using EmployeeManagement.Api.Queries.GetEmployeeByName;
using EmployeeManagement.Api.Queries.GetEmployees;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers
{
    /// <summary>
    /// 사원 관리 및 데이터 임포트를 담당하는 API 컨트롤러
    /// </summary>
    [Tags("사원 관리 API")]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEmployees([FromQuery] PagingRequest paging)
        {
            if (paging.Page <= 0)
            {
                return BadRequest(new { message = "Page must be greater than 0" });
            }

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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmployeeDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [ProducesResponseType(StatusCodes.Status201Created)]
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
