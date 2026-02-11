namespace EmployeeManagement.Api.Queries.GetEmployeeByName
{
    public class GetEmployeeByNameQuery
    {
        public string Name { get; set; } = string.Empty;

        public GetEmployeeByNameQuery(string name)
        {
            Name = name;
        }
    }
}
