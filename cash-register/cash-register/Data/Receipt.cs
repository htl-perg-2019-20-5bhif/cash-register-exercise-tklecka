﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace cash_register.Data
{
    public class Receipt
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }
        [JsonPropertyName("lines")]
        [MinLength(1)]
        public List<ReceiptLine> Lines { get; set; }
        [JsonPropertyName("timestamp")]
        [Required]
        public DateTime Timestamp { get; set; }
        [JsonPropertyName("totalPrice")]
        [Required]
        public decimal TotalPrice { get; set; }
    }
}
