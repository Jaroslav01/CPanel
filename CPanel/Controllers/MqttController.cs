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
using CPanel.Hubs;
//using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading;

namespace CPanel.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class MqttController : ControllerBase
    {
        public readonly HubConnection connection = new HubConnectionBuilder()
                    .WithUrl("https://localhost:5001/Hub")
                    .WithAutomaticReconnect()
                   .Build();
        public async void Start()
        {
            await connection.StartAsync();
        }
        public IMqttClient Client { get; private set; }
        public MqttClientAuthenticateResult Auth { get; private set; }
        public async Task Connect(string ip, string port, string login, string password)
        {
            IMqttClient client = new MqttFactory().CreateMqttClient();
            Client = client;
            IMqttClientOptions options = new MqttClientOptionsBuilder()
                .WithTcpServer(ip, int.Parse(port))
                .WithCredentials(login, password)
                .WithProtocolVersion(MqttProtocolVersion.V311)
                .Build();
            Auth = await client.ConnectAsync(options);
        }
        [HttpGet("Set")]
        public async Task Send(string topic, string value)
        {
            //await Connect("176.36.127.144", "1883", "yaroslav", "220977qQ");
            await Connect("localhost", "1883", "yaroslav", "220977qQ");
            await Client.PublishAsync(topic, value);
        }
        [HttpGet("update")]
        public async Task Update(string topic, string? name = null)
        {
            //await Connect("176.36.127.144", "1883", "yaroslav", "220977qQ");
            await Connect("localhost", "1883", "yaroslav", "220977qQ");
            var result = (await Client.SubscribeAsync(
                         new TopicFilterBuilder()
                         .WithTopic(topic)
                         .Build()
                     )).Items.ToList();
            switch (result[0].ResultCode)
            {
                case MqttClientSubscribeResultCode.GrantedQoS0:
                case MqttClientSubscribeResultCode.GrantedQoS1:
                case MqttClientSubscribeResultCode.GrantedQoS2:
                    Client.UseApplicationMessageReceivedHandler(async me =>
                    {
                        using var db = new PeopleContext();
                        var msg = me.ApplicationMessage;
                        var item = db.Parameters.FirstOrDefault(x => x.Topic == msg.Topic);
                        await connection.StartAsync();

                        if (item != null)
                        {
                            if (name == null) name = item.Name;
                            item.Name = name;
                            item.Data = Encoding.UTF8.GetString(msg.Payload);
                            await connection.SendAsync("MqttSync", "update", item.Id, item.DeviseId, item.Name, item.Topic, item.Data);

                        }
                        else
                        {
                            if (name == null) name = "Lamp";
                            var parameter = new Parameter
                            {
                                Name = name,
                                Data = Encoding.UTF8.GetString(msg.Payload),
                                Topic = msg.Topic
                            };
                            db.Add(parameter);
                            await connection.SendAsync("MqttSync", "add", parameter.Id, parameter.DeviseId, parameter.Name, parameter.Topic, parameter.Data);
                        }
                        db.SaveChanges();
                    });
                    break;
                default:
                    throw new Exception(result[0].ResultCode.ToString());
            }
        }
        [HttpGet("GetParameters")]
        public List<Parameter> GetParameters()
        {
            using var db = new PeopleContext();
            return db.Parameters.Select(x => new Parameter
            {
                Id = x.Id,
                Data = x.Data,
                DeviseId = x.DeviseId,
                Name = x.Name,
                Topic = x.Topic
            }).ToList();
        }
        [HttpGet("Delete")]
        public void Delete(int? id)
        {
            using var db = new PeopleContext();
            var deleteOrderDetails =
                from details in db.Parameters
                where details.Id == id
                select details;
            foreach (var detail in deleteOrderDetails)
            {
                db.Parameters.Remove(detail);
            }
            db.SaveChanges();
            Start();
        }
        [HttpGet("GetDevices")]
        public List<Device> GetDevices()
        {
            using var db = new PeopleContext();
            return db.Devices.Select(x => new Device
            {
                Id = x.Id,
                Name = x.Name,
                Topic = x.Topic,
                Ip = x.Ip,
                Mac = x.Mac,
                Rssi = x.Rssi,
                Uptime = x.Uptime,
                State = x.State
            }).ToList();
        }

        public async void knjc() {
            
            ChatHub chatHub = new ChatHub();
            using var db = new PeopleContext();
            var lis = db.Parameters.Select(x => new Parameter
            {
                Topic = x.Topic,
                Id = x.Id,
                DeviseId = x.DeviseId,
                Data = x.Data,
                Name = x.Name
            }).ToList();
            for (int i = 0; i < lis.Count; i++)
            {
                Thread.Sleep(100);
                await Update(lis[i].Topic);
            }
            
        }
        
        public async void Start1()
        {
            using var db = new PeopleContext();
            var lis = db.Parameters.Select(x => new Parameter
            {
                Topic = x.Topic,
                Id = x.Id,
                DeviseId = x.DeviseId,
                Data = x.Data,
                Name = x.Name
            }).ToList();
            if(lis != null) await connection.InvokeAsync("MqttSync", lis);
        }
        /* [HttpGet("api")]
         private List<Device> Dev()
         {
             Parameter parameter = new Parameter();
             using var db = new PeopleContext();
             return db.Devices.Select(x => new Device
             {
                 Id = x.Id,
                 Topic = x.Topic,
                 Parameters = db.Parameters.Select(r => new Parameter
                 {
                     Id = r.Id,
                     DeviseId = x.Id,
                     Name = r.Name,
                     Topic = r.Topic,
                     Data = r.Data,

                 }).ToList(),
                 Name = x.Name,
                 Ip = x.Ip,
                 Mac = x.Mac,
                 Rssi = x.Rssi,
                 Uptime = x.Uptime,
                 State = x.State
             }).ToList();
         }*/
    }
}