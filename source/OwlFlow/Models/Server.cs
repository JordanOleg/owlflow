using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text.Json.Serialization;
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
        [RegularExpression(@"^(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.
                (?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$")]
        public string IPAddress { get; set; }

        [Range(0, 100)]
        [JsonPropertyName("useCPU")]
        public int UseCPU { get; set; }

        [Range(0, 100)]
        [JsonPropertyName("useMemory")]
        public int UseMemory { get; set; }

        [JsonPropertyName("countClient")]
        public int CountClient { get; set; }
        public int Ping { get; set; }

        [JsonPropertyName("maxCapacityPeoples")]
        public int MaxCapacityPeoples { get; set; }

        [JsonPropertyName("countAllRequestUser")]
        public int CountAllRequestUser { get; set; }

        [JsonPropertyName("overloadingPermission")]
        public bool OverloadingPermission { get; set; }

        [Url]
        public Uri URI { get; set; }
        public void ResetProperty()
        {
            this.IsConnected = false;
            this.UseCPU = 0;
            this.UseMemory = 0;
            this.CountClient = 0;
            this.Ping = 0;
            this.MaxCapacityPeoples = 0;
            this.CountAllRequestUser = 0;
        }
    }
}