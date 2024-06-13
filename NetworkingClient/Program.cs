namespace NetworkingClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var client = new UdpClientApp("127.0.0.1", 12345, 12346);
            await client.StartAsync();
        }
    }
}
