using Supabase;
using Supabase.Interfaces;

public class UserService
{
    private readonly Client _client;

    public UserService(SupabaseClientService clientService)
    {        
        _client = clientService.Client;
    }

    // Регистрация
    public async Task<users?> RegisterAsync(string email, string password)
    {
        try
        {
            // Проверка на существование пользователя
            var existingUser = await _client.From<users>()
                .Filter("email", Supabase.Postgrest.Constants.Operator.Equals, email)
                .Single();

            if (existingUser != null)
            {
                return null; // Пользователь уже существует
            }

            // Хэшируем пароль (используйте Identity или другой механизм)
            var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<users>();
            var hashedPassword = passwordHasher.HashPassword(null, password);

            // Добавляем пользователя
            var newUser = new users
            {
                email = email,
                password_hash = hashedPassword
            };

            var response = await _client.From<users>().Insert(newUser);

            return response.Models.FirstOrDefault(); // Возвращаем нового пользователя
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in RegisterAsync: {ex.Message}");
            return null; // Ошибка при регистрации
        }
    }


    // Вход
    public async Task<users?> LoginAsync(string email, string password)
    {
        try
        {
            var user = await _client.From<users>()
                .Filter("email", Supabase.Postgrest.Constants.Operator.Equals, email)
                .Single();

            if (user == null)
            {
                Console.WriteLine("User not found in LoginAsync.");
                return null; // Пользователь не найден
            }

            // Если пароль не передается, пропускаем проверку пароля (для Google)
            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("User found (no password check).");
                return user; // Возвращаем пользователя без проверки пароля
            }

            var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<users>();
            var result = passwordHasher.VerifyHashedPassword(user, user.password_hash, password);

            if (result != Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success)
            {
                Console.WriteLine("Password mismatch.");
                return null; // Неверный пароль
            }

            Console.WriteLine("User found in LoginAsync.");
            return user; // Пользователь найден
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in LoginAsync: {ex.Message}");
            return null; // Ошибка
        }
    }
}
