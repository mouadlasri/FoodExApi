using System;
using System.Collections.Generic;

namespace FoodExApi.Models
{
    public partial class AppOrder
    {
        public int OrderId { get; set; }
        public int? RestaurantId { get; set; }
        public string CommentText { get; set; }
        public bool? Cancelled { get; set; }
        public short? OrderStatus { get; set; }
        public DateTime? DateCompleted { get; set; }

        public virtual Restaurant Restaurant { get; set; }
        public virtual MakeOrder MakeOrder { get; set; }
        public virtual OrderDetails OrderDetails { get; set; }
    }
}
