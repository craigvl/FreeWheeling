using FreeWheeling.Domain.Abstract;
using FreeWheeling.Domain.Entities;
using FreeWheeling.UI.DataContexts;
using FreeWheeling.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using FreeWheeling.UI.Filters;
using FreeWheeling.UI.Infrastructure;
using System.Globalization;
using PusherServer;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.ServiceBus.Notifications;

namespace FreeWheeling.UI.Controllers
{
    [Authorize]
    public class RideController : Controller
    {
        private IdentityDb idb = new IdentityDb();
        private ICycleRepository repository;
        private NotificationHubClient hubClient;

        public RideController(ICycleRepository repoParam)
        {
            repository = repoParam;
            var cn = "Endpoint=sb://bunchy-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=l+yxYaa2g7FcqmNAUrwX0K1Rq/dfFh2GpaYXMuuAHig=";
            hubClient = NotificationHubClient
                .CreateClientFromConnectionString(cn, "bunchy");
        }

        [Compress]
        public ActionResult Index(int groupid = -1, int rideid = -1, int InviteId = -1, string fromhome = "false")
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            if (InviteId != -1)
            {
                if (repository.PrivateBunchInviteUserEmailNotSet(InviteId))
                {
                    repository.UpdateInvitePrivateUser(currentUser.Id, currentUser.Email, InviteId);
                    repository.Save();
                }
            }

            RideModelIndex RideModel = new RideModelIndex();
            Group _Group = repository.GetGroupByID(groupid);

            if (_Group == null)
            {
                GroupModel GroupModel = new GroupModel();
                GroupModel._Groups = repository.GetGroups().ToList();
                GroupModel.CurrentGroupMembership = repository.CurrentGroupsForUser(currentUser.Id);
                return RedirectToAction("index", "group", GroupModel);
            }

            //Just in case location ID has not been set, set to same as ride.
            if (currentUser.LocationID == null)
            {
                currentUser.LocationID = _Group.Location.id;
                idb.SaveChanges();
            }

            RideModelHelper _RideHelper = new RideModelHelper(repository);
            var t = Request.QueryString["groupId"];
            if (groupid != -1 || rideid != -1)
            {
                RideModel = _RideHelper.PopulateRideModel(rideid, groupid, currentUser.Id, true);
            }
            else
            {
                GroupModel GroupModel = new GroupModel();
                GroupModel._Groups = repository.GetGroups().ToList();
                GroupModel.CurrentGroupMembership = repository.CurrentGroupsForUser(currentUser.Id);
                return RedirectToAction("index", "group", GroupModel);
            }

            if (RideModel.Ride != null)
            {
            }
            else
            {
                GroupModel GroupModel = new GroupModel();
                GroupModel._Groups = repository.GetGroups().ToList();
                GroupModel.CurrentGroupMembership = repository.CurrentGroupsForUser(currentUser.Id);
                return RedirectToAction("index", "group", GroupModel);
            }

            RideModel.FromHome = fromhome;
            RideModel.IsFavourite = repository.IsInFavouriteList(RideModel.Group.id, currentUser.Id);
            if (_Group.Lat != null)
            {
                RideModel.lat = _Group.Lat;
            }

            if (_Group.Lng != null)
            {
                RideModel.lng = _Group.Lng;
            }

            if (RideModel.Group.IsPrivate)
            {
                if (!repository.IsInvitedToPrivateBunch(RideModel.Group.id, currentUser.Id))
                {
                    GroupModel GroupModel = new GroupModel();
                    GroupModel._Groups = repository.GetGroups().ToList();
                    GroupModel.CurrentGroupMembership = repository.CurrentGroupsForUser(currentUser.Id);
                    return RedirectToAction("index", "group", GroupModel);
                }
            }

            RideModel.RouteCount = repository.RouteCountForGroup(RideModel.Group.id);

            return View(RideModel);
        }

        [HttpPost]
        public JsonResult RemoveFromFavouriteListJSON(int id)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Member _Member = repository.GetMemberByUserID(currentUser.Id);
            Group group = repository.GetGroupByID(id);
            CultureHelper _CultureHelper = new CultureHelper(repository);
            TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(currentUser.LocationID);
            repository.RemoveMember(currentUser.Id, group);
            repository.Save();

            Task T = new Task(() =>
            {
                Ride _Ride = repository.GetHomePageRideByUserID(currentUser.Id);
                if (_Ride.Group.id == id)
                {
                    repository.DeleteHomePageRide(currentUser.Id);
                }
            });

            T.Start();

            Task E = new Task(() =>
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.ConnectionStrings["AzureJobsData"].ConnectionString);

                // Create the queue client.
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

                // Retrieve a reference to a queue.
                CloudQueue queue = queueClient.GetQueueReference("updatehomepage");

                // Create the queue if it doesn't already exist.
                queue.CreateIfNotExists();

                // Create a message and add it to the queue.
                CloudQueueMessage message = new CloudQueueMessage("Hello, World");
                queue.AddMessage(message);

            });

            E.Start();

            return Json(new
            {
                success = true,
                message = "Emails Sent",
                GroupId = 2
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult JoinJSON(int id)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Member _Member = repository.GetMemberByUserID(currentUser.Id);
            Group group = repository.GetGroupByID(id);
            repository.AddMember(currentUser.Id, group);
            repository.Save();

            Task T = new Task(() =>
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.ConnectionStrings["AzureJobsData"].ConnectionString);

                // Create the queue client.
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

                // Retrieve a reference to a queue.
                CloudQueue queue = queueClient.GetQueueReference("updatehomepage");

                // Create the queue if it doesn't already exist.
                queue.CreateIfNotExists();

                // Create a message and add it to the queue.
                CloudQueueMessage message = new CloudQueueMessage("Joined");
                queue.AddMessage(message);

            });

            T.Start();
            //To Do: Need to confirm if this is used as looks incorrect.
            return Json(new
            {
                success = true,
                message = "Emails Sent",
                GroupId = 2
            }, JsonRequestBehavior.AllowGet);
        }

        [Compress]
        public ActionResult ViewSingleRide(int RideId, string fromhome = "false")
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Ride _Ride = repository.GetRideByIDIncludeGroup(RideId);
            if (_Ride == null)
            {
                GroupModel GroupModel = new GroupModel();
                GroupModel._Groups = repository.GetGroups().ToList();
                GroupModel.CurrentGroupMembership = repository.CurrentGroupsForUser(currentUser.Id);
                return RedirectToAction("index", "group", GroupModel);
            }
            else
            {
                SingleRideAndRandomRideViewModel _SingleRideRandomRideViewModel = new SingleRideAndRandomRideViewModel();
                RideModelHelper _AdHocHelper = new RideModelHelper(repository);
                _SingleRideRandomRideViewModel = _AdHocHelper.PopulateSingleRideModel(RideId, currentUser.Id);
                _SingleRideRandomRideViewModel.FromHome = fromhome;
                _SingleRideRandomRideViewModel.IsFavourite = repository.IsInFavouriteList(_Ride.Group.id, currentUser.Id);
                return View(_SingleRideRandomRideViewModel);
            }
        }

        public ActionResult SeeAllComments(int RideId, int GroupId, int PreviousID = -1)
        {
            AllRideComments _AllRideComments = new AllRideComments();
            _AllRideComments.RideId = RideId;
            _AllRideComments.GroupId = GroupId;
            _AllRideComments.Comments = repository.GetAllCommentsForRide(RideId);
            _AllRideComments.PreviousID = PreviousID;
            return View(_AllRideComments);
        }

        public ActionResult InviteOthersToBunch(int RideId, int PreviousID = -1)
        {
            Ride _Ride = repository.GetRideByIDIncludeGroup(RideId);
            InviteOthersToBunchModel _InviteOthersToBunchModel = new InviteOthersToBunchModel
            {
                RideId = _Ride.id,
                Name = _Ride.Group.name,
                RideDate = _Ride.RideDate.ToString("dd/MM/yyyy"),
                PreviousID = PreviousID
            };
            return View(_InviteOthersToBunchModel);
        }

        [HttpPost]
        public JsonResult InviteOthersToBunch(InviteOthersToBunchModel _InviteOthersToBunchModel)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Task T = new Task(() =>
            {
                Ride _Ride = repository.GetRideByIDIncludeGroup(_InviteOthersToBunchModel.RideId);
                List<string> UserNames = new List<string>();

                if (_InviteOthersToBunchModel.InviteUsers != null)
                {
                    foreach (InviteUser item in _InviteOthersToBunchModel.InviteUsers)
                    {
                        UserNames.Add(item.UserName);
                    }
                    UserHelper _UserHelp = new UserHelper();
                    _UserHelp.SendUsersBunchInviteEmail(_UserHelp.GetEmailsForUserNames(UserNames),
                    _InviteOthersToBunchModel.RideId,
                    currentUser.UserName, _Ride.RideDate.ToString("dd/MM/yyyy"), _Ride.Group.name);
                }
            });

            T.Start();
            return Json(new
            {
                success = true,
                message = "Emails Sent",
                RideId = _InviteOthersToBunchModel.RideId
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Vote(int routeid, int rideid, int ParentRideID)
        {
            Ride CurrentRide = repository.GetRideByID(rideid);
            Route CurrentRoute = repository.GetRouteById(routeid);
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            repository.AddVote(currentUser.Id, CurrentRoute, CurrentRide);
            int TotalVotes = repository.RouteVoteCountByRideid(routeid, rideid);
            return Json(new
            {
                success = true,
                message = "Vote Added",
                rideid = rideid,
                routeid = routeid,
                totalvotes = TotalVotes
                
            }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult AddComment(int groupid, int rideid, string CommentString, int ParentRideID)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());

            if (CommentString != string.Empty)
            {
                repository.AddRideComment(CommentString, rideid, currentUser.UserName, currentUser.Id);
                repository.Save();
                RideModelIndex RideModel = new RideModelIndex();
                RideModelHelper _RideHelper = new RideModelHelper(repository);
                RideModel = _RideHelper.PopulateRideModel(ParentRideID, groupid, currentUser.Id, false);
                int commentCount = repository.GetCommentCountForRide(rideid);

                Task E = new Task(() =>
                {
                    CultureHelper _CultureHelper = new CultureHelper(repository);
                    TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(currentUser.LocationID);
                    Ride _Ride = repository.GetRideByID(rideid);
                    string RideDate = _Ride.RideDate.ToShortDateString();
                    string GroupName = _Ride.Group.name;
                    UserHelper _UserHelp = new UserHelper();
                    List<Rider> _Riders = repository.GetRidersAndCommentersForRideDontIncludeCurrentUser(_Ride.id, TZone, currentUser.Id);
                    List<string> Emails = new List<string>();

                    foreach (Rider item in _Riders.GroupBy(x => x.userId).Select(x => x.FirstOrDefault()))
                    {
                        ApplicationUser ThisUser = idb.Users.Where(u => u.Id == item.userId).FirstOrDefault();
                        if (ThisUser != null)
                        {
                            if (ThisUser.ReceiveEmails)
                            {
                                string email = _UserHelp.GetUserEmailViaUserId(item.userId);
                                Emails.Add(email);
                            }
                        }
                    }

                    _UserHelp.SendUsersNewCommentRideEmail(Emails, GroupName, currentUser.UserName, CommentString, _Ride.Group.id, _Ride.RideDate);
                });

                E.Start();

                return Json(new
                {
                    success = true,
                    message = CommentString,
                    rideid = rideid,
                    username = currentUser.UserName,
                    commentcount = commentCount,
                    parentid = ParentRideID
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, Message = "Please enter a comment." }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Attend(int RideId, string Commitment, int Groupid, int ParentRideID)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Ride _Ride = new Ride();
            Group _Group = new Group();
            _Ride = repository.GetRideByID(RideId);
            _Group = repository.GetGroupByID(Groupid);
            Rider _Rider = new Rider
            {
                userId = currentUser.Id,
                Name = currentUser.UserName,
                Ride = _Ride,
                LeaveTime = DateTime.UtcNow,
                PercentKeen = Commitment
            };

            repository.AddRider(_Rider, _Group);
            repository.Save();
            RideModelIndex RideModel = new RideModelIndex();
            RideModelHelper _RideHelper = new RideModelHelper(repository);
            RideModel = _RideHelper.PopulateRideModel(ParentRideID, Groupid, currentUser.Id, false);
            string KeenCount = repository.GetKeenCountForRide(RideId).ToString();

            Task E = new Task(() =>
            {
                CultureHelper _CultureHelper = new CultureHelper(repository);
                TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(currentUser.LocationID);
                string RideDate = _Ride.RideDate.ToShortDateString();
                string GroupName = _Ride.Group.name;
                UserHelper _UserHelp = new UserHelper();
                List<Rider> _Riders = repository.GetRidersAndCommentersForRideDontIncludeCurrentUser(_Ride.id, TZone, currentUser.Id);
                List<string> Emails = new List<string>();

                foreach (Rider item in _Riders.GroupBy(x => x.userId).Select(x => x.FirstOrDefault()))
                {
                    var ThisUser = idb.Users.Find(item.userId);
                    if (ThisUser != null)
                    {
                        if (ThisUser.ReceiveEmails)
                        {
                            string email = _UserHelp.GetUserEmailViaUserId(item.userId);
                            Emails.Add(email);
                        }
                    }
                }

                foreach (string u in repository.GetFollowers(currentUser.Id))
                {
                    var ThisUser = idb.Users.Find(u);
                    if (ThisUser != null)
                    {
                        if (ThisUser.ReceiveEmails)
                        {
                            string email = _UserHelp.GetUserEmailViaUserId(u);
                            Emails.Add(email);
                        }
                    }
                }

                Emails = Emails.Distinct().ToList();

                foreach (string e in Emails)
                {
                    ApplicationUser _user = idb.Users.Where(d => d.Email == e).FirstOrDefault();
                    sendNotification(currentUser.UserName + " Is " + Commitment + " Ride " + _Group.name, _user.UserName);
                }

                _UserHelp.SendUsersGroupAttendStatusEmail(Emails, GroupName, Commitment, currentUser.UserName, _Ride.Group.id, _Ride.RideDate);
            });

            E.Start();

            Task T = new Task(() =>
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.ConnectionStrings["AzureJobsData"].ConnectionString);

                // Create the queue client.
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

                // Retrieve a reference to a queue.
                CloudQueue queue = queueClient.GetQueueReference("updatehomepage");

                // Create the queue if it doesn't already exist.
                queue.CreateIfNotExists();

                // Create a message and add it to the queue.
                CloudQueueMessage message = new CloudQueueMessage("Hello, World");
                queue.AddMessage(message);

            });

            T.Start();

            return Json(new
            {
                success = true,
                message = Commitment,
                rideid = RideId,
                username = currentUser.UserName,
                keencount = KeenCount,
                leavetime = DateTime.UtcNow,
                parentid = ParentRideID
            }, JsonRequestBehavior.AllowGet);
        }

        private async Task sendNotification(string notificationText, string tag)
        {
            try
            {
                // Create notifications for both Windows Store and iOS platforms.
                var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" +
                    notificationText + "</text></binding></visual></toast>";
                var alert = "{\"aps\":{\"alert\":\"" + notificationText +
                    "\"}, \"inAppMessage\":\"" + notificationText + "\"}";
                string gcmMessage = "{\"data\":{\"message\":\"" + notificationText + "\"}}";

                // Send a notification to the logged-in user on both platforms.
                //await hubClient.SendWindowsNativeNotificationAsync(toast, tag);
                //await hubClient.SendAppleNativeNotificationAsync(alert, tag);
                await hubClient.SendGcmNativeNotificationAsync(gcmMessage, tag);
            }
            catch (ArgumentException ex)
            {
                // This is expected when an APNS registration doesn't exist.
                Console.WriteLine(ex.Message);
            }
        }


        public JsonResult AttendSingleRide(int RideId, string Commitment, int Groupid, int ParentRideID)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Ride _Ride = new Ride();
            Group _Group = new Group();
            _Ride = repository.GetRideByID(RideId);
            _Group = repository.GetGroupByID(Groupid);

            Rider _Rider = new Rider
            {
                userId = currentUser.Id,
                Name = currentUser.UserName,
                Ride = _Ride,
                LeaveTime = DateTime.UtcNow,
                PercentKeen = Commitment
            };

            repository.AddRider(_Rider, _Group);
            repository.Save();
            SingleRideAndRandomRideViewModel _SingleRideRandomRideViewModel = new SingleRideAndRandomRideViewModel();
            RideModelHelper _RideHelper = new RideModelHelper(repository);
            _SingleRideRandomRideViewModel = _RideHelper.PopulateSingleRideModel(RideId, currentUser.Id);
            string KeenCount = repository.GetKeenCountForRide(RideId).ToString();

            Task E = new Task(() =>
            {
                CultureHelper _CultureHelper = new CultureHelper(repository);
                TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(currentUser.LocationID);
                string RideDate = _Ride.RideDate.ToShortDateString();
                string GroupName = _Ride.Group.name;
                UserHelper _UserHelp = new UserHelper();
                List<Rider> _Riders = repository.GetRidersAndCommentersForRideDontIncludeCurrentUser(_Ride.id, TZone, currentUser.Id);
                List<string> Emails = new List<string>();
                foreach (Rider item in _Riders.GroupBy(x => x.userId).Select(x => x.FirstOrDefault()))
                {
                    var ThisUser = idb.Users.Find(item.userId);
                    if (ThisUser != null)
                    {
                        if (ThisUser.ReceiveEmails)
                        {
                            string email = _UserHelp.GetUserEmailViaUserId(item.userId);
                            Emails.Add(email);
                        }
                    }
                }

                foreach (string u in repository.GetFollowers(currentUser.Id))
                {
                    var ThisUser = idb.Users.Find(u);
                    if (ThisUser != null)
                    {
                        if (ThisUser.ReceiveEmails)
                        {
                            string email = _UserHelp.GetUserEmailViaUserId(u);
                            Emails.Add(email);
                        }
                    }
                }

                Emails = Emails.Distinct().ToList();

                foreach (string e in Emails)
                {
                    ApplicationUser _user = idb.Users.Where(d => d.Email == e).FirstOrDefault();
                    sendNotification(currentUser.UserName + " Is " + Commitment + " Ride " + _Group.name, _user.UserName);
                }

                _UserHelp.SendUsersGroupAttendStatusEmail(Emails, GroupName, Commitment, currentUser.UserName, _Ride.Group.id, _Ride.RideDate);
            });

            E.Start();

            Task T = new Task(() =>
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.ConnectionStrings["AzureJobsData"].ConnectionString);

                // Create the queue client.
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

                // Retrieve a reference to a queue.
                CloudQueue queue = queueClient.GetQueueReference("updatehomepage");

                // Create the queue if it doesn't already exist.
                queue.CreateIfNotExists();

                // Create a message and add it to the queue.
                CloudQueueMessage message = new CloudQueueMessage("Hello, World");
                queue.AddMessage(message);

            });

            T.Start();

            return Json(new
            {
                success = true,
                message = Commitment,
                rideid = RideId,
                username = currentUser.UserName,
                keencount = KeenCount,
                leavetime = DateTime.UtcNow,
                parentid = ParentRideID
            }, JsonRequestBehavior.AllowGet);
        }
    }        
}