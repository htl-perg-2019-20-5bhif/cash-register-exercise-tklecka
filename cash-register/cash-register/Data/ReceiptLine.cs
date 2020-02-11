using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace cash_register.Data
{
    public class ReceiptLine
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }
        [JsonPropertyName("productID")]
        [Required]
        public int ProductID { get; set; }
        [JsonPropertyName("product")]
        public Product Product { get; set; }
        [JsonPropertyName("pieces")]
        [Required]
        public int Pieces { get; set; }
        [JsonPropertyName("totalPrice")]
        public float TotalPrice { get; set; }
    }
}
