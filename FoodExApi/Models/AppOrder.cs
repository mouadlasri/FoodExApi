using System;
using System.Collections.Generic;

namespace FoodExApi.Models
{
    public partial class AppOrder
    {
        public AppOrder()
        {
            OrderDetails = new HashSet<OrderDetails>();
        }

        public int OrderId { get; set; }
        public int? RestaurantId { get; set; }
        public string CommentText { get; set; }
        public bool? Cancelled { get; set; }
        public short? OrderStatus { get; set; }
        public DateTime? DateCompleted { get; set; }

        public virtual MakeOrder Order { get; set; }
        public virtual Restaurant Restaurant { get; set; }
        public virtual ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
