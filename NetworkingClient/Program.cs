namespace NetworkingClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var tasks = new Task[10];
            for (int i = 0; i < 10; i++)
            {
                int clientId = i + 1;
                tasks[i] = Task.Run(async () =>
                {
                    var clientApp = new UdpClientApp("127.0.0.1", 12345, 12346 + clientId);
                    await clientApp.StartAsync();
                });
            }

            await Task.WhenAll(tasks);
            Console.WriteLine("Все клиенты завершили отправку сообщений.");
        }
    }
}
