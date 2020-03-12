using System;
using System.Collections.Generic;

namespace FoodExApi.Models
{
    public partial class Account
    {
        public int UserId { get; set; }
        public string UserPassword { get; set; }

        public virtual AppUser AppUser { get; set; }
    }
}
