namespace OneIncChallenge.Models.User;

public class UserPutRequestDTO
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? PhoneNumber { get; set; }
}