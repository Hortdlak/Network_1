using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetworkingCommon;

namespace NetworkingServer
{
    public class UdpServer
    {
        private readonly int _receivePort;
        private readonly int _sendPort;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly IPEndPoint _sendEndPoint;
        private readonly UdpClient _udpClient;

        public UdpServer(int receivePort, int sendPort)
        {
            _receivePort = receivePort;
            _sendPort = sendPort;
            _cancellationTokenSource = new CancellationTokenSource();
            _sendEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _sendPort);
            _udpClient = new UdpClient(_receivePort);
        }

        public async Task StartAsync()
        {
            var token = _cancellationTokenSource.Token;
            Console.WriteLine("Сервер запущен");

            try
            {
                while (!token.IsCancellationRequested)
                {
                    var result = await _udpClient.ReceiveAsync();
                    HandleClient(result, token);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Операция отменена");
            }
            finally
            {
                _udpClient.Close();
            }

            Console.WriteLine("Сервер выключен");
        }

        private void HandleClient(UdpReceiveResult result, CancellationToken token)
        {
            Task.Run(async () =>
            {
                var buffer = result.Buffer;
                var jsonMessage = Encoding.UTF8.GetString(buffer);
                var message = Message.DeserializeFromJson(jsonMessage);
                Console.WriteLine($"Сообщение получено от {message?._from}: {message?._text}");

                var responseMessage = new Message("Ок", "Сервер", message?._from);
                if (message._text.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    responseMessage._text = "Сервер выключен";
                    _cancellationTokenSource.Cancel();
                    throw new OperationCanceledException("Сервер завершил работу");
                }

                var responseJson = responseMessage.SerializeToJson();
                var responseBuffer = Encoding.UTF8.GetBytes(responseJson);

                await _udpClient.SendAsync(responseBuffer, responseBuffer.Length, result.RemoteEndPoint);
                Console.WriteLine($"Ответ отправлен {message?._from}");
            }, token);
        }
    }
}
