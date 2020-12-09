using CaseHandler.WebApplication.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CaseHandler.WebApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ApplicationUser> ApplicationUsers {get; set;}
        public DbSet<Case> Cases { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<History> Histories { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasKey(a => a.Id)
                .HasName("PK_AspNetUsers");

            modelBuilder.Entity<Case>()
                .HasOne(caseModel => caseModel.Assignee)
                .WithMany(applicationUserModel => applicationUserModel.AssignedCases)
                .HasForeignKey(caseModel => caseModel.AssigneeId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Case>()
                .HasOne(caseModel => caseModel.ReportedBy)
                .WithMany(applicationUserModel => applicationUserModel.ReportedCases)
                .HasForeignKey(caseModel => caseModel.ReportedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(commentModel => commentModel.Case)
                .WithMany(caseModel => caseModel.Comments)
                .HasForeignKey(commentModel => commentModel.CaseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(commentModel => commentModel.CommentedBy)
                .WithMany(applicationUserModel => applicationUserModel.Comments)
                .HasForeignKey(commentModel => commentModel.CommentedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<History>()
                .HasOne(historyModel => historyModel.Case)
                .WithMany(caseModel => caseModel.HistoryItems)
                .HasForeignKey(historyModel => historyModel.CaseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<History>()
                .HasOne(historyModel => historyModel.CreatedBy)
                .WithMany(applicationUserModel => applicationUserModel.HistoryItems)
                .HasForeignKey(historyModel => historyModel.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(notificationModel => notificationModel.Case)
                .WithMany(caseModel => caseModel.Notifications)
                .HasForeignKey(notificationModel => notificationModel.CaseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(notificationModel => notificationModel.CreatedBy)
                .WithMany(applicationUserModel => applicationUserModel.CreatedNotifications)
                .HasForeignKey(notificationModel => notificationModel.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(notificationModel => notificationModel.Recipient)
                .WithMany(applicationUserModel => applicationUserModel.ReceiptNotifications)
                .HasForeignKey(notificationModel => notificationModel.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
