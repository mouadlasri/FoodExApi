using System;
using System.Collections.Generic;

namespace FoodExApi.Models
{
    public partial class AppUser
    {
        public AppUser()
        {
            DiscountsDetails = new HashSet<DiscountsDetails>();
            MakeOrder = new HashSet<MakeOrder>();
            Ratings = new HashSet<Ratings>();
        }

        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public byte[] ProfilePicture { get; set; }
        public DateTime? DateJoined { get; set; }
        public int? Points { get; set; }

        public virtual Account User { get; set; }
        public virtual ICollection<DiscountsDetails> DiscountsDetails { get; set; }
        public virtual ICollection<MakeOrder> MakeOrder { get; set; }
        public virtual ICollection<Ratings> Ratings { get; set; }
    }
}
