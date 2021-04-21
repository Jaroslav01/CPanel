using CPanel.MqttServer;
using CPanel.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Formatter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPanel
{
    public class MyServiceA : BackgroundService
    {
        public IConfiguration _configuration { get; }
        private MqttServerClient _mqttServerClient;
        private readonly SignalRClient _signalRClient;

        public MyServiceA(MqttServerClient mqttServerClient, IConfiguration configuration, SignalRClient signalRClient)
        {
            _mqttServerClient = mqttServerClient;
            _configuration = configuration;
            _signalRClient = signalRClient;
        }
        private async Task StartMqtt()
        {
            _mqttServerClient.Client = new MqttFactory().CreateMqttClient();
            _mqttServerClient.options = new MqttClientOptionsBuilder()
                            .WithTcpServer(_configuration["Mqtt:ip"], int.Parse(_configuration["Mqtt:port"]))
                            .WithCredentials(_configuration["Mqtt:login"], _configuration["Mqtt:password"])
                            .WithProtocolVersion(MqttProtocolVersion.V311)
                            .Build();
            while (_mqttServerClient.Client.IsConnected != true)
            {
                _mqttServerClient.Auth = await _mqttServerClient.Client.ConnectAsync(_mqttServerClient.options);
                if (_mqttServerClient.Client.IsConnected != true)
                {
                    Console.WriteLine("### CONNECTING FAILED ###");
                    await Task.Delay(1000);
                }
                Console.WriteLine("### CONNECTING SUCESSFUL ###");
            }
        }
        private async Task StartSubscribe()
        {
            var parametersList = _mqttServerClient.GetParameters();
            var topicList = new List<string>();
            foreach (var parameter in parametersList)
            {
                topicList.Add(parameter.Topic);
            }
            await _mqttServerClient.Subscribe(topicList);
        }
        private async Task ReconnectHendler()
        {
            _mqttServerClient.Client.UseDisconnectedHandler(async e =>
            {
                Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                await Task.Delay(TimeSpan.FromSeconds(5));
                try
                {
                    await _mqttServerClient.Client.ReconnectAsync(); // Since 3.0.5 with CancellationToken
                    Console.WriteLine("### RECONNECTING SUCESSFUL ###");
                }
                catch
                {
                    Console.WriteLine("### RECONNECTING FAILED ###");
                }
            });
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //Console.WriteLine("MyServiceA is starting.");
            await StartMqtt();
            await _signalRClient.connection.StartAsync();
            await _mqttServerClient.AddTopicsForSubscribe();
            await _mqttServerClient.WaitForReciveMessage();
            await ReconnectHendler();

            //stoppingToken.Register(() => Console.WriteLine("MyServiceA is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            Console.WriteLine("MyServiceA background task is stopping.");
        }
    }
}
