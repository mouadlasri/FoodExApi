using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodExApi.Models
{
    public class ConfirmOrder
    {
        public List<CartItem> ItemsList { get; set; }
        public string UserId { get; set; }
        public string RestaurantId { get; set; }

    }
}
