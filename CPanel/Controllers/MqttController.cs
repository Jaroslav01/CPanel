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
        public async Task Send(int id)
        {
            await Connect("176.36.127.144", "1883", "yaroslav", "220977qQ");
            await Client.PublishAsync("yaroslav/Kitchen/output2", $"{id}");
        }









        public IEnumerable<Mqtt> Senddata(string topic, string data)
        {
            return Enumerable.Range(1, 5).Select(index => new Mqtt
            {
                Topic = topic,
                Data = data
            }).ToArray();
        }
        [HttpGet]
        public async Task<Array> Subscribe()
        {
            Array A = Senddata("yaroslav/Kitchen/output2", "Empty").ToArray();
            await Connect("176.36.127.144", "1883", "yaroslav", "220977qQ");

            MqttClientSubscribeResultItem result = (await Client.SubscribeAsync(
                        new TopicFilterBuilder()
                        .WithTopic("yaroslav/Kitchen/output2")
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
                        var data = Encoding.UTF8.GetString(msg.Payload);
                        A = Senddata(msg.Topic, data).ToArray();
                    });
                    return A; 

                default:
                    throw new Exception(result.ResultCode.ToString());
            }
        }
    }
}
