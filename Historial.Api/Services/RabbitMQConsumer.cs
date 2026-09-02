using Historial.Api.Data;
using Historial.Api.Events;
using Historial.Api.Models;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Historial.Api.Services
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RabbitMQConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        private IConnection? _connection;
        private IModel? _channel;

        public RabbitMQConsumer(
            IConfiguration configuration,
            ILogger<RabbitMQConsumer> logger,
            IServiceScopeFactory scopeFactory)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var hostName = _configuration["RabbitMQ:HostName"] ?? throw new InvalidOperationException("RabbitMQ:HostName no configurado");
            var portStr = _configuration["RabbitMQ:Port"] ?? throw new InvalidOperationException("RabbitMQ:Port no configurado");
            var userName = _configuration["RabbitMQ:UserName"] ?? throw new InvalidOperationException("RabbitMQ:UserName no configurado");
            var password = _configuration["RabbitMQ:Password"] ?? throw new InvalidOperationException("RabbitMQ:Password no configurado");

            var factory = new ConnectionFactory
            {
                HostName = hostName,
                Port = int.Parse(portStr),
                UserName = userName,
                Password = password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            var queueName = _configuration["RabbitMQ:QueueName"] ?? throw new InvalidOperationException("RabbitMQ:QueueName no configurado");

            _channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (sender, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var mensaje = Encoding.UTF8.GetString(body);
                    var evento = JsonSerializer.Deserialize<PacienteCreadoEvento>(mensaje);

                    if (evento != null)
                    {
                        _logger.LogInformation(
                            "Paciente creado recibido. IdPaciente: {IdPaciente}",
                            evento.IdPaciente
                        );

                        using var scope = _scopeFactory.CreateScope();
                        var dbContext = scope.ServiceProvider
                            .GetRequiredService<HistorialDBContext>();

                        var existe = dbContext.HistorialClinico
                            .Any(h => h.IdPaciente == evento.IdPaciente);

                        if (!existe)
                        {
                            var historial = new HistorialClinico
                            {
                                IdPaciente = evento.IdPaciente,
                                NumHistoria = $"H-{evento.IdPaciente:D6}",
                                Diagnostico = "Pendiente de diagnóstico",
                                Tratamiento = "Pendiente de tratamiento",
                                Fecha = DateTime.Now
                            };

                            dbContext.HistorialClinico.Add(historial);
                            dbContext.SaveChanges();

                            _logger.LogInformation(
                                "Historial clínico creado automáticamente para IdPaciente: {IdPaciente}",
                                evento.IdPaciente
                            );
                        }
                    }

                    _channel.BasicAck(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error procesando mensaje");
                    if (_channel != null)
                    {
                        _channel.BasicNack(
                            deliveryTag: ea.DeliveryTag,
                            multiple: false,
                            requeue: true
                        );
                    }
                }
            };

            _channel.BasicConsume(
                queue: queueName,
                autoAck: false,
                consumer: consumer
            );

           
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}