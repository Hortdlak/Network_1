using System.Net;
using NetworkingCommon;

namespace NetworkingClient
{
    public class UdpClientApp
    {
        private readonly IPEndPoint _serverEndPoint;
        private readonly int _receivePort;

        public UdpClientApp(string serverAddress, int serverPort, int receivePort)
        {
            _serverEndPoint = new IPEndPoint(IPAddress.Parse(serverAddress), serverPort);
            _receivePort = receivePort;
        }

        public async Task StartAsync()
        {
            string? input;
            var message = new Message("", "Сергей", "Сервер");

            do
            {
                Console.WriteLine("Введите сообщение или нажмите 'exit'");
                input = Console.ReadLine();
                message._text = input;
                await UdpHelper.SendAsync(message, _serverEndPoint);

                var responseMessage = await UdpHelper.ReceiveAsync(_receivePort);
                input = responseMessage?._text?.ToLower();
            } while (!input!.Equals("сервер выключен", StringComparison.OrdinalIgnoreCase));
        }
    }
}
