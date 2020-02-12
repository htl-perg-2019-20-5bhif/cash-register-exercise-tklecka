using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace cash_register.Data
{
    public class Product
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }
        [JsonPropertyName("name")]
        [Required]
        public string Name { get; set; }
        [JsonPropertyName("price")]
        [Required]
        public decimal Price { get; set; }

    }
}
