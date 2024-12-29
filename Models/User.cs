namespace UserManagementSystem.Models
{
    public class User
    {
        required public int Id { get; set; }
        required public string Name { get; set; }
        required public string Email { get; set; }
    }
}