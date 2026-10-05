using System.Text;
using API.Dto;
using API.Util;
using Domain.Entities;
using Domain.Exceptions;
using Task = System.Threading.Tasks.Task;

namespace API.Features.Users;


public interface IUserService
{
    Task DeleteUserAsync(long userId);
    Task<UserDto> FindUserByIdWithData(long userId);
    Task UpdateUserInformationAsync(long userId, UpdateUserRequest updateUserRequest);
    Task ImportStudentsUsersCsvAsync(CsvFile csvFile);
    Task<UsersDtoList> PagingSearchUsersAsync(int pageSize, int pageNumber, string? text, int? roleId);
}

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task DeleteUserAsync(long userId)
    {
        await userRepository.DeleteUser(userId);
    }

    public async Task<UserDto> FindUserByIdWithData(long userId)
    {
        var user = await userRepository.FindUserByIdWithData(userId);
        if (user == null)
            throw new NotFoundException("Не найден пользователь", userId);
        return user;
    }

    public async Task UpdateUserInformationAsync(long userId, UpdateUserRequest userData)
    {
        // todo
        var userEntity = userData.UserInformation.MapToEntity();
        await userRepository.UpdateUserInformationAsync(userId, userEntity);
    }


    public bool CheckUsersCsvTitles(string titlesString)
    {
        var titles = titlesString.SplitCleanSpace(",");
        return titles[0].EqualsCi("email")
               && titles[1].EqualsCi("пароль")
               && titles[2].EqualsCi("фамилия")
               && titles[3].EqualsCi("имя")
               && titles[4].EqualsCi("отчество");
    }

    public async Task ImportStudentsUsersCsvAsync(CsvFile csvFile)
    {
        var file = csvFile.FormFile;
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        var text = Encoding.UTF8.GetString(stream.ToArray()).Trim();
        var lines = text.Split("\n");
        List<User> usersList = new List<User>();
        
        if (!CheckUsersCsvTitles(lines[0]))
        {
            throw new ArgumentException(
                "Неправильный формат заголовков файла. Они должны быть следующими (через запятую): "
                + "email, пароль, фамилия, имя, отчество");
        }

        for (int i = 1; i < lines.Length; i++)
        {
            var elements = lines[i].SplitCleanSpace(",");
            if (elements.Length < 3 || elements.Length > 5)
            {
                throw new ArgumentException($"Неправильный формат содержания файла. " +
                                            $"На каждой строчке через запятую должны быть указаны корректные данные, " +
                                            $"не менее 4 значений (отчество необязательно)");
            }

            var email = elements[0];
            var password = elements[1];
            var lastName = elements[2];
            var firstName = elements[3];
            string? middleName = elements.Length == 5 ? elements[4] : null;

            if (await userRepository.DoesExistUserWithEmail(email))
            {
                throw new ArgumentException($"Почта {email} занята, выберите другую почту");
            }

            var salt = Hashes.GetNewSalt();
            var newUser = new User()
            {
                RoleId = 3,
                Email = email,
                Salt = salt,
                Password = Hashes.GetPbkdf2Hash(password, salt),
                UserInformation = new UserInformation()
                {
                    LastName = lastName,
                    FirstName = firstName,
                    MiddleName = middleName
                }
            };
            usersList.Add(newUser);
        }
        await userRepository.AddUsers(usersList);
    }

    public async Task<UsersDtoList> PagingSearchUsersAsync(int pageSize, int pageNumber, string? text, int? roleId)
    {
        return await userRepository.PagingSearchUsers(pageSize, pageNumber, text, roleId);
    }
}