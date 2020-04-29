using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FoodExApi.Models
{
    public partial class FoodExContext : DbContext
    {
        public FoodExContext()
        {
        }

        public FoodExContext(DbContextOptions<FoodExContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<AppOrder> AppOrder { get; set; }
        public virtual DbSet<AppUser> AppUser { get; set; }
        public virtual DbSet<DiscountsDetails> DiscountsDetails { get; set; }
        public virtual DbSet<Item> Item { get; set; }
        public virtual DbSet<ItemCategory> ItemCategory { get; set; }
        public virtual DbSet<MakeOrder> MakeOrder { get; set; }
        public virtual DbSet<OrderDetails> OrderDetails { get; set; }
        public virtual DbSet<Ratings> Ratings { get; set; }
        public virtual DbSet<Restaurant> Restaurant { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=DESKTOP-RMD70U0\\MOUADSQL;Database=FoodEx;User ID=lasrim;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.1-servicing-10028");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("pk_account");

                entity.ToTable("account");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.UserPassword)
                    .IsRequired()
                    .HasColumnName("user_password")
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<AppOrder>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("pk_order");

                entity.ToTable("app_order");

                entity.Property(e => e.OrderId)
                    .HasColumnName("order_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Cancelled)
                    .HasColumnName("cancelled")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.CommentText)
                    .HasColumnName("comment_text")
                    .HasColumnType("text");

                entity.Property(e => e.DateCompleted)
                    .HasColumnName("date_completed")
                    .HasColumnType("datetime");

                entity.Property(e => e.DatePickup)
                    .HasColumnName("date_pickup")
                    .HasColumnType("datetime");

                entity.Property(e => e.OrderStatus).HasColumnName("order_status");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.HasOne(d => d.Order)
                    .WithOne(p => p.AppOrder)
                    .HasForeignKey<AppOrder>(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_app_order_order_id");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.AppOrder)
                    .HasForeignKey(d => d.RestaurantId)
                    .HasConstraintName("fk_order");
            });

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("pk_app_user");

                entity.ToTable("app_user");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.DateJoined)
                    .HasColumnName("date_joined")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EmailAddress)
                    .HasColumnName("email_address")
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasColumnName("first_name")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasColumnName("last_name")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .HasColumnName("phone_number")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Points)
                    .HasColumnName("points")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ProfilePicture).HasColumnName("profile_picture");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.AppUser)
                    .HasForeignKey<AppUser>(d => d.UserId)
                    .HasConstraintName("fk_app_user_credential_id");
            });

            modelBuilder.Entity<DiscountsDetails>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RestaurantId })
                    .HasName("pk_discounts_details");

                entity.ToTable("discounts_details");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.Property(e => e.DateExpiry)
                    .HasColumnName("date_expiry")
                    .HasColumnType("datetime");

                entity.Property(e => e.DateRedeemed)
                    .HasColumnName("date_redeemed")
                    .HasColumnType("datetime");

                entity.Property(e => e.DiscountType)
                    .HasColumnName("discount_type")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.OriginalQuantity).HasColumnName("original_quantity");

                entity.Property(e => e.Points)
                    .HasColumnName("points")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.QuantityLeft).HasColumnName("quantity_left");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.DiscountsDetails)
                    .HasForeignKey(d => d.RestaurantId)
                    .HasConstraintName("fk_discounts_details_restaurant_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DiscountsDetails)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_discounts_details_user_id");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.ToTable("item");

                entity.Property(e => e.ItemId).HasColumnName("item_id");

                entity.Property(e => e.Category).HasColumnName("category");

                entity.Property(e => e.ItemDescription)
                    .HasColumnName("item_description")
                    .IsUnicode(false);

                entity.Property(e => e.ItemImage)
                    .HasColumnName("item_image")
                    .IsUnicode(false);

                entity.Property(e => e.ItemStatus)
                    .HasColumnName("item_status")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("decimal(4, 2)");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.Property(e => e.WaitingTime).HasColumnName("waiting_time");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.Item)
                    .HasForeignKey(d => d.RestaurantId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_item_restaurant_id");
            });

            modelBuilder.Entity<ItemCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("pk_category_id");

                entity.ToTable("item_category");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.CategoryName)
                    .HasColumnName("category_name")
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<MakeOrder>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("pk_make_order");

                entity.ToTable("make_order");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.DateOrdered)
                    .HasColumnName("date_ordered")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MakeOrder)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_make_order");
            });

            modelBuilder.Entity<OrderDetails>(entity =>
            {
                entity.ToTable("order_details");

                entity.Property(e => e.OrderDetailsId).HasColumnName("order_details_id");

                entity.Property(e => e.ItemId).HasColumnName("item_id");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_order_details_item_id");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("fk_order_details_id");
            });

            modelBuilder.Entity<Ratings>(entity =>
            {
                entity.HasKey(e => e.RatingId)
                    .HasName("pk_ratings");

                entity.ToTable("ratings");

                entity.Property(e => e.RatingId).HasColumnName("rating_id");

                entity.Property(e => e.Rating).HasColumnName("rating");

                entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.RestaurantId)
                    .HasConstraintName("fk_ratings_restaurant_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_ratings_user_id");
            });

            modelBuilder.Entity<Restaurant>(entity =>
            {
                entity.ToTable("restaurant");

                entity.Property(e => e.RestaurantId)
                    .HasColumnName("restaurant_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.ImageLink)
                    .HasColumnName("image_link")
                    .IsUnicode(false);

                entity.Property(e => e.RestaurantDescription)
                    .HasColumnName("restaurant_description")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RestaurantLocation)
                    .HasColumnName("restaurant_location")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RestaurantName)
                    .HasColumnName("restaurant_name")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.WaitingTime)
                    .HasColumnName("waiting_time")
                    .HasMaxLength(15)
                    .IsUnicode(false);
            });
        }
    }
}
