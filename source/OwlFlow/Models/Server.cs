using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace OwlFlow.Models
{
    public class Server
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsConnected { get; set; }
        //public IPEndPoint iPEndPoint { get; set; }
        public string IPAddress { get; set; }
        public int UseCPU { get; set; }
        public int UseMemory { get; set; }
        public int CountClient { get; set; }
        public int Ping { get; set; }
        public int MaxCapacityPeoples { get; set; }
        public int CountAllRequestUser { get; set; }
        public bool OverloadingPermission { get; set; }
    }
}