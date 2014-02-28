using FreeWheeling.UI.DataContexts;
using Postal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FreeWheeling.UI.Infrastructure
{
    [Authorize]
    public class UserHelper
    {
        private IdentityDb idb = new IdentityDb(); 

        public List<string> GetEmailsForUserNames(List<string> UserNames)
        {
            List<string> UserEmails = new List<string>();

            foreach (string item in UserNames)
            {
                string UserEmail = idb.Users.Where(e => e.UserName == item).Select(o => o.Email).FirstOrDefault();

                if (UserEmail != null)
                {
                    UserEmails.Add(UserEmail);      
                }
                else
                {
                    UserEmails.Add(item);
                }
            }
            return UserEmails;
        }

        public void SendUsersAdHocEmail(List<string> Emails, int AdhocID, string createdby)
        {
            foreach (string email in Emails)
            {
                dynamic emailToUser = new Email("SendAdHocEmails");
                emailToUser.To = email;
                emailToUser.UserName = GetUserNameViaEmail(email);
                emailToUser.creator = GetUserNameViaUserId(createdby);
                emailToUser.link = "http://localhost:6049/Ride/ViewAdHocRide?adhocrideid=" + AdhocID;
                emailToUser.Send();       
            }
        }

        public string GetUserNameViaEmail(string email)
        {
            return idb.Users.Where(e => e.Email == email).Select(f => f.UserName).FirstOrDefault() ?? "New User";
        }

        public string GetUserNameViaUserId(string Userid)
        {
            return idb.Users.Where(e => e.Id == Userid).Select(f => f.UserName).FirstOrDefault();
        }
    }
}