using CPanel.Hubs;
using Microsoft.AspNetCore.Mvc;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Subscribing;
using MQTTnet.Formatter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USQLCSharp.DataAccess;
using USQLCSharp.Models;
using Microsoft.AspNetCore.SignalR.Client;
using CPanel.Controllers;

namespace CPanel.MqttServer
{
    public class MqttServerClient : ControllerBase
    {

        HubConnection connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5001/hub")
                .WithAutomaticReconnect()
                .Build();
        IMqttClientOptions options;
        private PeopleContext db = new PeopleContext();
        private IMqttClient Client { get; set; }
        public MqttClientAuthenticateResult Auth { get; private set; } = new MqttClientAuthenticateResult();
        public List<MqttClientSubscribeResultItem> Result { get; set; } = new List<MqttClientSubscribeResultItem>();
        public async Task Connect(string ip, string port, string login, string password)
        {
            Client = new MqttFactory().CreateMqttClient();
            options = new MqttClientOptionsBuilder()
                            .WithTcpServer(ip, int.Parse(port))
                            .WithCredentials(login, password)
                            .WithProtocolVersion(MqttProtocolVersion.V311)
                            .Build();
            do
            {
                Auth = await Client.ConnectAsync(options);
            } while (Client.IsConnected != true);
            do
            {
                await connection.StartAsync();
            } while (connection.State != HubConnectionState.Connected);
        }
        public async Task Send(string topic, string data)
        {
            await Client.PublishAsync(topic, data);
        }
        public async Task WaitForReciveMessage()
        {
            while (true)
            {
                for (int i = 0; i < Result.Count; i++)
                {
                    switch (Result[i].ResultCode)
                    {
                        case MqttClientSubscribeResultCode.GrantedQoS0:
                        case MqttClientSubscribeResultCode.GrantedQoS1:
                        case MqttClientSubscribeResultCode.GrantedQoS2:
                            Client.UseApplicationMessageReceivedHandler(me =>
                            {
                                var msg = me.ApplicationMessage;
                                var item = db.Parameters.FirstOrDefault(x => x.Topic == msg.Topic);
                                if (item != null)
                                {
                                    connection.StartAsync();
                                    if (msg.Payload != null)
                                    item.Data = Encoding.UTF8.GetString(msg.Payload);
                                    connection.SendAsync("MqttSync", "update", item.Id, item.DeviseId, item.Name, item.Topic, item.Data, item.Type);
                                }
                                db.SaveChanges();
                            });
                            break;
                        default:
                            throw new Exception(Result[i].ResultCode.ToString());
                    }
                }
            }
        }
        public async Task GetTopicsForSubscribe()
        {
            var mqttController = new MqttController();
            var parametersList = mqttController.GetParameters();
            var topicList = new List<string>();
            for (int i = 0; i < parametersList.Count; i++)
            {
                for (int j = 0; j < Result.Count; j++)
                {
                    if (parametersList[i].Topic == Result[j].ResultCode.ToString()) continue;
                    topicList.Add(parametersList[i].Topic);
                }
            }
            await Subscribe(topicList);
        }
        public async Task Subscribe(List<string> topicList)
        {
            foreach (var topic in topicList)
            {
                MqttClientSubscribeResultItem resultItems = (await Client.SubscribeAsync(
                    new TopicFilterBuilder()
                    .WithTopic(topic)
                    .Build()
                )).Items[0];
                Result.Add(resultItems);
            }
        }
    }
}