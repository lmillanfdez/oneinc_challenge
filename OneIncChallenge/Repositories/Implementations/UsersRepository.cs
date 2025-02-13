using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using OneIncChallenge.Configurations;
using OneIncChallenge.Exceptions;
using OneIncChallenge.Helpers;
using OneIncChallenge.Models.User;
using OneIncChallenge.Repositories.Contracts;
using System.Data;

namespace OneIncChallenge.Repositories.Implementations; 

public class UsersRepository : IUsersRepository
{
    private readonly string connString;

    public UsersRepository(IOptions<ConnectionsStrings> connStrings)
    {
        connString = connStrings.Value.OneIncChallengeDB;
    }

    public async Task<IEnumerable<UserDetailsDTO>> GetDetailedUsers()
    {
        var results = new List<UserDetailsDTO>();

        using(var conn = new SqlConnection(connString))
        {
            await conn.OpenAsync();
            
            var sqlQuery = "SELECT * FROM [Challenge].[Users]";
            var sqlCommand = new SqlCommand(sqlQuery, conn);
            var sqlDataReader = await sqlCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection);

            while(await sqlDataReader.ReadAsync())
            {
                var tempResult = new UserDetailsDTO
                {
                    Id = Guid.Parse(sqlDataReader[0].ToString() ?? throw new Exception("Guid is null")),
                    FirstName = sqlDataReader[1].ToString(),
                    LastName = sqlDataReader[2].ToString() ?? string.Empty,
                    Email = sqlDataReader[3].ToString(),
                    DateOfBirth = DateTime.Parse(sqlDataReader[4].ToString() ?? throw new Exception("Date of birth is null")),
                    PhoneNumber = Convert.ToInt64(sqlDataReader[5]),
                };

                tempResult.Age = ComputeAge(tempResult.DateOfBirth);
                tempResult.DateOfBirthAsString = tempResult.DateOfBirth.ToString("MMM dd, yyyy");

                results.Add(tempResult);
            }
        }

        return results;
    }

    public async Task<UserDetailsDTO?> GetDetailedUser(Guid id)
    {
        UserDetailsDTO? result = null;

        using(var conn = new SqlConnection(connString))
        {
            await conn.OpenAsync();

            var sqlQuery = "SELECT * FROM [Challenge].[Users] WHERE [Id] = @Id";
            var sqlCommand = new SqlCommand(sqlQuery, conn);

            sqlCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier){ Value = id });

            var sqlDataReader = await sqlCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection);

            while(await sqlDataReader.ReadAsync())
            {
                result = new UserDetailsDTO
                {
                    Id = Guid.Parse(sqlDataReader[0].ToString() ?? throw new Exception("Guid is null")),
                    FirstName = sqlDataReader[1].ToString(),
                    LastName = sqlDataReader[2].ToString() ?? string.Empty,
                    Email = sqlDataReader[3].ToString(),
                    DateOfBirth = DateTime.Parse(sqlDataReader[4].ToString() ?? throw new Exception("Date of birth is null")),
                    PhoneNumber = Convert.ToInt64(sqlDataReader[5])
                };

                result.Age = ComputeAge(result.DateOfBirth);
                result.DateOfBirthAsString = result.DateOfBirth.ToString("MMM dd, yyyy");
            }
        }

        return result;
    }

    public async Task<User> AddUser(User user)
    {
        using(var conn = new SqlConnection(connString))
        {
            await conn.OpenAsync();

            var sqlQuery = "INSERT INTO [Challenge].[Users]([Id], [FirstName], [LastName], [Email], [PhoneNumber], [DateOfBirth])";
            sqlQuery = string.Concat(sqlQuery, $" VALUES (@Id, @FirstName, @LastName, @Email, @PhoneNumber, @DateOfBirth)");

            var uniqueIdentifier = Guid.NewGuid();

            var sqlCommand = new SqlCommand(sqlQuery, conn);

            sqlCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier){ Value = uniqueIdentifier });
            sqlCommand.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.NVarChar, 128){ Value = user.FirstName });
            sqlCommand.Parameters.Add(new SqlParameter("@LastName", SqlDbType.NVarChar, 128){ Value = user.LastName });
            sqlCommand.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar){ Value = user.Email });
            sqlCommand.Parameters.Add(new SqlParameter("@PhoneNumber", SqlDbType.BigInt){ Value = user.PhoneNumber });
            sqlCommand.Parameters.Add(new SqlParameter("@DateOfBirth", SqlDbType.DateTime){ Value = user.DateOfBirth });

            await sqlCommand.ExecuteNonQueryAsync();

            user.Id = uniqueIdentifier;

            return user;
        }
    }

    public async Task<bool> UpdateUserOrCreateIfNotExist(Guid userGuid, UserPutRequestDTO user)
    {
        var sqlParameters = new List<SqlParameter>();
        var sqlFields = new List<string>();

        using(var conn = new SqlConnection(connString))
        {
            await conn.OpenAsync();

            var sqlQuery = "UPDATE [Challenge].[Users] SET ";

            if(!string.IsNullOrEmpty(user.FirstName))
            {
                sqlFields.Add("[FirstName]=@FirstName");
                sqlParameters.Add(new SqlParameter("@FirstName", SqlDbType.NVarChar, 128){ Value = user.FirstName });
            }

            if(!string.IsNullOrEmpty(user.LastName))
            {
                sqlFields.Add("[LastName]=@LastName");
                sqlParameters.Add(new SqlParameter("@LastName", SqlDbType.NVarChar, 128){ Value = user.LastName });
            }

            if(!string.IsNullOrEmpty(user.Email))
            {
                sqlFields.Add("[Email]=@Email");
                sqlParameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar){ Value = user.Email });
            }

            if(user.DateOfBirth.HasValue)
            {
                sqlFields.Add("[DateOfBirth]=@DateOfBirth");
                sqlParameters.Add(new SqlParameter("@DateOfBirth", SqlDbType.DateTime){ Value = user.DateOfBirth });
            }

            if(user.PhoneNumber.HasValue)
            {
                sqlFields.Add("[PhoneNumber]=@PhoneNumber");
                sqlParameters.Add(new SqlParameter("@PhoneNumber", SqlDbType.BigInt){ Value = user.PhoneNumber });
            }

            sqlQuery = string.Concat(sqlQuery, string.Join(',', sqlFields));

            sqlQuery = string.Concat(sqlQuery, " WHERE [Id]=@Id");
            sqlParameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier){ Value = userGuid });

            var sqlCommand = new SqlCommand(sqlQuery, conn);

            sqlCommand.Parameters.AddRange([..sqlParameters]);

            var amountRowsAffected = await sqlCommand.ExecuteNonQueryAsync();

            if(amountRowsAffected == 0)
            {
                var userToInsert = new User
                {
                    FirstName = string.IsNullOrEmpty(user.FirstName) || string.IsNullOrWhiteSpace(user.FirstName) 
                                    ? throw new InvalidUserException("Cannot insert user without a valid firstname.") : user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email ?? throw new InvalidUserException("Cannot insert user without email."),
                    DateOfBirth = user.DateOfBirth ?? throw new InvalidUserException("Cannot insert user without date of birth."),
                    PhoneNumber = user.PhoneNumber ?? throw new InvalidUserException("Cannot insert user without phone number."),
                };

                string errorMessage;

                if(!UsersHelper.IsValidUser(user, out errorMessage))
                {
                    throw new InvalidUserException(errorMessage);
                }

                var insertSqlQuery = "INSERT INTO [Challenge].[Users]([Id], [FirstName], [LastName], [Email], [PhoneNumber], [DateOfBirth])";
                insertSqlQuery = string.Concat(insertSqlQuery, $" VALUES (@Id, @FirstName, @LastName, @Email, @PhoneNumber, @DateOfBirth)");

                sqlCommand = new SqlCommand(insertSqlQuery, conn);

                sqlCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier){ Value = userGuid });
                sqlCommand.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.NVarChar, 128){ Value = user.FirstName });
                sqlCommand.Parameters.Add(new SqlParameter("@LastName", SqlDbType.NVarChar, 128){ Value = user.LastName });
                sqlCommand.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar){ Value = user.Email });
                sqlCommand.Parameters.Add(new SqlParameter("@PhoneNumber", SqlDbType.BigInt){ Value = user.PhoneNumber });
                sqlCommand.Parameters.Add(new SqlParameter("@DateOfBirth", SqlDbType.DateTime){ Value = user.DateOfBirth });

                await sqlCommand.ExecuteNonQueryAsync();

                return true;
            }

            return false;
        }
    }

    public async Task<bool> DeleteUser(Guid userGuid)
    {
        using(var conn = new SqlConnection(connString))
        {
            await conn.OpenAsync();

            var sqlQuery = "DELETE [Challenge].[Users] WHERE [Id]=@Id";

            var sqlCommand = new SqlCommand(sqlQuery, conn);

            sqlCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier){ Value = userGuid });

            var amountRowsAffected = await sqlCommand.ExecuteNonQueryAsync();

            return amountRowsAffected != 0;
        }
    }

    private int ComputeAge(DateTime dateOfBirth)
    {
        var currentDateTime = DateTime.Now;
        var age = currentDateTime.Year - dateOfBirth.Year;

        if(currentDateTime.Month < dateOfBirth.Month)
        {
            return age - 1;
        }

        if(currentDateTime.Month == dateOfBirth.Month && currentDateTime.Day < dateOfBirth.Day )
        {
            return age - 1;
        }

        return age;
    }
}