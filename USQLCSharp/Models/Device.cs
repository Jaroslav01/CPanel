using System.Collections.Generic;

namespace USQLCSharp.Models
{
    public class Device
    {
        public int Id { get; set; }

        public int Uptime { get; set; }

        public int Rssi { get; set; }

        public string Name { get; set; }

        public string Topic { get; set; }

        public string Ip { get; set; }

        public string Mac { get; set; }

        public bool State { get; set; }

        public List<Parameter> Parameters { get; set; } = new List<Parameter>();
    }
}
