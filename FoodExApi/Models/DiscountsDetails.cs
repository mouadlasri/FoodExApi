using System;
using System.Collections.Generic;

namespace FoodExApi.Models
{
    public partial class DiscountsDetails
    {
        public int UserId { get; set; }
        public int RestaurantId { get; set; }
        public string DiscountType { get; set; }
        public int? OriginalQuantity { get; set; }
        public int? QuantityLeft { get; set; }
        public DateTime? DateRedeemed { get; set; }
        public DateTime? DateExpiry { get; set; }
        public int? Points { get; set; }

        public virtual Restaurant Restaurant { get; set; }
        public virtual AppUser User { get; set; }
    }
}
