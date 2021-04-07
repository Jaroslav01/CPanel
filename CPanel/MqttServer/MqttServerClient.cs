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

namespace CPanel.MqttServer
{
    public class MqttServerClient
    {
        private readonly IServiceProvider services;
        public MqttServerClient(IServiceProvider services)
        {
            this.services = services;
        }
        public HubConnection connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5001/hub")
                .WithAutomaticReconnect()
                .Build();
        public IMqttClientOptions options;
        public IMqttClient Client { get; set; }
        public MqttClientAuthenticateResult Auth { get; set; } = new MqttClientAuthenticateResult();
        public List<MqttClientSubscribeResultItem> Result { get; set; } = new List<MqttClientSubscribeResultItem>();
        public async Task Send(string topic, string data)
        {
            await Client.PublishAsync(topic, data);
        }
        private async Task MessageHendler(MqttApplicationMessageReceivedEventArgs me)
        {
            var db = (PeopleContext)services.GetService(typeof(PeopleContext));
            var msg = me.ApplicationMessage;
            var item = db.Parameters.FirstOrDefault(x => x.Topic == msg.Topic);
            if (item != null)
            {
                if (msg.Payload != null)
                    item.Data = Encoding.UTF8.GetString(msg.Payload);
                await connection.SendAsync("MqttSync", "update", item.Id, item.DeviseId, item.Name, item.Topic, item.Data, item.Type);
            }
            db.SaveChanges();
        }
        public async Task WaitForReciveMessage()
        {
            Client.UseApplicationMessageReceivedHandler(MessageHendler);
        }
        public async Task AddTopicsForSubscribe()
        {
            var parametersList = GetParameters();
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
            var allowdTopics = new List<MqttClientSubscribeResultCode>() {
                MqttClientSubscribeResultCode.GrantedQoS0,
                MqttClientSubscribeResultCode.GrantedQoS1,
                MqttClientSubscribeResultCode.GrantedQoS2
            };
            foreach (var topic in topicList)
            {
                MqttClientSubscribeResultItem resultItems = (await Client.SubscribeAsync(
                    new TopicFilterBuilder()
                    .WithTopic(topic)
                    .Build()
                )).Items[0];
                if (!allowdTopics.Contains(resultItems.ResultCode))
                {
                    throw new Exception("Unsucssesful subscribtion on topic");
                }
            }
        }
        public async Task Unsubscribe(string topic)
        {
            MqttClientSubscribeResultItem resultItems = (await Client.SubscribeAsync(
                    new TopicFilterBuilder()
                    .WithTopic(topic)
                    .Build()
                )).Items[0];
            Result.Remove(resultItems);
        }
        public List<Parameter> GetParameters()
        {
            var db = (PeopleContext)services.GetService(typeof(PeopleContext));
            var response = db.Parameters.Select(x => new Parameter
            {
                Id = x.Id,
                Data = x.Data,
                DeviseId = x.DeviseId,
                Name = x.Name,
                Topic = x.Topic,
                Type = x.Type
            }).ToList();
            return response;
        }
    }
}