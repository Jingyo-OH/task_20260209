namespace EmployeeManagement.Api.Commands.ImportEmployees
{
    public class ImportEmployeeCommand
    {
        public IFormFile? File { get; set; }
        public string? RawData { get; set; }

        public ImportEmployeeCommand(IFormFile? file = null, string? rawData = null)
        {
            File = file;
            RawData = rawData;
        }
    }
}
