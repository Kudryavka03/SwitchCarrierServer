using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Renci.SshNet;
using System;

namespace SwitchCarrierServer.Controller
{
    delegate string SyncMsg(string msg);
    [Route("/api")]
    public class switchController : ControllerBase
    {
        readonly SyncMsg sendMsg = new SyncMsg(Program.sendMsg);
        static SshClient client = new SshClient("192.168.255.1", "root", "password");
        private readonly IWebHostEnvironment _env;
        public switchController(IWebHostEnvironment env)
        {
            _env = env;
        }
        [HttpGet("switch")]
        public string Index()
        {
           // var client = Program.client;
            string result = "";
            string clientIP = HttpContext.Connection.RemoteIpAddress.ToString();
            var ipaddr = clientIP;
            if (clientIP.Contains(":")) return "0,不支持IPV6访问";
            if ((result = sendMsg($"ip rule | grep {clientIP} |grep campus_net")) == "")
            {
                if ((sendMsg($"ip rule add from {clientIP} table campus_net pref 1501")) == "")
                {
                    return $"IP：{ipaddr}  已成功切换到 校园网-移动 ！";
                }
                else return "Error while switching CMCC";
            }
            else
            {
                result = sendMsg($"ip rule del from {clientIP} table campus_net");
            }
            if (result == "") return $"IP：{ipaddr}  已成功切换到 珠江宽频-电信 ！";

            //Program.client.Disconnect();
            return "Error While switching the network:"+result;
        }
        [HttpGet("current_ip")]
        public string current_ip()
        {
            return HttpContext.Connection.RemoteIpAddress.ToString();
        }
        [HttpGet("current_carrier")]
        public string current_carrier()
        {
            var ip = HttpContext.Connection.RemoteIpAddress.ToString(); 
            return sendMsg($"ip rule | grep {ip} |grep campus_net") == ""? "珠江宽频-电信" : "校园网-移动";
        }

        [HttpGet("switch/{ipaddr}")]
        public string switch_ip(string ipaddr)
        {
            // var client = Program.client;
            string result = "";
            string clientIP = ipaddr;
            if ((result = sendMsg($"ip rule | grep {clientIP} |grep campus_net")) == "")
            {
                if ((sendMsg($"ip rule add from {clientIP} table campus_net pref 1501")) == "")
                {
                    return $"IP：{ipaddr}  已成功切换到 校园网-移动 ！";
                }
                else return "Error while switching CMCC";
            }
            else
            {
                result = sendMsg($"ip rule del from {clientIP} table campus_net");
            }
            if (result == "") return $"IP：{ipaddr}  已成功切换到 珠江宽频-电信 ！";

            //Program.client.Disconnect();
            return "Error While switching the network:" + result;
        }
    }

}
