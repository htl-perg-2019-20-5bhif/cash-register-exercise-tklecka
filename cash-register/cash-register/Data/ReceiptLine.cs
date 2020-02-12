using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace cash_register.Data
{
    public class ReceiptLine
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }
        [JsonPropertyName("product")]
        public Product Product { get; set; }
        [JsonPropertyName("pieces")]
        [Required]
        public int Pieces { get; set; }
        [JsonPropertyName("totalPrice")]
        [Required]
        public decimal TotalPrice { get; set; }
    }
}

public class ReceiptLineDto
{
    public int ProductID { get; set; }
    public int Amount { get; set; }
}
