namespace EmployeeManagement.Api.Dtos
{
    public class EmployeeImportRequest
    {
        public IFormFile? File { get; set; }
        public string? RawData { get; set; }

        public (bool IsValid, string? ErrorMessage) Validate()
        {
            if (File == null && string.IsNullOrWhiteSpace(RawData))
                return (false, "File or raw data is required.");

            if (File != null)
            {
                if (File.Length == 0)
                    return (false, "Uploaded file is empty.");

                var ext = Path.GetExtension(File.FileName).ToLower();
                if (ext != ".csv" && ext != ".json")
                    return (false, "Only CSV or JSON files are allowed.");
            }

            return (true, null);
        }
    }
}
