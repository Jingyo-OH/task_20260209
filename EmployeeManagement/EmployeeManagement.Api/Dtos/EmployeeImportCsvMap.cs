using CsvHelper.Configuration;

namespace EmployeeManagement.Api.Dtos
{
    public class EmployeeImportCsvMap : ClassMap<EmployeeImportDto>
    {
        public EmployeeImportCsvMap()
        {
            Map(m => m.Name).Index(0);
            Map(m => m.Email).Index(1);
            Map(m => m.Tel).Index(2);
            Map(m => m.Joined).Index(3);
        }
    }
}
