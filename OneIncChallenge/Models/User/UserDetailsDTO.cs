namespace OneIncChallenge.Models.User;

public class UserDetailsDTO : User
{
    public int Age { get; set; }
    public string DateOfBirthAsString { get; set; } = string.Empty;
}