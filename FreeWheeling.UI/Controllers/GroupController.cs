using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FreeWheeling.Domain.Entities;
using FreeWheeling.UI.DataContexts;
using Microsoft.AspNet.Identity;
using FreeWheeling.Domain.Abstract;
using FreeWheeling.UI.Models;
using FreeWheeling.UI.Infrastructure;
using System.Globalization;
using System.Threading.Tasks;

namespace FreeWheeling.UI.Controllers
{
    [Authorize]
    public class GroupController : Controller
    {
        private IdentityDb idb = new IdentityDb(); 
        private ICycleRepository repository;

        public GroupController(ICycleRepository repoParam)
        {
            repository = repoParam;
        }

        // GET: /Group/
        public ActionResult Index(string searchString)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            CultureHelper _CultureHelper = new CultureHelper(repository);
            Location _Location = repository.GetLocations().Where(l => l.id == currentUser.LocationID).FirstOrDefault();
            Session["Culture"] = _CultureHelper.GetCulture(Convert.ToInt32(currentUser.LocationID));
            GroupModel _GroupModel = new GroupModel();
            GroupModelHelper _GroupHelper = new GroupModelHelper(repository);
            _GroupModel = _GroupHelper.PopulateGroupModel(currentUser.Id, _Location, searchString, currentUser.Email);
            return View(_GroupModel);
        }

        public PartialViewResult GetGroupDetails(int id)
        {
            Group _Group = new Group();
            _Group = repository.GetGroupByID(id);
            MoreGroupDetailsModel _MoreGroupDetailsModel = new MoreGroupDetailsModel();
            _MoreGroupDetailsModel.AverageSpeed = _Group.AverageSpeed;
            _MoreGroupDetailsModel.StartLocation = _Group.StartLocation;
            _MoreGroupDetailsModel.Description = _Group.Description;
            return PartialView("_GroupDetailPartial", _MoreGroupDetailsModel);
        }

        public ActionResult CreateAdHoc()
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            CultureHelper _CultureHelper = new CultureHelper(repository);
            TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(currentUser.LocationID);
            DateTime LocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZone);          
            Location _Location = repository.GetLocations().Where(l => l.id == currentUser.LocationID).FirstOrDefault();
            AdHocCreateModel _Ad_HocRide = new AdHocCreateModel();
            _Ad_HocRide.Locations = repository.GetLocations().ToList();
            _Ad_HocRide.RideDate = LocalNow;
            _Ad_HocRide.DateString = LocalNow.ToString("dd/MM/yyyy");
            _Ad_HocRide.LocationsId = _Location.id;
            _Ad_HocRide.Hour = 5;
            _Ad_HocRide.Minute = 30;
            return View(_Ad_HocRide);
        }

        [HttpPost]
        public JsonResult CreateAdHoc(AdHocCreateModel _AdHocCreateModel)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Location _Location = repository.GetLocations().Where(l => l.id == _AdHocCreateModel.LocationsId).FirstOrDefault();
            CultureHelper _CultureHelper = new CultureHelper(repository);
            TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(currentUser.LocationID);
            DateTime LocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZone);
            DateTime dateResult;

            if (!DateTime.TryParse(_AdHocCreateModel.DateString, _CultureHelper.GetCulture(_Location.id), DateTimeStyles.None, out dateResult))
            {
                ModelState.AddModelError(string.Empty, "Date is not in a valid date format");
                _AdHocCreateModel.Locations = repository.GetLocations().ToList();
                _AdHocCreateModel.LocationsId = _Location.id;
                return Json(new { success = false, Message = "Date is not a valid Date" }, JsonRequestBehavior.AllowGet);
                //return View(_AdHocCreateModel);
            }
            else
            {
                DateTime da = DateTime.ParseExact(_AdHocCreateModel.DateString, "dd/MM/yyyy", null);
                DateTime _RideDate = da.Date.Add(new TimeSpan(_AdHocCreateModel.Hour, _AdHocCreateModel.Minute, 0));

                if (_RideDate < LocalNow)
                {
                    ModelState.AddModelError(string.Empty, "Please select date and time that is greater than current date and time");
                    _AdHocCreateModel.Locations = repository.GetLocations().ToList();
                    _AdHocCreateModel.LocationsId = _Location.id;
                    return Json(new { success = false,
                        Message = "Please select date and time that is greater than current date and time" }, JsonRequestBehavior.AllowGet);
                }

                Ad_HocRide NewAdHoc = new Ad_HocRide
                {
                    Name = _AdHocCreateModel.Name,
                    AverageSpeed = _AdHocCreateModel.AverageSpeed,
                    Location = _Location,
                    RideDate = _RideDate,
                    Creator = currentUser.UserName,
                    StartLocation = _AdHocCreateModel.StartLocation,
                    Description = _AdHocCreateModel.Description,
                    RideTime = _RideDate.TimeOfDay.ToString(),
                    RideHour = _RideDate.Hour,
                    RideMinute = _RideDate.Minute,
                    CreatedBy = currentUser.Id,
                    CreatedTimeStamp = LocalNow,
                    ModifiedTimeStamp = LocalNow,
                    MapUrl = _AdHocCreateModel.MapUrl,
                    IsPrivate = _AdHocCreateModel.IsPrivate
                };

                repository.AddAdHocRide(NewAdHoc);
                repository.Save();

                if (_AdHocCreateModel.InviteUsers != null)
                {
                    Task T = new Task(() =>
                    {
                        List<string> UserNames = new List<string>();
                        UserHelper _UserHelp = new UserHelper();
                        List<PrivateRandomUsers> _PrivateRandomUsersList = new List<PrivateRandomUsers>();

                        foreach (InviteUser item in _AdHocCreateModel.InviteUsers)
                        {
                            UserNames.Add(item.UserName);

                            if (_AdHocCreateModel.IsPrivate)
                            {
                                if (_UserHelp.IsValidUserName(item.UserName))
                                {
                                    var _User = idb.Users.Where(g => g.UserName == item.UserName).FirstOrDefault();
                                    PrivateRandomUsers _PrivateRandomUsers = new PrivateRandomUsers
                                    {
                                        RideId = NewAdHoc.id,
                                        Email = _User.Email,
                                        UserId = _User.Id
                                    };
                                    _PrivateRandomUsersList.Add(_PrivateRandomUsers);
                                }
                                else
                                {
                                    PrivateRandomUsers _PrivateRandomUsers = new PrivateRandomUsers
                                    {
                                        RideId = NewAdHoc.id,
                                        Email = item.UserName,
                                    };
                                    _PrivateRandomUsersList.Add(_PrivateRandomUsers);
                                }
                            }
                        }
                        repository.AddPrivateAdHocInvite(_PrivateRandomUsersList);
                        repository.Save();
                        _UserHelp.SendUsersCreateAdHocEmail(_UserHelp.GetEmailsForUserNames(UserNames),
                            NewAdHoc.id,
                            NewAdHoc.CreatedBy,
                            NewAdHoc.Name);
                    });

                    T.Start();
                }
                return Json(new { success = true, Message = "New AdHoc Ride has been created." }, 
                    JsonRequestBehavior.AllowGet);  
            }
            
        }

        //Get user names for auto lookup
        [HttpGet]
        public JsonResult GetNames(string term)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());

            // A list of names to mimic results from a database
            List<string> nameList = idb.Users.Where(y => y.LocationID == currentUser.LocationID)
                .Select(i => i.UserName).ToList();

            var results = nameList.Where(n =>
                n.StartsWith(term, StringComparison.OrdinalIgnoreCase));

            return new JsonResult()
            {
                Data = results.ToArray(),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult EditGroup(int groupId)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());

            if (!repository.IsGroupCreator(groupId, currentUser.Id))
            {
                return RedirectToAction("Index", "Group");
            }
            else
            {
                CultureHelper _CultureHelper = new CultureHelper(repository);
                TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(currentUser.LocationID);
                DateTime LocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZone);
                Group CurrentGroup = repository.GetGroupByID(groupId);

                EditGroupModel _EditGroupModel = new EditGroupModel
                {
                    AverageSpeed = CurrentGroup.AverageSpeed,
                    GroupId = groupId,
                    Hour = CurrentGroup.RideHour,
                    Minute = CurrentGroup.RideMinute,
                    LocationsId = CurrentGroup.Location.id,
                    Name = CurrentGroup.name,
                    StartLocation = CurrentGroup.StartLocation,
                    Locations = repository.GetLocations().ToList(),
                    MapUrl = CurrentGroup.MapUrl
                };

                _EditGroupModel.LocationsId = repository.GetLocations()
                    .Where(l => l.id == CurrentGroup.Location.id).Select(t => t.id).FirstOrDefault();

                foreach (DayOfWeekViewModel ditem in _EditGroupModel.DaysOfWeek)
                {
                    foreach (CycleDays item in CurrentGroup.RideDays)
                    {
                        if (ditem.Name == item.DayOfWeek)
                        {
                            ditem.Checked = true;
                        }
                    }
                }
                return View(_EditGroupModel);
            }
        }

        [HttpPost]
        public ActionResult EditGroup(EditGroupModel _EditGroupModel)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            List<CycleDays> _CycleDays = new List<CycleDays>();
            Location _Location = repository.GetLocations()
                .Where(l => l.id == _EditGroupModel.LocationsId).FirstOrDefault();
            CultureHelper _CultureHelper = new CultureHelper(repository);
            TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(currentUser.LocationID);
            DateTime LocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZone);         
            Group CurrentGroup = repository.GetGroupByID(_EditGroupModel.GroupId);

            foreach (DayOfWeekViewModel item in _EditGroupModel.DaysOfWeek)
            {
                if (item.Checked)
                {
                    CycleDays NewDay = new CycleDays { DayOfWeek = item.Name };
                    _CycleDays.Add(NewDay);
                }
            }

            Group UpdatedGroup = new Group
            {
                name = _EditGroupModel.Name,
                RideTime = _EditGroupModel.Hour.ToString() + ":" + _EditGroupModel.Minute.ToString(),
                RideDays = _CycleDays,
                Location = _Location,
                Rides = new List<Ride>(),
                AverageSpeed = _EditGroupModel.AverageSpeed,
                StartLocation = _EditGroupModel.StartLocation,
                RideHour = _EditGroupModel.Hour,
                RideMinute = _EditGroupModel.Minute,
                CreatedBy = CurrentGroup.CreatedBy,
                CreatedTimeStamp = CurrentGroup.CreatedTimeStamp,
                ModifiedTimeStamp = LocalNow,
                MapUrl = _EditGroupModel.MapUrl,  
                id = _EditGroupModel.GroupId
            };

            repository.UpdateGroup(UpdatedGroup);
            repository.Save();
            repository.UpdateRideTimes(UpdatedGroup,TZone);
            repository.Save();
            //Not needed if not able to change days would need to do some work here if allowed.
            //NewGroup = repository.PopulateRideDates(NewGroup);
            //repository.Save();

            return RedirectToAction("Index", "Group");
        }

        public ActionResult DeleteGroup(int GroupId)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());

            if (!repository.IsGroupCreator(GroupId, currentUser.Id))
            {
                return RedirectToAction("Index", "Group");
            }
            else
            {
                Group CurrentGroup = repository.GetGroupByID(GroupId);
                DeleteGroupModel _DeleteGroupModel = new DeleteGroupModel { GroupId = GroupId, Name = CurrentGroup.name };
                return View(_DeleteGroupModel);
            }
        }

        [HttpPost, ActionName("DeleteGroup")]
        public ActionResult DeleteConfirmed(int GroupId)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());

            if (!repository.IsGroupCreator(GroupId, currentUser.Id))
            {
                return RedirectToAction("Index", "Group");
            }
            else
            {
                repository.DeleteGroup(GroupId);
                repository.Save();
                return RedirectToAction("Index", "Group");
            }
        }

        public ActionResult Create()
        {
            GroupCreateModel _GroupCreateModel = new GroupCreateModel();
            _GroupCreateModel.Locations = repository.GetLocations().ToList();
            _GroupCreateModel.Hour = 5;
            _GroupCreateModel.Minute = 30;
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Location _Location = repository.GetLocations().Where(l => l.id == currentUser.LocationID).FirstOrDefault();
            _GroupCreateModel.LocationsId = _Location.id;
            return View(_GroupCreateModel);
        }
       
        [HttpPost]
        public ActionResult Create(GroupCreateModel _GroupCreateModel)
        {
            List<CycleDays> _CycleDays = new List<CycleDays>();
            Location _Location = repository.GetLocations().Where(l => l.id == _GroupCreateModel.LocationsId).FirstOrDefault();
            CultureHelper _CultureHelper = new CultureHelper(repository);
            TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(_GroupCreateModel.LocationsId);
            DateTime LocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZone);
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            bool DayOfWeekSelected = false;
            
            foreach (DayOfWeekViewModel item in _GroupCreateModel.DaysOfWeek)
            {
                if (item.Checked)
                {
                    CycleDays NewDay = new CycleDays { DayOfWeek = item.Name };
                    _CycleDays.Add(NewDay);
                    DayOfWeekSelected = true;
                }
            }

            if(!DayOfWeekSelected)
            {
                ModelState.AddModelError(string.Empty, "Please select one or more days");
                _GroupCreateModel.Locations = repository.GetLocations().ToList();
                _GroupCreateModel.LocationsId = _Location.id;
                return View(_GroupCreateModel);
            }

            Group NewGroup = new Group
            {
                name = _GroupCreateModel.Name,
                RideTime = _GroupCreateModel.Hour.ToString() +":"+ _GroupCreateModel.Minute.ToString() + " " + _GroupCreateModel.AM_PM,
                RideDays = _CycleDays, Location = _Location, Rides = new List<Ride>(), AverageSpeed = _GroupCreateModel.AverageSpeed, 
                StartLocation = _GroupCreateModel.StartLocation,
                Description = _GroupCreateModel.Description,
                RideHour = _GroupCreateModel.Hour,
                RideMinute = _GroupCreateModel.Minute,
                CreatedBy = currentUser.Id,
                ModifiedTimeStamp = LocalNow,              
                CreatedTimeStamp = LocalNow,
                MapUrl = _GroupCreateModel.MapUrl,
                IsPrivate = _GroupCreateModel.IsPrivate
            };

            repository.AddGroup(NewGroup);
            repository.Save();
            NewGroup = repository.PopulateRideDates(NewGroup,TZone);
            repository.Save();

            if (_GroupCreateModel.IsPrivate)
            {
                return RedirectToAction("InviteOthersToPrivateBunch", "Group", new { GroupId = NewGroup.id });
            }

            return RedirectToAction("Index", "Group");
        }

        public ActionResult RemoveFromFavouriteList(int id, string title)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Member _Member = repository.GetMemberByUserID(currentUser.Id);
            Group group = repository.GetGroupByID(id);
            CultureHelper _CultureHelper = new CultureHelper(repository);
            TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(currentUser.LocationID);
            repository.RemoveMember(currentUser.Id, group);
            repository.Save();

            if(title == "Favourite bunches")
            {
                return RedirectToAction("Mybunches", "Group");
            }
            else{
                return RedirectToAction("Index", "Group");
            }
        }

        //// This is to add group to fav list
        public ActionResult Join(int id, string title)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Member _Member = repository.GetMemberByUserID(currentUser.Id);
            Group group = repository.GetGroupByID(id);
            repository.AddMember(currentUser.Id, group);
            repository.Save();
            return RedirectToAction("Index", "Group");
        }

        public ViewResult Mybunches(string searchString)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Location _Location = repository.GetLocations().Where(l => l.id == currentUser.LocationID).FirstOrDefault();
            GroupModel _GroupModel = new GroupModel();
            GroupModelHelper _GroupHelper = new GroupModelHelper(repository);
            _GroupModel = _GroupHelper.PopulateGroupModel(currentUser.Id, _Location, currentUser.Email, searchString, true);
            return View("Index",_GroupModel);
        }

        public ActionResult InviteOthersToPrivateBunch(int GroupId)
        {
            Group _Group = repository.GetGroupByID(GroupId);
            InviteOthersToPrivateBunchModel _InviteOthersToPrivateBunchModel = new InviteOthersToPrivateBunchModel
            {
                GroupId = GroupId,
                Name = _Group.name
            };

            return View(_InviteOthersToPrivateBunchModel);
        }

        [HttpPost]
        public JsonResult InviteOthersToPrivateBunch(InviteOthersToPrivateBunchModel _InviteOthersToPrivateBunchModel)
        {

            var currentUser = idb.Users.Find(User.Identity.GetUserId());

            if (_InviteOthersToPrivateBunchModel.InviteUsers != null)
            {
                Task T = new Task(() =>
                {
                    UserHelper _UserHelp = new UserHelper();
                    Group _Group = repository.GetGroupByID(_InviteOthersToPrivateBunchModel.GroupId);
                    List<PrivateGroupUsers> _PrivateGroupUsersList = new List<PrivateGroupUsers>();
                    List<string> UserNames = new List<string>();
                    foreach (InviteUser item in _InviteOthersToPrivateBunchModel.InviteUsers)
                    {
                        UserNames.Add(item.UserName);

                        if (_UserHelp.IsValidUserName(item.UserName))
                        {
                            var _User = idb.Users.Where(g => g.UserName == item.UserName).FirstOrDefault();
                            PrivateGroupUsers _PrivateGroupUsers = new PrivateGroupUsers
                            {
                                GroupId = _InviteOthersToPrivateBunchModel.GroupId,
                                Email = _User.Email,
                                UserId = _User.Id
                            };
                            _PrivateGroupUsersList.Add(_PrivateGroupUsers);
                        }
                        else
                        {
                            PrivateGroupUsers _PrivateGroupUsers = new PrivateGroupUsers
                            {
                                GroupId = _InviteOthersToPrivateBunchModel.GroupId,
                                Email = item.UserName,
                            };
                            _PrivateGroupUsersList.Add(_PrivateGroupUsers);
                        }
                    }

                    repository.AddPrivateGroupInvite(_PrivateGroupUsersList);
                    repository.Save();

                    _UserHelp.SendUsersPrivateBunchInviteEmail(_UserHelp.GetEmailsForUserNames(UserNames),
                        _InviteOthersToPrivateBunchModel.GroupId,
                        currentUser.UserName,
                        _Group.name);
                });

                T.Start();
            }

            return Json(new
            {
                success = true,
                message = "Emails Sent",
                GroupId = _InviteOthersToPrivateBunchModel.GroupId
            }, JsonRequestBehavior.AllowGet);
        }
    }
}