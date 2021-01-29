using Microsoft.AspNetCore.Mvc;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Subscribing;
using MQTTnet.Formatter;
using System;
using System.Text;
using System.Threading.Tasks;
using USQLCSharp.DataAccess;
using USQLCSharp.Models;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
namespace CPanel.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class MqttController : ControllerBase
    {
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
        [HttpGet("{id}")]
        public async Task Send(string id)
        {
            await Connect("176.36.127.144", "1883", "yaroslav", "220977qQ");
            await Client.PublishAsync("yaroslav/Kitchen/output2", id);
        }
        [HttpGet("data")]    
        public async Task Data()
        {
            await Connect("176.36.127.144", "1883", "yaroslav", "220977qQ");
            var result = (await Client.SubscribeAsync(
                         new TopicFilterBuilder()
                         .WithTopic($"yaroslav/TableLamp/output5")
                         .Build()
                     )).Items.ToList();
            switch (result[0].ResultCode)
            {
                case MqttClientSubscribeResultCode.GrantedQoS0:
                case MqttClientSubscribeResultCode.GrantedQoS1:
                case MqttClientSubscribeResultCode.GrantedQoS2:
                    Client.UseApplicationMessageReceivedHandler(me =>
                    {
                        var msg = me.ApplicationMessage;
                        using var db = new PeopleContext();
                        var item = db.Parameters.FirstOrDefault(x => x.Topic == msg.Topic);
                        if (item != null)
                        {
                            item.Data = Encoding.UTF8.GetString(msg.Payload);
                        }
                        else
                        {
                            var device = new Parameter();
                            device.Name = "Lamp";
                            device.Data = Encoding.UTF8.GetString(msg.Payload);
                            device.Topic = msg.Topic;
                            db.Add(device);
                        }
                        db.SaveChanges();
                         
                    });
                    break;
                default:
                    throw new Exception(result[0].ResultCode.ToString());
            }
        }
        [HttpGet("leng")]
        public List<Parameter> get(int id)
        {
            using var db = new PeopleContext();
            return db.Parameters.Select(x => new Parameter
            {
                Id = x.Id,
                Topic = x.Topic,
                Data = x.Data,
                Name = x.Name
            }).ToList();
        }
    }
}