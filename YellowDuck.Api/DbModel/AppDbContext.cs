using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Entities.Profiles;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.DbModel.Entities.Conversations;
using YellowDuck.Api.DbModel.Entities.Notifications;
using YellowDuck.Api.DbModel.Entities.Contracts;
using YellowDuck.Api.DbModel.Entities.Ratings;
using YellowDuck.Api.DbModel.Entities.Payment;
using YellowDuck.Api.DbModel.Entities.Alerts;

namespace YellowDuck.Api.DbModel
{
    public class AppDbContext : IdentityDbContext<
            AppUser,
            IdentityRole,
            string,
            IdentityUserClaim<string>,
            IdentityUserRole<string>,
            IdentityUserLogin<string>,
            IdentityRoleClaim<string>,
            IdentityUserToken<string>>,
            IDataProtectionKeyContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserProfileRegisteringInterest> UserProfileRegisteringInterest { get; set; }
        public DbSet<Ad> Ads { get; set; }
        public DbSet<AdTranslation> AdTranslations { get; set; }
        public DbSet<AdAddress> AdAddress { get; set; }
        public DbSet<AdGalleryItem> AdGalleryItems { get; set; }
        public DbSet<AdRating> AdRatings { get; set; }
        public DbSet<AdProfessionalKitchenEquipment> AdProfessionalKitchenEquipments { get; set; }
        public DbSet<AdDayAvailability> AdDayAvailabilityWeekdays { get; set; }
        public DbSet<AdEveningAvailability> AdEveningAvailabilityWeekdays { get; set; }
        public DbSet<AdCertification> AdCertifications { get; set; }
        public DbSet<UserRating> UserRatings { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<AlertAddress> AlertAddress { get; set; }
        public DbSet<AlertProfessionalKitchenEquipment> AlertProfessionalKitchenEquipments { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ConversationParticipant> ConversationParticipants { get; set; }
        public DbSet<ConversationNotification> ConversationNotifications { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ContractFileItem> ContractFiles { get; set; }
        public DbSet<StripeAccount> StripeAccounts { get; set; }
        public DbSet<CheckoutSession> CheckoutSessions { get; set; }
        public DbSet<Payout> Payouts { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            Configure<AppUser>(_ =>
            {
                _.HasOne(x => x.Profile)
                    .WithOne(x => x.User)
                    .HasForeignKey<UserProfile>(x => x.UserId);

                _.HasMany(x => x.UserRatings).WithOne().HasForeignKey(x => x.UserId);

                _.HasMany(x => x.Ads).WithOne().HasForeignKey(x => x.UserId);

                _.HasMany(x => x.Alerts).WithOne().HasForeignKey(x => x.UserId);

                _.HasMany(x => x.OwnerContracts).WithOne().HasForeignKey(x => x.OwnerId);

                _.HasMany(x => x.TenantContracts).WithOne().HasForeignKey(x => x.TenantId);

                _.HasOne(x => x.StripeAccount)
                    .WithOne(x => x.User)
                    .HasForeignKey<StripeAccount>(x => x.UserId);

                _.Property(x => x.Id).ValueGeneratedOnAdd();
            });

            Configure<UserProfile>(_ => {
                _.HasOne(x => x.User).WithOne(x => x.Profile).HasForeignKey<UserProfile>(x => x.UserId);
                _.HasMany(x => x.RegisteringInterests).WithOne().HasForeignKey(x => x.UserProfileId);
            });

            Configure<UserProfileRegisteringInterest>(_ => {
                _.HasOne(x => x.UserProfile).WithMany(x => x.RegisteringInterests).HasForeignKey(x => x.UserProfileId);
            });

            Configure<Ad>(_ => {
                _.HasMany(x => x.Gallery).WithOne().HasForeignKey(x => x.AdId);
                _.HasMany(x => x.Translations).WithOne(x => x.Ad).HasForeignKey(x => x.AdId);
                _.HasMany(x => x.AdRatings).WithOne(x => x.Ad).HasForeignKey(x => x.AdId);
                _.HasMany(x => x.ProfessionalKitchenEquipments).WithOne().HasForeignKey(x => x.AdId);
                _.HasMany(x => x.DayAvailability).WithOne().HasForeignKey(x => x.AdId);
                _.HasMany(x => x.EveningAvailability).WithOne().HasForeignKey(x => x.AdId);
                _.HasMany(x => x.Certifications).WithOne().HasForeignKey(x => x.AdId);
                _.HasOne(x => x.User).WithMany(x => x.Ads).HasForeignKey(x => x.UserId);
            });

            Configure<Contract>(_ => {
                _.HasOne(x => x.Conversation)
                    .WithOne(x => x.Contract)
                    .HasForeignKey<Conversation>(x => x.ContractId);

                _.HasOne(x => x.CheckoutSession)
                    .WithOne(x => x.Contract)
                    .HasForeignKey<CheckoutSession>(x => x.ContractId);

                _.HasOne(x => x.Payout)
                    .WithOne(x => x.Contract)
                    .HasForeignKey<Payout>(x => x.ContractId);

                _.HasMany(x => x.Files).WithOne().HasForeignKey(x => x.ContractId);

                _.HasOne(x => x.AdRating)
                    .WithOne(x => x.Contract)
                    .HasForeignKey<AdRating>(x => x.ContractId);

                _.HasOne(x => x.Owner)
                    .WithMany(x => x.OwnerContracts)
                    .HasForeignKey(x => x.OwnerId);

                _.HasOne(x => x.Tenant)
                    .WithMany(x => x.TenantContracts)
                    .HasForeignKey(x => x.TenantId);

                _.HasMany(x => x.UserRatings).WithOne().HasForeignKey(x => x.ContractId);
            });

            Configure<AdAddress>(_ => {
                _.HasIndex(x => new { x.Id, x.Latitude, x.Longitude }).IsUnique();
            });

            Configure<AdTranslation>(_ => {
                _.HasIndex(x => new { x.AdId, x.Language }).IsUnique();
            });

            Configure<AdProfessionalKitchenEquipment>(_ => {
                _.HasOne(x => x.Ad).WithMany(x => x.ProfessionalKitchenEquipments).HasForeignKey(x => x.AdId);
            });

            Configure<AdDayAvailability>(_ => {
                _.HasOne(x => x.Ad).WithMany(x => x.DayAvailability).HasForeignKey(x => x.AdId);
            });

            Configure<AdEveningAvailability>(_ => {
                _.HasOne(x => x.Ad).WithMany(x => x.EveningAvailability).HasForeignKey(x => x.AdId);
            });

            Configure<AdCertification>(_ => {
                _.HasOne(x => x.Ad).WithMany(x => x.Certifications).HasForeignKey(x => x.AdId);
            });

            Configure<Alert>(_ => {
                _.HasOne(x => x.User).WithMany(x => x.Alerts).HasForeignKey(x => x.UserId);
            });

            Configure<AlertAddress>(_ => {
                _.HasIndex(x => new { x.Id, x.Latitude, x.Longitude }).IsUnique();
            });

            Configure<Conversation>(_ => {
                _.HasMany(x => x.Participants).WithOne().HasForeignKey(x => x.ConversationId);
            });

            Configure<ConversationParticipant>(_ => {
                _.HasIndex(x => new { x.ConversationId, x.Sid }).IsUnique();
            });

            Configure<AdRating>(_ => {
                _.HasOne(x => x.Ad).WithMany(x => x.AdRatings).HasForeignKey(x => x.AdId);
                _.HasOne(x => x.Contract).WithOne(x => x.AdRating).HasForeignKey<AdRating>(x => x.ContractId);
                _.HasIndex(x => x.AdId);
            });

            Configure<UserRating>(_ => {
                _.HasOne(x => x.User).WithMany(x => x.UserRatings).HasForeignKey(x => x.UserId);
                _.HasOne(x => x.Contract).WithMany(x => x.UserRatings).HasForeignKey(x => x.ContractId);
                _.HasIndex(x => x.UserId);
            });

            void Configure<TEntity>(Action<EntityTypeBuilder<TEntity>> action) where TEntity : class => action(builder.Entity<TEntity>());
        }

        public void RejectChanges(bool detachAll)
        {
            // https://stackoverflow.com/a/22098063
            foreach (var entry in ChangeTracker.Entries().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified; //Revert changes made to deleted entity.
                        entry.State = detachAll ? EntityState.Detached : EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    default:
                        if (detachAll) entry.State = EntityState.Detached;
                        break;
                }
            }
        }
    }
}
