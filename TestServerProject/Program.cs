using System;
using System.Threading.Tasks;

namespace TestServerProject
{
    class Program
    {
        static async Task Main(string[] args)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            Server server = new Server(10555, "127.0.0.14");

            // Запуск сервера в отдельном Task с токеном отмены
            var serverTask = server.StartAsync(token);

            // Ожидание нажатия клавиши для остановки сервера
            Console.WriteLine("Нажмите любую клавишу для остановки сервера...");
            Console.ReadKey();

            // Запрашиваем остановку сервера
            cancellationTokenSource.Cancel();

            // Ждем завершения сервера
            await serverTask;

            Console.WriteLine("Сервер остановлен.");
        }
    }
}