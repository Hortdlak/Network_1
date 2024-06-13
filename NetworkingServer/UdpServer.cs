using System.Net;
using NetworkingCommon;

namespace NetworkingServer
{
    public class UdpServer
    {
        private readonly int _receivePort;
        private readonly int _sendPort;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly IPEndPoint _sendEndPoint;

        public UdpServer(int receivePort, int sendPort)
        {
            _receivePort = receivePort;
            _sendPort = sendPort;
            _cancellationTokenSource = new CancellationTokenSource();
            _sendEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _sendPort);
        }

        public async Task StartAsync()
        {
            var token = _cancellationTokenSource.Token;
            string? receivedText;
            var responseMessage = new Message("Ок", "Сервер", "Сергей");

            while (!token.IsCancellationRequested)
            {
                var receivedMessage = await UdpHelper.ReceiveAsync(_receivePort, token);
                receivedText = receivedMessage?._text;

                if (receivedText!.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    responseMessage._text = "Сервер выключен";
                    _cancellationTokenSource.Cancel();
                }

                await UdpHelper.SendAsync(responseMessage, _sendEndPoint);
            }

            Console.WriteLine("Сервер выключен");
        }
    }
}
