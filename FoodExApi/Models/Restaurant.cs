using System;
using System.Collections.Generic;

namespace FoodExApi.Models
{
    public partial class Restaurant
    {
        public Restaurant()
        {
            AppOrder = new HashSet<AppOrder>();
            DiscountsDetails = new HashSet<DiscountsDetails>();
            Item = new HashSet<Item>();
            Ratings = new HashSet<Ratings>();
        }

        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantLocation { get; set; }
        public string WaitingTime { get; set; }

        public virtual ICollection<AppOrder> AppOrder { get; set; }
        public virtual ICollection<DiscountsDetails> DiscountsDetails { get; set; }
        public virtual ICollection<Item> Item { get; set; }
        public virtual ICollection<Ratings> Ratings { get; set; }
    }
}
