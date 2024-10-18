using Konscious.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServerProject
{
    public class Authenticator
    {
        private readonly UserDatabase _userDatabase;

        public Authenticator(UserDatabase userDatabase)
        {
            _userDatabase = userDatabase;
        }

        public bool ValidateUser(string username, string inputPassword)
        {
            // Получаем перец для пользователя
            string pepper = _userDatabase.GetPepperForUser(username);

            // Если пользователь не найден, возвращаем false
            if (pepper == null) return false;

            // Хэшируем введённый пароль с солью и перцем
            string calculatedHash = HashPassword(inputPassword, pepper);
            Console.WriteLine($"Calculated hash: {calculatedHash}");

            // Получаем хэш пароля для сравнения
            string storedHash = _userDatabase.GetHashForUser(username); // Получаем хэш из базы данных

            // Сравниваем захэшированный ввод с сохранённым хэшем
            return storedHash == calculatedHash;
        }

        private string HashPassword(string password, string salt)
        {
            using (var hasher = new Argon2i(System.Text.Encoding.UTF8.GetBytes(password)))
            {
                hasher.Salt = System.Text.Encoding.UTF8.GetBytes(salt); // Соль
                hasher.DegreeOfParallelism = 1; // Число потоков (можно настроить)
                hasher.Iterations = 4; // Количество итераций (можно настроить)
                hasher.MemorySize = 65536; // Объём памяти (можно настроить)

                // Добавляем перец в комбинированный массив
                hasher.GetBytes(32); // Получаем хэш длиной 32 байта

                byte[] hash = hasher.GetBytes(32); // Получаем хэш длиной 32 байта

                // Преобразование хэша в строку для вывода
                return Convert.ToBase64String(hash);
            }
        }
    }
}
