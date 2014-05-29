using FreeWheeling.Domain.Entities;
using FreeWheeling.UI.DataContexts;
using Postal;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

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

        public void SendUsersPrivateBunchInviteEmail(List<PrivateGroupUsers> _PrivateGroupUsers,
            int GroupId,
            string createdby,
            string bunchName)
        {

            foreach (PrivateGroupUsers item in _PrivateGroupUsers)
            {
                if (item.UserId != null)
                {
                    dynamic emailToUser = new Email("SendUsersPrivateBunchInviteEmail");
                    emailToUser.To = item.Email;
                    emailToUser.UserName = GetUserNameViaEmail(item.Email);
                    emailToUser.creator = createdby;
                    emailToUser.bunchName = bunchName;
                    emailToUser.link = "http://www.bunchy.com.au/Ride?groupId=" + GroupId;
                    emailToUser.Send();
                }
                else
                {

                    NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

                    queryString["groupid"] = GroupId.ToString();
                    queryString["InviteId"] = item.id.ToString();

                    dynamic emailToUser = new Email("SendUsersPrivateBunchInviteEmail");
                    emailToUser.To = item.Email;
                    emailToUser.UserName = GetUserNameViaEmail(item.Email);
                    emailToUser.creator = createdby;
                    emailToUser.bunchName = bunchName;
                    emailToUser.link ="http://www.bunchy.com.au/Ride?" +  queryString.ToString(); 
                    emailToUser.Send();
                }
            }
        }

        public void SendFeedBack(string Name, string Message)
        {
            dynamic emailToUser = new Email("SendFeedback");
            emailToUser.Name = Name;
            emailToUser.Message = Message;
            emailToUser.Send();
        }

        public void SendUsersPrivateAdHocBunchInviteEmail(
           List<PrivateRandomUsers> _PrivateRandomUsers,
           int RideId,
           string createdby,
           string bunchDate,
           string bunchName)
        {
            foreach (PrivateRandomUsers item in _PrivateRandomUsers)
            {
                if (item.UserId != null)
                {
                    dynamic emailToUser = new Email("SendUsersPrivateAdHocBunchInviteEmail");
                    emailToUser.To = item.Email;
                    emailToUser.UserName = GetUserNameViaEmail(item.Email);
                    emailToUser.creator = createdby;
                    emailToUser.BunchName = bunchName;
                    emailToUser.bunchDate = bunchDate;
                    emailToUser.link = "http://www.bunchy.com.au/Ride/ViewAdHocRide?adhocrideid=" + RideId;
                    emailToUser.Send();
                }
                else
                {
                    NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

                    queryString["adhocrideid"] = RideId.ToString();
                    queryString["InviteRandomId"] = item.id.ToString();

                    dynamic emailToUser = new Email("SendUsersPrivateAdHocBunchInviteEmail");
                    emailToUser.To = item.Email;
                    emailToUser.UserName = GetUserNameViaEmail(item.Email);
                    emailToUser.creator = createdby;
                    emailToUser.BunchName = bunchName;
                    emailToUser.bunchDate = bunchDate;
                    emailToUser.link = "http://www.bunchy.com.au/Ride/ViewAdHocRide?" + queryString.ToString();
                    emailToUser.Send();
                }
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

        public bool IsValidUserName(string UserName)
        {
            int userNameCount = idb.Users.Where(u => u.UserName == UserName).Count();
            if (userNameCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetEmailViaUserName(string UserName)
        {
            return idb.Users.Where(e => e.UserName == UserName).Select(f => f.Email).FirstOrDefault();
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

    public class HandleAntiForgeryError : ActionFilterAttribute, IExceptionFilter
    {
        #region IExceptionFilter Members

        public void OnException(ExceptionContext filterContext)
        {
            var exception = filterContext.Exception as HttpAntiForgeryException;
            if (exception != null)
            {
                var routeValues = new RouteValueDictionary();
                routeValues["controller"] = "Account";
                routeValues["action"] = "Login";
                filterContext.Result = new RedirectToRouteResult(routeValues);
                filterContext.ExceptionHandled = true;
            }
        }

        #endregion
    }

}