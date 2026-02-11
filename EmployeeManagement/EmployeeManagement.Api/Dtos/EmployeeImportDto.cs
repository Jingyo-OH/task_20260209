using System.Text.Json.Serialization;

namespace EmployeeManagement.Api.Dtos
{
    public class EmployeeImportDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        [JsonPropertyName("tel")]
        public string Tel { get; set; } = string.Empty;
        [JsonPropertyName("joined")]
        public string Joined { get; set; } = string.Empty;
    }
}
