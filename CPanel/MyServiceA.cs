using CPanel.MqttServer;
using Microsoft.Extensions.Configuration;
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
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("MyServiceA is starting.");

            var ParametersList = mqttServerClient.GetParameters();
            var topicList = new List<string>();
            await mqttServerClient.Connect(
                Configuration["Mqtt:ip"],
                Configuration["Mqtt:port"],
                Configuration["Mqtt:login"],
                Configuration["Mqtt:password"]);
            foreach (var parameter in ParametersList)
            {
                topicList.Add(parameter.Topic);
            }
            await mqttServerClient.Subscribe(topicList);
            System.Diagnostics.Debug.WriteLine("Mqtt is started");
            await Task.Run(mqttServerClient.WaitForReciveMessage);
            System.Diagnostics.Debug.WriteLine("SignalR is started");

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
