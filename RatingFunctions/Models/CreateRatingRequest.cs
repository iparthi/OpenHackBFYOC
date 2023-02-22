using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RatingFunctions.Models
{
    public class CreateRatingRequest
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("productId")]
        public string ProductId { get; set; }

        [JsonProperty("locationName")]
        public string LocationName { get; set; }

        [JsonProperty("rating")]
        public int Rating { get; set; }

        [JsonProperty("userNotes")]
        public string UserNotes { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
    }

    
}
