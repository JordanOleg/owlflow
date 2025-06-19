using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OwlFlow.Models
{
    public class RemoteDataServer
    {
        [Range(0, 100)]
        [JsonPropertyName("useCPU")]
        public int? UseCPU { get; set; }

        [Range(0, 100)]
        [JsonPropertyName("useMemory")]
        public int? UseMemory { get; set; }

        [JsonPropertyName("countClient")]
        public int? CountClient { get; set; }

        [JsonPropertyName("maxCapacityPeoples")]
        public int? MaxCapacityClient { get; set; }

        [JsonPropertyName("countAllRequestUser")]
        public int? CountAllRequestUser { get; set; }

        [JsonPropertyName("overloadingPermission")]
        public bool OverloadingPermission { get; set; }

        [BindNever]
        [Url(ErrorMessage = "Invalid URL")]
        public string? URI { get; set; }
    }
}