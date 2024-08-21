using CollectionManagement.Models.Domain;
using CollectionManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CollectionManagement.Data
{
    public class MVCmainDBConetxt : IdentityDbContext<User, Role, int>
    {
        public MVCmainDBConetxt(DbContextOptions options) : base(options) { }

        //public DbSet<User> Users { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ItemTag> ItemTags { get; set; }  // New DbSet for the junction table
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<ItemCustomField> ItemCustomFields { get; set; }
        public DbSet<ItemCustomFieldValue> ItemCustomFieldValues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User and Roles relationship
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // Collection and User relationship
            modelBuilder.Entity<Collection>()
                .HasOne(c => c.User)
                .WithMany(u => u.Collections)
                .HasForeignKey(c => c.UserId);

            // Item and Collection relationship
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Collection)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CollectionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Item and Custom Field relationships
            modelBuilder.Entity<ItemCustomField>()
                .HasOne(cf => cf.Collection)
                .WithMany(c => c.CustomFields)
                .HasForeignKey(cf => cf.CollectionId);

            modelBuilder.Entity<ItemCustomFieldValue>()
                .HasOne(cf => cf.Item)
                .WithMany(i => i.CustomFieldValues)
                .HasForeignKey(cf => cf.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ItemCustomFieldValue>()
                .HasOne(cf => cf.CustomField)
                .WithMany()
                .HasForeignKey(cf => cf.CustomFieldId)
                .OnDelete(DeleteBehavior.Cascade);

            // Comment and Item relationship
            modelBuilder.Entity<Comment>()
                .HasOne(co => co.Item)
                .WithMany(i => i.Comments)
                .HasForeignKey(co => co.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // Comment and User relationship
            modelBuilder.Entity<Comment>()
                .HasOne(co => co.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(co => co.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Like and Item relationship
            modelBuilder.Entity<Like>()
                .HasOne(l => l.Item)
                .WithMany(i => i.Likes)
                .HasForeignKey(l => l.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // Like and User relationship
            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Tag relationships
            modelBuilder.Entity<Tag>()
                .HasIndex(t => t.Name)
                .IsUnique();

            // Configure many-to-many relationship between Item and Tag
            modelBuilder.Entity<ItemTag>()
                .HasKey(it => new { it.ItemId, it.TagId });
            modelBuilder.Entity<ItemTag>()
                .HasOne(it => it.Item)
                .WithMany(i => i.Tags)
                .HasForeignKey(it => it.ItemId);
            modelBuilder.Entity<ItemTag>()
                .HasOne(it => it.Tag)
                .WithMany(t => t.ItemTags)
                .HasForeignKey(it => it.TagId);
        }
    }
}
