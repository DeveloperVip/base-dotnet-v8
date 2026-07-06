namespace growth_planning_be.Models.Auth;

public class LoginResponse
{
    public string Token { get; set; }
    public List<string?> Roles { get; set; } = new List<string>();
}
