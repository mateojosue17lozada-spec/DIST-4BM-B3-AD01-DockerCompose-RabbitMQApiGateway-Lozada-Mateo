using Pacientes.Api.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Pacientes.Api.Services
{
    public class RabbitMQPublisher
    {
        private readonly IConfiguration _configuration;

        public RabbitMQPublisher(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void PublicarPacienteCreado(Paciente paciente)  
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:HostName"],
                Port = int.Parse(_configuration["RabbitMQ:Port"]!),
                UserName = _configuration["RabbitMQ:UserName"],
                Password = _configuration["RabbitMQ:Password"]
            };

            using var connection = factory.CreateConnection(); 
            using var channel = connection.CreateModel();      

            var queueName = _configuration["RabbitMQ:QueueName"]!;

            channel.QueueDeclare(  
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var mensaje = JsonSerializer.Serialize(paciente);
            var body = Encoding.UTF8.GetBytes(mensaje);

            channel.BasicPublish(  
                exchange: "",
                routingKey: queueName,
                body: body
            );
        }
    }
}