using Microsoft.AspNet.Identity.EntityFramework;

namespace FreeWheeling.UI.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public int? LocationID {get;set;}
        public string Email { get; set; }
        public bool ReceiveEmails { get; set; }
        public bool ReceiveKeen { get; set; }
        public bool ReceiveComments { get; set; }
        public bool ReceiveSummary { get; set; }
        public string TimeBefore { get; set; }
    }

}