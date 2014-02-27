using FreeWheeling.UI.DataContexts;
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

            }


            return UserEmails;

        }

        public void SendUsersAdHocEmail(List<string> Emails)
        {

            foreach (string email in Emails)
            {
                
            }


        }

    }
}