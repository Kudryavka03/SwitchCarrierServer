using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Renci.SshNet;
namespace SwitchCarrierServer
{
    public class Program
    {
        public static SshClient client = new SshClient("192.168.255.1", "root", "password");
        public static void Main(string[] args)
        {
            client.Connect();
            Thread t = new Thread(new ThreadStart(checkRouterTable));
            t.Start();
            CreateHostBuilder(args).Build().Run();


        }
        public static string sendMsg(string args)
        {
            GC.Collect();
            try
            {
                if (!client.IsConnected) {
                    Console.WriteLine($"{DateTime.Now} Reconnecting to Router...");
                    client.Connect();
                }
                return client.RunCommand(args).Execute(); 
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static void checkRouterTable()
        {
            var cmdResult = "";
            while (true)
            {
                try
                {
                    if (!client.IsConnected)
                    {
                        Console.WriteLine($"{DateTime.Now} Reconnecting to Router...");
                        client.Connect();
                    }

                    cmdResult = client.RunCommand("ip route show table campus_net").Execute();
                    if (!cmdResult.Contains("192.168.72.1")) addRouteRule();
                }
                finally
                {
                    Thread.Sleep(1000);
                }
#if DEBUG
                Console.WriteLine(cmdResult); 
#endif
                Thread.Sleep(1000);
#if DEBUG
                Console.WriteLine("checkRouterTable Methode Executed. Code2");
#endif
            }
        }

        public static void addRouteRule()
        {
            try
            {
                if (!client.IsConnected)
                {
                    Console.WriteLine($"{DateTime.Now} Reconnecting to Router...");
                    client.Connect();
                }
                client.RunCommand("ip route add 192.168.255.0/24 via 0.0.0.0 dev br-lan table campus_net && ip route add default via 192.168.72.1 dev br-hlu_net table campus_net").Execute();
            }
            finally { }
        }
       

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://0.0.0.0:10086/");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
