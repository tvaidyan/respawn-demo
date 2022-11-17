namespace RespawnDemo.Api.Employee;

public class Employee
{
    public DateTime HireDate { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid EmployeeId { get; set; } = Guid.NewGuid();
    public string FavoriteColor { get; set; } = string.Empty;
}
