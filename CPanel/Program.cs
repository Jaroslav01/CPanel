using CPanel.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading;

namespace CPanel
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MqttController mqttController = new MqttController();
            Thread thread1 = new Thread(mqttController.knjc);
            thread1.Start();
            CreateHostBuilder(args).Build().Run();

            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
