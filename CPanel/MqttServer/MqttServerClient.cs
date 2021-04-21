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
using CPanel.SignalR;

namespace CPanel.MqttServer
{
    public class MqttServerClient
    {
        private readonly IServiceProvider services;
        private readonly SignalRClient _signalRClient;
        public MqttServerClient(IServiceProvider services, SignalRClient signalRClient)
        {
            this.services = services;
            _signalRClient = signalRClient;
        }
        public IMqttClientOptions options;
        public IMqttClient Client { get; set; }
        public MqttClientAuthenticateResult Auth { get; set; } = new MqttClientAuthenticateResult();
        public List<MqttClientSubscribeResultItem> Result { get; set; } = new List<MqttClientSubscribeResultItem>();
        public async Task Send(string topic, string value)
        {
            var messagePayload = new MqttApplicationMessageBuilder()
            .WithTopic(topic) //"yaroslav/TableLamp/output16"
            .WithPayload(value) //"0"
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce)
            .WithRetainFlag()
            .Build();
            await Client.PublishAsync(messagePayload);
        }
        private async Task MessageHendler(MqttApplicationMessageReceivedEventArgs me)
        {
            //var db = (PeopleContext)services.GetService(typeof(PeopleContext));
            using var db = new PeopleContext();
            var msg = me.ApplicationMessage;
            var item = db.Parameters.FirstOrDefault(x => x.Topic == msg.Topic);

            if (item != null)
            {
                if (msg.Payload != null)
                    item.Data = Encoding.UTF8.GetString(msg.Payload);
                await _signalRClient.connection.SendAsync("MqttSync", "update", item.Id, item.DeviseId, item.UserId, item.Name, item.Topic, item.Data, item.Type);
            }
            else
            {
                var tS = msg.Topic.Split("/");
                var devices = db.Devices.Where(x => x.Topic == tS[0] + "/" + tS[1]).ToList<Device>();
                var payload = Encoding.UTF8.GetString(msg.Payload);
                foreach (var device in devices)
                {
                    if (msg.Topic == device.Topic + "/uptime")
                    {
                        device.Uptime = Int32.Parse(payload);
                    }
                    else if (msg.Topic == device.Topic + "/mac")
                    {
                        device.Mac = payload;
                    }
                    else if (msg.Topic == device.Topic + "/wanip")
                    {
                        device.Ip = payload;
                    }
                    else if (msg.Topic == device.Topic + "/rssi")
                    {
                        device.Rssi = Int32.Parse(payload);
                    }
                    else if (msg.Topic == device.Topic + "/freemem")
                    {
                        device.FreeMem = Int32.Parse(payload);
                    }
                }
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
            var deviceList = GetDevices();
            var topicList = new List<string>();
            for (int i = 0; i < parametersList.Count; i++)
            {
                topicList.Add(parametersList[i].Topic);
            }
            for (int i = 0; i < deviceList.Count; i++)
            {
                string[] topics =
                {
                    deviceList[i].Topic + "/uptime",
                    deviceList[i].Topic + "/mac",
                    deviceList[i].Topic + "/wanip",
                    deviceList[i].Topic + "/rssi",
                    deviceList[i].Topic + "/freemem"
                };
                topicList.AddRange(topics);
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
            //var db = (PeopleContext)services.GetService(typeof(PeopleContext));
            using var db = new PeopleContext();

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
        public List<Device> GetDevices()
        {
            //var db = (PeopleContext)services.GetService(typeof(PeopleContext));
            using var db = new PeopleContext();
            var response = db.Devices.Select(x => new Device
            {
                Id = x.Id,
                Ip = x.Ip,
                Mac = x.Mac,
                Name = x.Name,
                Rssi = x.Rssi,
                Topic = x.Topic,
                Uptime = x.Uptime,
                UserId = x.UserId,
                FreeMem = x.FreeMem,
                Parameters = x.Parameters,
            }).ToList();
            return response;
        }
    }
}