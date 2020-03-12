using System;
using System.Collections.Generic;

namespace FoodExApi.Models
{
    public partial class MakeOrder
    {
        public int OrderId { get; set; }
        public int? UserId { get; set; }
        public DateTime? DateOrdered { get; set; }

        public virtual AppOrder Order { get; set; }
        public virtual AppUser User { get; set; }
    }
}
