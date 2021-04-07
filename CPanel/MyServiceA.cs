using CPanel.MqttServer;
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
        public IConfiguration Configuration { get; }
        private MqttServerClient mqttServerClient;

        public MyServiceA(MqttServerClient mqttServerClient, IConfiguration configuration)
        {
            this.mqttServerClient = mqttServerClient;
            Configuration = configuration;
        }
        private async Task StartMqtt()
        {
            mqttServerClient.Client = new MqttFactory().CreateMqttClient();
            mqttServerClient.options = new MqttClientOptionsBuilder()
                            .WithTcpServer(Configuration["Mqtt:ip"], int.Parse(Configuration["Mqtt:port"]))
                            .WithCredentials(Configuration["Mqtt:login"], Configuration["Mqtt:password"])
                            .WithProtocolVersion(MqttProtocolVersion.V311)
                            .Build();
            while (true)
            {
                while (mqttServerClient.Client.IsConnected != true)
                {
                    mqttServerClient.Auth = await mqttServerClient.Client.ConnectAsync(mqttServerClient.options);
                    if (mqttServerClient.Client.IsConnected != true)
                        await Task.Delay(1000);
                }
                await Task.Delay(5000);
            }
        }
        private async Task StartSignalR()
        {
            while (true)
            {
                while (mqttServerClient.connection.State != HubConnectionState.Connected)
                {
                    await mqttServerClient.connection.StartAsync();
                    if (mqttServerClient.connection.State != HubConnectionState.Connected)
                        await Task.Delay(1000);
                }
                await Task.Delay(5000);
            }
        }
        private async Task StartSubscribe()
        {
            var ParametersList = mqttServerClient.GetParameters();
            var topicList = new List<string>();
            foreach (var parameter in ParametersList)
            {
                topicList.Add(parameter.Topic);
            }
            await mqttServerClient.Subscribe(topicList);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("MyServiceA is starting.");

            _ = Task.Run(async () => await StartMqtt(), stoppingToken);
            Console.WriteLine("Mqtt is started");
            _ = Task.Run(async () => await StartSignalR(), stoppingToken);
            Console.WriteLine("SignalR is started");
            _ = Task.Run(async () => await StartSubscribe(), stoppingToken);
            Console.WriteLine("Mqtt is started");
            _ = Task.Run(mqttServerClient.WaitForReciveMessage, stoppingToken);
            Console.WriteLine("WaitForReciveMessage is started");

            stoppingToken.Register(() => Console.WriteLine("MyServiceA is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("MyServiceA is doing background work.");

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            Console.WriteLine("MyServiceA background task is stopping.");
        }
    }
}
