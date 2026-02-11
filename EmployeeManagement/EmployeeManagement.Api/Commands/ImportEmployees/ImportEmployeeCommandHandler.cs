using CsvHelper;
using CsvHelper.Configuration;
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Dtos;
using EmployeeManagement.Api.Infrastructure;
using EmployeeManagement.Api.Models;
using System.Globalization;
using System.Text.Json;

namespace EmployeeManagement.Api.Commands.ImportEmployees
{
    public class ImportEmployeeCommandHandler(
        AppDbContext _context,
        IEmployeeCsvReader _csvReader
        )
    {
        public async Task Handle(ImportEmployeeCommand command)
        {
            var employees = new List<Employee>();

            #region [파일 업로드 처리]
            if (command.File != null)
            {
                var ext = Path.GetExtension(command.File.FileName).ToLower();
                using var stream = command.File.OpenReadStream();

                if (ext == ".csv")
                {
                    var importDtos = _csvReader.Read(stream);
                    employees.AddRange(_csvReader.MapToEntities(importDtos));
                }
                else if (ext == ".json")
                {
                    var importDtos = await TryDeserializeJsonFile(stream);
                    if (importDtos != null)
                        employees.AddRange(_csvReader.MapToEntities(importDtos));
                }
            }
            #endregion

            #region [RawData 처리]
            if (!string.IsNullOrWhiteSpace(command.RawData))
            {
                List<EmployeeImportDto>? importDtos = null;

                // 1. JSON 배열
                importDtos = TryDeserializeJson<List<EmployeeImportDto>>(command.RawData);

                // 2. JSON 단일 객체
                if (importDtos == null)
                {
                    var singleDto = TryDeserializeJson<EmployeeImportDto>(command.RawData);
                    if (singleDto != null)
                        importDtos = new List<EmployeeImportDto> { singleDto };
                }

                // 3. CSV
                if (importDtos == null)
                {
                    using var reader = new StringReader(command.RawData);
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = false,
                        TrimOptions = TrimOptions.Trim,
                        IgnoreBlankLines = true,
                        Delimiter = ","
                    };
                    using var csv = new CsvReader(reader, config);
                    csv.Context.RegisterClassMap<EmployeeImportCsvMap>();

                    importDtos = csv.GetRecords<EmployeeImportDto>().ToList();
                }

                if (importDtos != null)
                    employees.AddRange(_csvReader.MapToEntities(importDtos));
            }
            #endregion

            if (employees.Any())
            {
                await _context.Employees.AddRangeAsync(employees);
                await _context.SaveChangesAsync();
            }
        }

        #region [헬퍼 메서드]
        private T? TryDeserializeJson<T>(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
            catch
            {
                return default;
            }
        }

        private async Task<List<EmployeeImportDto>?> TryDeserializeJsonFile(Stream stream)
        {
            stream.Position = 0;
            try
            {
                // 배열 시도
                return await JsonSerializer.DeserializeAsync<List<EmployeeImportDto>>(
                    stream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
            catch
            {
                try
                {
                    stream.Position = 0; // 다시 처음부터 읽기
                    var single = await JsonSerializer.DeserializeAsync<EmployeeImportDto>(
                        stream,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                    if (single != null) return new List<EmployeeImportDto> { single };
                }
                catch { }
            }

            return null;
        }
        #endregion
    }
}
