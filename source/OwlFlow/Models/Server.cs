using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OwlFlow.Models
{
    public class Server
    {
        [Required(ErrorMessage = "Id (guid) model invalid")]
        public Guid Id { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Server name is empty or exceeds 20 characters")]
        public string? Name { get; set; }
        public bool IsConnected { get; set; }
        //public IPEndPoint iPEndPoint { get; set; }
        [Required]
        [RegularExpression(@"^(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
                    ErrorMessage = "Invalid IPv4 address format")]
        public string? IPAddress { get; set; }

        [Range(0, 100)]
        [JsonPropertyName("useCPU")]
        public int? UseCPU { get; set; }

        [Range(0, 100)]
        [JsonPropertyName("useMemory")]
        public int? UseMemory { get; set; }

        [JsonPropertyName("countClient")]
        public int? CountClient { get; set; }
        public int? Ping { get; set; }

        [JsonPropertyName("maxCapacityPeoples")]
        public int? MaxCapacityClient { get; set; }

        [JsonPropertyName("countAllRequestUser")]
        public int? CountAllRequestUser { get; set; }

        [JsonPropertyName("overloadingPermission")]
        public bool OverloadingPermission { get; set; }

        [BindNever]
        [Url(ErrorMessage = "Invalid URL")]
        public string? URI { get; set; }
        public void ResetProperty()
        {
            this.IsConnected = false;
            this.UseCPU = 0;
            this.UseMemory = 0;
            this.CountClient = 0;
            this.Ping = 0;
            this.MaxCapacityClient = 0;
            this.CountAllRequestUser = 0;
        }
    }
}