using FreeWheeling.UI.DataContexts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System.Data.Entity;
using Postal;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FreeWheeling.UI.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public int? LocationID {get;set;}
        public bool ReceiveEmails { get; set; }
        public bool ReceiveMobileNotifications { get; set; }
        public bool ReceiveMobileFollowingNotifications { get; set; }
        public bool ReceiveMobileKeenNotifications { get; set; }
        public bool ReceiveMobileGroupNotifications { get; set; }
        public bool ReceiveKeen { get; set; }
        public bool ReceiveComments { get; set; }
        public bool ReceiveSummary { get; set; }
        public string TimeBefore { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        static ApplicationDbContext()
        {
            // Set the database intializer which is run once during application start
            // This seeds the database with admin user credentials and admin role
            Database.SetInitializer<ApplicationDbContext>(null);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    public class EmailServiceUser : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            dynamic emailToUser = new Email("SendUserVerify");
            emailToUser.To = message.Destination;
            emailToUser.Body = message.Body;
            emailToUser.Subject = message.Subject;
            emailToUser.Send();
            return emailToUser.Send();
        }
    }
}