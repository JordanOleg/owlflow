using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace OwlFlow.Model
{
    public class Server
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsConnected { get; set; }
        //public IPEndPoint iPEndPoint { get; set; }
        public IPAddress iPAddress { get; set; }
        public int UseCPU { get; set; }
        public int UseMemory { get; set; }
        public int CountClient { get; set; }
    }
}