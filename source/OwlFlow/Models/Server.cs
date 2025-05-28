using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace OwlFlow.Models
{
    public class Server
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }
        public bool IsConnected { get; set; }
        //public IPEndPoint iPEndPoint { get; set; }
        [RegularExpression("^(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$")]
        public string IPAddress { get; set; }
        [Range(0, 100)]
        public int UseCPU { get; set; }
        [Range(0, 100)]
        public int UseMemory { get; set; }
        public int CountClient { get; set; }
        public int Ping { get; set; }
        public int MaxCapacityPeoples { get; set; }
        public int CountAllRequestUser { get; set; }
        public bool OverloadingPermission { get; set; }

        [Url]
        public Uri URI { get; set; }
    }
}