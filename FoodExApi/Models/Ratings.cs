using System;
using System.Collections.Generic;

namespace FoodExApi.Models
{
    public partial class Ratings
    {
        public int RatingId { get; set; }
        public int? UserId { get; set; }
        public int? RestaurantId { get; set; }
        public int? Rating { get; set; }

        public virtual Restaurant Restaurant { get; set; }
        public virtual AppUser User { get; set; }
    }
}
