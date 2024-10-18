using System;
using System.Collections.Generic;

public class UserDatabase
{
    private readonly Dictionary<string, (string passwordHash, string salt, string pepper)> _users;
    private readonly string _defaultSalt; // Стандартная соль

    public UserDatabase()
    {
        // Инициализация тестовых пользователей с хэшированными паролями, солью и перцем
        _users = new Dictionary<string, (string, string, string)>
        {
            { "user1", ("WpDLNWiSDgtaV4UY0FsReDkyY9Im6SepS0oqS3I3BCY=", "соль_для_user1", "перец_для_user1") },
            { "user2", ("<хэш_пароля_для_user2>", "соль_для_user2", "перец_для_user2") }
        };

        _defaultSalt = "стандартная_соль"; // Установите вашу стандартную соль здесь
    }

    public string GetSaltForUser(string username)
    {
        if (_users.TryGetValue(username, out var userData))
        {
            return userData.salt; // Возвращаем соль пользователя
        }

        return _defaultSalt; // Возвращаем стандартную соль, если пользователь не найден
    }

    public string GetPepperForUser(string username)
    {
        if (_users.TryGetValue(username, out var userData))
        {
            return userData.pepper; // Возвращаем перец пользователя
        }

        return null; // Возвращаем null, если пользователь не найден
    }

    // Новый метод для получения хэша пароля пользователя
    public string GetHashForUser(string username)
    {
        if (_users.TryGetValue(username, out var userData))
        {
            return userData.passwordHash; // Возвращаем хэш пароля пользователя
        }

        return null; // Возвращаем null, если пользователь не найден
    }
}
