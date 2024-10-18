using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestServerProject
{
    public class Server
    {
        private readonly int _port;
        private readonly string _ipAddress;
        private TcpListener _listener;
        private UserDatabase _userDatabase;
        private Authenticator _authenticator; // Добавляем поле для Authenticator

        public Server(int port, string ipAddress)
        {
            _port = port;
            _ipAddress = ipAddress;
            _userDatabase = new UserDatabase(); // Инициализация базы данных пользователей
            _authenticator = new Authenticator(_userDatabase); // Инициализация Authenticator
        }

        public async Task StartAsync(CancellationToken token)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(_ipAddress);
                _listener = new TcpListener(ip, _port);
                _listener.Start();

                Console.WriteLine("Сервер запущен и ожидает подключений...");

                // Бесконечный цикл для обработки входящих подключений
                while (!token.IsCancellationRequested)
                {
                    if (_listener.Pending())
                    {
                        TcpClient client = await _listener.AcceptTcpClientAsync();
                        // Обработка клиентского подключения
                        _ = HandleClientAsync(client, token);
                    }
                    else
                    {
                        // Делаем небольшую задержку, чтобы избежать перегрузки процессора
                        await Task.Delay(100);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при запуске сервера: {ex.Message}");
            }
            finally
            {
                _listener?.Stop();
                Console.WriteLine("Получен запрос на остановку сервера.");
            }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken token)
        {
            Console.WriteLine("Клиент подключен.");

            try
            {
                using (NetworkStream stream = client.GetStream())
                {
                    // Получение логина
                    byte[] buffer = new byte[256];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Получено сообщение от клиента: {message}");

                    // Проверка, начинается ли сообщение с "login:"
                    if (message.StartsWith("login:"))
                    {
                        string username = message.Substring(6); // Извлекаем имя пользователя
                        string salt = _userDatabase.GetSaltForUser(username);

                        // Отправка сообщения 401 Unauthorized клиенту с солью
                        string unauthorizedMessage = $"401 Unauthorized: {salt}";
                        byte[] unauthorizedBuffer = Encoding.UTF8.GetBytes(unauthorizedMessage);
                        await stream.WriteAsync(unauthorizedBuffer, 0, unauthorizedBuffer.Length, token);
                        Console.WriteLine($"Отправлено клиенту: {unauthorizedMessage}");

                        // Теперь ожидаем хэш пароля от клиента
                        buffer = new byte[1024]; // буфер для получения сообщения
                        bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);

                        if (bytesRead > 0)
                        {
                            string clientMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            Console.WriteLine($"Получено сообщение от клиента: {clientMessage}");

                               

                                // Создаем экземпляр Authenticator
                                var authenticator = new Authenticator(_userDatabase);
                                // Проверяем пользователя
                                bool isValidUser = authenticator.ValidateUser(username, clientMessage);

                                if (isValidUser)
                                {
                                    string successMessage = "200 OK: Аутентификация успешна";
                                    byte[] successBuffer = Encoding.UTF8.GetBytes(successMessage);
                                    await stream.WriteAsync(successBuffer, 0, successBuffer.Length, token);
                                    Console.WriteLine($"Отправлено клиенту: {successMessage}");
                                }
                                else
                                {
                                    string failureMessage = "401 Unauthorized: Неверные учетные данные";
                                    byte[] failureBuffer = Encoding.UTF8.GetBytes(failureMessage);
                                    await stream.WriteAsync(failureBuffer, 0, failureBuffer.Length, token);
                                    Console.WriteLine($"Отправлено клиенту: {failureMessage}");
                                }
          
                        }
                    }
                    else
                    {
                        // Отправка сообщения об ошибке и закрытие соединения
                        string errorMessage = "403 Forbidden: Login header error";
                        byte[] errorBuffer = Encoding.UTF8.GetBytes(errorMessage);
                        await stream.WriteAsync(errorBuffer, 0, errorBuffer.Length, token);
                        Console.WriteLine($"Ошибка: {errorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обработке клиента: {ex.Message}");
            }
            finally
            {
                client.Close();
                Console.WriteLine("Соединение закрыто.");
            }
        }



    }
}
