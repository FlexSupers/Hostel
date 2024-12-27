using RabbitMQ.Client;
using Suo.Admin.Data.ViewModel;
using System.Text;
using Suo.Admin.Extentions;

namespace Suo.Admin.Data.RabbitMqService
{
    public class RabbitService
    {
        private readonly AppConfiguration _configuration;
        private ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;
        public RabbitService(AppConfiguration configuration)
        {
            _configuration = configuration;
            //_factory = new ConnectionFactory { HostName = _configuration.RabbitMqHostName, UserName = "ruser", Password = "wlad1051" };
            _factory = new ConnectionFactory { HostName = _configuration.RabbitMQHostName, UserName = _configuration.RabbitMQUserName, Password = _configuration.RabbitMQPassword };

            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public async Task CreateQueue()
        {
            _channel.QueueDeclare(queue: "suoMesages",
               durable: true,
               exclusive: false,
               autoDelete: false,
               arguments: null);
        }

        public async Task SendMessageToTgBot(MessageModelForTg messageModel)
        {
            CreateQueue();//стоит ли вызывать каждый раз
            string jsonModel = System.Text.Json.JsonSerializer.Serialize(messageModel);
            _channel.BasicPublish(exchange: string.Empty,
                routingKey: "suoMesages",
                body: ConvertMessage(jsonModel));
        }

        private byte[] ConvertMessage(string jsonModel)
        {
            return Encoding.UTF8.GetBytes(jsonModel);
        }
    }
}
