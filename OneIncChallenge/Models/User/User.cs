using System.ComponentModel.DataAnnotations;

namespace OneIncChallenge.Models.User;

public class User
{
    public Guid Id { get; set; }

    [Required]
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    [Required]
    public string? Email { get; set; }
    [Required]
    public DateTime DateOfBirth { get; set; }
    [Required]
    public long PhoneNumber { get; set; }
}