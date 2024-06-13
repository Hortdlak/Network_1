    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    namespace NetworkingCommon
    {
        public static class UdpHelper
        {
            public static async Task SendAsync(Message msg, IPEndPoint endPoint)
            {
                using var udpClient = new UdpClient();
                var jsonMessage = msg.SerializeToJson();
                var buffer = Encoding.UTF8.GetBytes(jsonMessage);

                for (int i = 0; i < 3; i++)
                    await udpClient.SendAsync(buffer, buffer.Length, endPoint);

                Console.WriteLine("Сообщение отправлено");
            }

            public static async Task<Message?> ReceiveAsync(int port, CancellationToken token = default)
            {
                using var udpClient = new UdpClient(port);
                try
                {
                    Console.WriteLine("Ожидание подключения");
                    var data = await udpClient.ReceiveAsync(token);
                    var jsonMessage = Encoding.UTF8.GetString(data.Buffer);
                    var message = Message.DeserializeFromJson(jsonMessage);

                    Console.WriteLine("Сообщение получено.");
                    Console.WriteLine(message);

                    return message;
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Операция отменена!");
                    return null;
                }
            }
        }
    }
