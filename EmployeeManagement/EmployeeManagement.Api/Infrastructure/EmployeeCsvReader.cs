using CsvHelper;
using CsvHelper.Configuration;
using EmployeeManagement.Api.Dtos;
using EmployeeManagement.Api.Models;
using System.Formats.Asn1;
using System.Globalization;
using System.Text;

namespace EmployeeManagement.Api.Infrastructure
{
    public interface IEmployeeCsvReader
    {
        List<EmployeeImportDto> Read(Stream stream);
        IEnumerable<Employee> MapToEntities(IEnumerable<EmployeeImportDto> dtos);
    }
    public class EmployeeCsvReader : IEmployeeCsvReader
    {
        public List<EmployeeImportDto> Read(Stream stream)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding encoding = Encoding.GetEncoding(949); // Excel CSV 기본 인코딩
            using var reader = new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks: true);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                MissingFieldFound = null,
                HeaderValidated = null,
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true,
                Delimiter = ","
            };

            using var csv = new CsvReader(reader, config);
            csv.Context.RegisterClassMap<EmployeeImportCsvMap>();

            return csv.GetRecords<EmployeeImportDto>().ToList();
        }

        public IEnumerable<Employee> MapToEntities(IEnumerable<EmployeeImportDto> dtos)
        {
            return dtos.Select(r => new Employee
            {
                Name = r.Name,
                Email = r.Email,
                Tel = r.Tel,
                Joined = r.Joined
            });
        }
    }
}
