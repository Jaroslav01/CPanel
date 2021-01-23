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
        public async Task Send(string id, string topic)
        {
            await Connect("176.36.127.144", "1883", "yaroslav", "220977qQ");
            await Client.PublishAsync("yaroslav/Kitchen/output0", id);
        }
        [HttpGet("sub")]
        public async Task Subscribe()
        {

            await Connect("176.36.127.144", "1883", "yaroslav", "220977qQ");
            MqttClientSubscribeResultItem result = (await Client.SubscribeAsync(
                         new TopicFilterBuilder()
                         .WithTopic("yaroslav/Kitchen/output0")
                         .Build()
                     )).Items[0];
            switch (result.ResultCode)
            {
                case MqttClientSubscribeResultCode.GrantedQoS0:
                case MqttClientSubscribeResultCode.GrantedQoS1:
                case MqttClientSubscribeResultCode.GrantedQoS2:
                    Client.UseApplicationMessageReceivedHandler(me =>
                    {
                        var msg = me.ApplicationMessage;
                        using var db = new PeopleContext();
                        var item = db.Devices.FirstOrDefault(x => x.Topic == msg.Topic);
                        if (item != null)
                        {
                            item.Data = Encoding.UTF8.GetString(msg.Payload);
                        }
                        else
                        {
                            var device = new Device();
                            device.Data = Encoding.UTF8.GetString(msg.Payload);
                            device.Topic = msg.Topic;
                            db.Add(device);
                        }
                        db.SaveChanges();
                    });
                    break;
                default:
                    throw new Exception(result.ResultCode.ToString());
            }
        }
    }
}