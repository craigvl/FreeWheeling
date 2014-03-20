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

        public void SendUsersBunchInviteEmail(List<string> Emails,
            int RideId,
            string createdby,
            string bunchDate,
            string bunchName)
        {
            foreach (string email in Emails)
            {
                dynamic emailToUser = new Email("SendUsersBunchInviteEmail");
                emailToUser.To = email;
                emailToUser.UserName = GetUserNameViaEmail(email);
                emailToUser.creator = createdby;
                emailToUser.bunchDate = bunchDate;
                emailToUser.bunchName = bunchName;
                emailToUser.link = "http://www.bunchy.com.au/Ride/ViewSingleRide?RideId=" + RideId;
                emailToUser.Send();
            }
        }

        public void SendUsersAdHocBunchInviteEmail(List<string> Emails,
           int RideId,
           string createdby,
           string bunchDate,
           string bunchName)
        {
            foreach (string email in Emails)
            {
                dynamic emailToUser = new Email("SendUsersAdHocBunchInviteEmail");
                emailToUser.To = email;
                emailToUser.UserName = GetUserNameViaEmail(email);
                emailToUser.creator = createdby;
                emailToUser.BunchName = bunchName;
                emailToUser.bunchDate = bunchDate;
                emailToUser.link = "http://www.bunchy.com.au/Ride/ViewAdHocRide?adhocrideid=" + RideId;
                emailToUser.Send();
            }
        }

        public void SendUsersCreateAdHocEmail(List<string> Emails, int AdhocID, string createdby, string bunchName)
        {
            foreach (string email in Emails)
            {
                dynamic emailToUser = new Email("SendAdHocEmails");
                emailToUser.To = email;
                emailToUser.UserName = GetUserNameViaEmail(email);
                emailToUser.BunchName = bunchName;
                emailToUser.creator = GetUserNameViaUserId(createdby);
                emailToUser.link = "http://www.bunchy.com.au/Ride/ViewAdHocRide?adhocrideid=" + AdhocID;
                emailToUser.Send();       
            }
        }

        public void SendUsersDeleteAdHocEmail(List<string> Emails, string AdhocName, string createdbyName)
        {
            foreach (string email in Emails)
            {
                dynamic emailToUser = new Email("SendAdHocEmailsDelete");
                emailToUser.To = email;
                emailToUser.UserName = GetUserNameViaEmail(email);
                emailToUser.creator = createdbyName;
                emailToUser.AdHocName = AdhocName;
                emailToUser.Send();
            }
        }

        public void SendUsersNewCommentAdHocEmail(List<string> Emails, string AdhocName, string UserName, string comment, int AdhocID)
        {
            foreach (string email in Emails)
            {
                dynamic emailToUser = new Email("SendAdHocNewCommentEmails");
                emailToUser.To = email;
                emailToUser.UserName = UserName;
                emailToUser.Comment = comment;
                emailToUser.AdHocName = AdhocName;
                emailToUser.link = "http://www.bunchy.com.au/Ride/ViewAdHocRide?adhocrideid=" + AdhocID;
                emailToUser.Send();
            }
        }

        public void SendUsersNewCommentRideEmail(List<string> Emails, string GroupName, string UserName, string comment, int GroupID, DateTime RideDate)
        {
            foreach (string email in Emails)
            {
                dynamic emailToUser = new Email("SendGroupRideNewCommentEmails");
                emailToUser.To = email;
                emailToUser.UserName = UserName;
                emailToUser.Comment = comment;
                emailToUser.GroupName = GroupName;
                emailToUser.link = "http://www.bunchy.com.au/Ride?groupid=" + GroupID;
                emailToUser.RideDate = RideDate.ToString("dd/MM/yyyy");
                emailToUser.Send();
            }
        }

        public void SendUsersGroupAttendStatusEmail(List<string> Emails, string GroupName, string status, string UserName, int GroupID, DateTime RideDate)
        {
            foreach (string email in Emails)
            {
                dynamic emailToUser = new Email("SendGroupRideEmailsAttend");
                emailToUser.To = email;
                emailToUser.UserName = UserName;
                emailToUser.Status = status;
                emailToUser.GroupName = GroupName;
                emailToUser.RideDate = RideDate.ToString("dd/MM/yyyy"); ;
                emailToUser.link = "http://www.bunchy.com.au/Ride?groupid=" + GroupID;
                emailToUser.Send();
            }
        }

        public void SendUsersAdHocAttendStatusEmail(List<string> Emails, string AdhocName, string UserName, string status, int AdhocID, DateTime RideDate)
        {
            foreach (string email in Emails)
            {
                dynamic emailToUser = new Email("SendAdHocEmailsAttend");
                emailToUser.To = email;
                emailToUser.UserName = UserName;
                emailToUser.Status = status;
                emailToUser.AdHocName = AdhocName;
                emailToUser.RideDate = RideDate.ToString("dd/MM/yyyy"); ;
                emailToUser.link = "http://www.bunchy.com.au/Ride/ViewAdHocRide?adhocrideid=" + AdhocID;
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

        public string GetUserEmailViaUserId(string Userid)
        {
            return idb.Users.Where(e => e.Id == Userid).Select(f => f.Email).FirstOrDefault();
        }
    }
}