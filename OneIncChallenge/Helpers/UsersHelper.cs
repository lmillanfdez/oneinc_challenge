using OneIncChallenge.Models.User;
using System.Text.RegularExpressions;

namespace OneIncChallenge.Helpers;

public class UsersHelper
{
    public static bool IsValidUser(UserPutRequestDTO user, out string errorMessage)
    {
        errorMessage = string.Empty;

        if(!string.IsNullOrEmpty(user.FirstName) && string.IsNullOrWhiteSpace(user.FirstName))
        {
            errorMessage = "Firstname must have characters other than whitespaces.";
        }

        if(!string.IsNullOrEmpty(user.LastName) && string.IsNullOrWhiteSpace(user.LastName))
        {
            errorMessage = "LastName must have characters other than whitespaces.";
        }

        var validEmailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        if(!string.IsNullOrEmpty(user.Email) && !Regex.IsMatch(user.Email, validEmailRegex))
        {
            errorMessage = "Invalid email.";
        }

        var eigtheenYearsAgo = DateTime.Now.AddYears(-18);

        if(user.DateOfBirth.HasValue && DateTime.Compare(user.DateOfBirth ?? DateTime.Now, eigtheenYearsAgo) > 0)
        {
            errorMessage = "Invalid date of birth.";
        }

        if(user.PhoneNumber.HasValue && user.PhoneNumber < Math.Pow(10, 9) || user.PhoneNumber >= Math.Pow(10, 10))
        {
            errorMessage = "Invalid phone number.";
        }

        return string.IsNullOrEmpty(errorMessage);
    }

    public static bool IsValidUser(User user, out string errorMessage)
    {
        errorMessage = string.Empty;
        var eigtheenYearsAgo = DateTime.Now.AddYears(-18);

        if(DateTime.Compare(user.DateOfBirth, eigtheenYearsAgo) > 0)
        {
            errorMessage = "User's age is under 18.";
        }

        var centuryAndAHalfAgo = DateTime.Now.AddYears(-150);

        if(DateTime.Compare(user.DateOfBirth, centuryAndAHalfAgo) < 0)
        {
            errorMessage = "User is too old to be alive.";
        }

        var validEmailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        if(string.IsNullOrEmpty(user.Email) || !Regex.IsMatch(user.Email, validEmailRegex))
        {
            errorMessage = "Invalid email.";
        }

        if(user.PhoneNumber < Math.Pow(10, 9) || user.PhoneNumber >= Math.Pow(10, 10))
        {
            errorMessage = "Invalid phone number.";
        }

        return string.IsNullOrEmpty(errorMessage);
    }
}