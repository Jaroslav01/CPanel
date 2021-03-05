using CPanel.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace CPanel
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MqttController mqttController = new MqttController();
            Thread thread1 = new Thread(mqttController.UpdateDataAsync);
            thread1.Start();
            CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                /*.ConfigureServices(services =>
                {
                    services.AddHostedService<VideosWatcher>();
                })*/;
    }
}
