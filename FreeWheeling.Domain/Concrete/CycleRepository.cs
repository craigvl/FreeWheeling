using FreeWheeling.Domain.Abstract;
using FreeWheeling.Domain.DataContexts;
using FreeWheeling.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheeling.Domain.Concrete
{
    public class CycleRepository : ICycleRepository
    {
        private CycleDb context = new CycleDb();

        public IEnumerable<Group> GetGroups()
        {
            return context.Groups.Include("Members")
                .Include("Rides")
                .Include("Location")
                .Include("RideDays")
                .Where(g => g.IsPrivate == false)
                .ToList(); 
        }

        public IEnumerable<Group> GetGroupsIncludePrivate()
        {
            return context.Groups.Include("Members")
                .Include("Rides")
                .Include("Location")
                .Include("RideDays")
                .ToList();
        }

        public IEnumerable<Rider> GetRiders()
        {
            return context.Riders;
        }

        public IEnumerable<Group> GetGroupsByLocation(int? LocationID)
        {
            //return context.Groups.Include("Members").Include("Rides").Include("Location").Include("RideDays").Where(g => g.Location.id == LocationID).ToList();
            return context.Groups
                .Include("Rides")
                .Include("RideDays")
                .Where(g => g.Location.id == LocationID && g.IsPrivate == false).ToList();
        }

        public int GetGroupCount(int? LocationID)
        {
            return context.Groups.Where(g => g.Location.id == LocationID && g.IsPrivate == false).Count();
        }

        public IEnumerable<Group> GetGroupsByLocationWithSearch(int? LocationID, string SearchString)
        {
            return context.Groups.Include("Rides")
                .Include("RideDays")
                .Where(g => g.Location.id == LocationID
                && g.name.ToUpper().Contains(SearchString) && g.IsPrivate == false).ToList();
        }

        public IEnumerable<Group> GetFavouriteGroupsByLocation(int? LocationID, string CurrentUserId)
        {
            return context.Groups
                .Include("Members")
                .Include("Rides")
                .Include("RideDays")
                .Where(g => g.Location.id == LocationID 
                    && g.Members.Any(m => m.userId == CurrentUserId)
                    ).ToList();
        }

        public IEnumerable<Group> GetFavouriteGroupsByLocationWithSearch(int? LocationID, string SearchString, string CurrentUserId)
        {
            return context.Groups
                .Include("Members")
                .Include("Rides")
                .Include("RideDays")
                .Where(g => g.Location.id == LocationID
                && g.name.ToUpper().Contains(SearchString) 
                && g.Members.Any(m => m.userId == CurrentUserId)
                ).ToList();
        }

        public IEnumerable<Group> GetGroupsWithRiders()
        {
            return context.Groups
                .Include("Members")
                .Include("Rides")
                .Where(g => g.IsPrivate == false)
                .ToList();
        }

        public IEnumerable<Ride> GetRides()
        {
            return context.Rides;
        }

        public IEnumerable<Ad_HocRide> GetRandomRides()
        {
            return context.Ad_HocRide;
        }

        public IEnumerable<Ride> GetRidesWithRiders()
        {
            return context.Rides.Include("Riders");
        }

        public IEnumerable<Ad_HocRide> GetRandomRidesWithRiders()
        {
            return context.Ad_HocRide.Include("Riders");
        }

        public IEnumerable<Member> GetMembersWithGroups()
        {
            return context.Members.Include("Group").Where(g => g.Group.IsPrivate == false);
        }

        public IEnumerable<Member> GetMembersWithGroupsIncludePrivate()
        {
            return context.Members.Include("Group");
        }

        public Group GetGroupByID(int id)
        {
            Group group = context.Groups
                .Include("Members")
                .Include("Rides")
                .Include("Location")
                .Include("RideDays")
                .Where(i => i.id == id).FirstOrDefault();
            return group;
        }

        public Group GetGroupByRideID(int Rideid)
        {
            Ride _Ride = context.Rides.Include("Group").Where(r => r.id == Rideid).FirstOrDefault();
            Group group = context.Groups
                .Include("Members")
                .Include("Rides")
                .Include("Location")
                .Include("RideDays")
                .Where(i => i.id == _Ride.Group.id).FirstOrDefault();
            return group;
        }

        public Group GetGroupByIDNoIncludes(int id)
        {
            Group group = context.Groups.Where(i => i.id == id).FirstOrDefault();
            return group;
        }

        public List<CycleDays> GetCycleDaysForGroup(int GroupId)
        {
            return context.CycleDays.Where(t => t.Group.id == GroupId).ToList();
        }

        public List<int> CurrentGroupsForUser(string UserId)
        {
            List<int> GroupMemeberOf = new List<int>();
            List<Group> Groups = context.Groups.Where(u => u.Members.Any(m => m.userId == UserId)).ToList();

            foreach (Group item in Groups)
            {
                GroupMemeberOf.Add(item.id);
            }

            return (GroupMemeberOf);
        }

        public IEnumerable<Location> GetLocations()
        {
            return context.Locations.ToList();
        }

        public string GetLocationName(int? id)
        {
            return context.Locations.Where(l => l.id == id).Select(o => o.Name).FirstOrDefault();
        }

        public Ride GetRideByID(int id)
        {
            return context.Rides.Where(r => r.id == id).FirstOrDefault();
        }

        public Ride GetRideByIDIncludeGroup(int id)
        {
            return context.Rides.Include("Group").Where(r => r.id == id).FirstOrDefault();
        }

        public Ride GetClosestNextRide(Group _Group, TimeZoneInfo TimeZone)
        {
            DateTime LocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZone);
            Ride _Ride = context.Rides.Include("Riders").Where(t => t.Group.id == _Group.id && t.RideDate >= LocalNow).OrderBy(r => r.RideDate).FirstOrDefault();

            if (context.Rides.Where(t => t.Group.id == _Group.id && t.RideDate >= LocalNow).Count() == 1)
            {
                PopulateRideDatesFromDate(_Group, _Ride.RideDate, TimeZone);
            }

            return _Ride;
        }

        public Ride Get2ndClosestNextRide(Group _Group, TimeZoneInfo TimeZone)
        {
            DateTime LocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZone);
            Ride _ClosestRide = context.Rides.Include("Riders").Where(t => t.Group.id == _Group.id && t.RideDate >= LocalNow).OrderBy(r => r.RideDate).FirstOrDefault();
            Ride _2ndClosestRide = context.Rides.Include("Riders").Where(t => t.Group.id == _Group.id && t.RideDate > _ClosestRide.RideDate).OrderBy(r => r.RideDate).FirstOrDefault();

            if (context.Rides.Where(t => t.Group.id == _Group.id && t.RideDate >= LocalNow).Count() == 1)
            {
                PopulateRideDatesFromDate(_Group, _ClosestRide.RideDate, TimeZone);
            }

            return _2ndClosestRide;
        }

        public List<Ad_HocRide> GetAdHocRides(Location _Location, TimeZoneInfo TimeZone)
        {
            DateTime LocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZone);
            LocalNow = LocalNow.AddHours(-2);
            return context.Ad_HocRide.Where(l => l.Location.id == _Location.id 
                && l.RideDate >= LocalNow 
                && l.IsPrivate == false).ToList();
        }

        public Ad_HocRide GetAdHocRideByID(int id)
        {
            Ad_HocRide Ad = new Ad_HocRide();
            Ad = context.Ad_HocRide.Include("Location").Where(i => i.id == id).FirstOrDefault();
            return Ad;
        }

        public List<Group> GetPrivateGroupsByUserID(string UserId, Location _Location)
        {
            List<int> UsersPrivateGroups = context.PrivateGroupUsers.Where(u => u.UserId == UserId)
                .Select(g => g.GroupId)
                .ToList();
            UsersPrivateGroups.AddRange(context.Groups.Where(u => u.IsPrivate == true
                && u.CreatedBy == UserId).Select(t => t.id).ToList());
            return context.Groups.Include("Rides").Where(g => UsersPrivateGroups.Contains(g.id)
                && g.Location.id == _Location.id).Distinct().ToList();
        }

        public List<Ad_HocRide> GetPrivateAdHocRideByUserID(string UserId, Location _Location, TimeZoneInfo TimeZone)
        {
            DateTime LocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZone);
            LocalNow = LocalNow.AddHours(-2);
            List<int> UsersPrivateRandomRides = context.PrivateRandomUsers.Where(u => u.UserId == UserId)
                .Select(g => g.RideId)
                .ToList();
            UsersPrivateRandomRides.AddRange(context.Ad_HocRide.Where(u => u.IsPrivate == true
                && u.CreatedBy == UserId).Select(t => t.id).ToList());
            return context.Ad_HocRide.Where(g => UsersPrivateRandomRides.Contains(g.id) 
                && g.Location.id == _Location.id && g.RideDate >= LocalNow).Distinct().ToList();
        }

        public List<Comment> GetTop2CommentsForRide(int Rideid)
        {
            return context.Comment.Where(c => c.Ride.id == Rideid).OrderByDescending(t => t.Date).Take(2).ToList();
        }

        public List<Comment> GetAllCommentsForRide(int Rideid)
        {
            return context.Comment.Where(c => c.Ride.id == Rideid).OrderByDescending(t => t.Date).ToList();
        }

        public List<AdHocComment> GetTop2CommentsForAdHocRide(int AdHocRideid)
        {
            return context.AdHocComment.Where(r => r.AdHocRide.id == AdHocRideid).OrderByDescending(r => r.Date).Take(2).ToList();
        }

        public int GetFollowingCount(string UserID)
        {
            return context.UserFollowingUsers.Where(u => u.userId == UserID).Count();
        }

        public List<string> GetFollowers(string CurrentUserId)
        {
          List<string> Followers = new List<string>();
          return Followers = context.UserFollowingUsers.Where(f => f.followedUserId == CurrentUserId).Select(u => u.userId).ToList();
        }

        public List<AdHocComment> GetAllCommentsForAdHocRide(int AdHocRideid)
        {
            return context.AdHocComment.Where(r => r.AdHocRide.id == AdHocRideid).OrderByDescending(r => r.Date).ToList();
        }

        public int GetUpCommingAd_HocCount(Location _Location, TimeZoneInfo TimeZone)
        {
            DateTime LocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZone);
            LocalNow = LocalNow.AddHours(-2);
            return context.Ad_HocRide.Where(l => l.Location.id == _Location.id 
                && l.RideDate >= LocalNow
                && l.IsPrivate == false).Count();
        }

        public List<Rider> GetRidersForRide(int id, TimeZoneInfo TimeZone)
        {
            List<Rider> Riders = context.Riders.Where(r => r.Ride.id == id).ToList();

            foreach (Rider R in Riders)
            {
                R.LeaveTime = TimeZoneInfo.ConvertTimeFromUtc(R.LeaveTime, TimeZone);
            }
            return Riders.OrderBy(i => i.PercentKeen).ToList();
        }

        public List<Rider> GetRidersForRideDontIncludeCurrentUser(int id, TimeZoneInfo TimeZone, string CurrentUserId)
        {
            List<Rider> Riders = context.Riders.Where(r => r.Ride.id == id).ToList();

            foreach (Rider R in Riders)
            {
                R.LeaveTime = TimeZoneInfo.ConvertTimeFromUtc(R.LeaveTime, TimeZone);
            }
            return Riders.Where(g => g.userId != CurrentUserId).ToList();
        }

        public List<Rider> GetRidersAndCommentersForRideDontIncludeCurrentUser(int id, TimeZoneInfo TimeZone, string CurrentUserId)
        {
            List<Rider> Riders = context.Riders.Where(r => r.Ride.id == id).ToList();
            Ride _Ride = context.Rides.Where(r => r.id == id).FirstOrDefault();

            foreach (Rider R in Riders)
            {
                R.LeaveTime = TimeZoneInfo.ConvertTimeFromUtc(R.LeaveTime, TimeZone);
            }

            Riders.AddRange(GetCommentersForRide(_Ride));
            return Riders.Where(g => g.userId != CurrentUserId).ToList();
        }

        public List<Rider> GetCommentersForRide(Ride _Ride)
        {
            List<Comment> Comments = context.Comment.Where(k => k.Ride.id == _Ride.id).ToList();
            List<Rider> Riders = new List<Rider>();

            foreach (Comment item in Comments)
            {
                Rider r = context.Riders.Where(u => u.Name == item.userName).FirstOrDefault();

                if (r == null)
                {
                    //Userdb 
                    Rider CommentRider = new Rider { Name = item.userName, Ride = _Ride, userId = item.userId };
                    Riders.Add(CommentRider);
                }
                else
                {
                    Riders.Add(r);                    
                }
            }
            return Riders.Distinct().ToList();
        }

        public Member GetMemberByUserID(string id)
        {
            return context.Members.Where(m => m.userId == id).FirstOrDefault();
        }

        public int GetCommentCountForAdHocRide(int AdHocRideid)
        {
            return context.AdHocComment.Where(r => r.AdHocRide.id == AdHocRideid).Count();
        }

        public int GetCommentCountForRide(int Rideid)
        {
            return context.Comment.Where(r => r.Ride.id == Rideid).Count();
        }

        public int GetKeenCountForRide(int Rideid)
        {
            return context.Riders.Where(r => (r.Ride.id == Rideid && r.PercentKeen == "In") || (r.Ride.id == Rideid && r.PercentKeen == "OnWay")).Count();
        }

        public Ride GetHomePageRideByUserID(string UserId)
        {
            HomePageRide _HomePageRide = context.HomePageRide.Where(i => i.Userid == UserId).FirstOrDefault();
            if (_HomePageRide != null)
            {
                return context.Rides.Include("Group").Where(r => r.id == _HomePageRide.Rideid).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public Ad_HocRide GetHomePageRandomRideByUserID(string UserId)
        {
            HomePageRide _HomePageRide = context.HomePageRide.Where(i => i.Userid == UserId).FirstOrDefault();
            if (_HomePageRide != null)
            {
                return context.Ad_HocRide.Where(r => r.id == _HomePageRide.Rideid).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public int GetKeenCountForAdHocRide(int AdHocRideid)
        {
            return context.AdHocRider.Where(r => (r.AdHocRide.id == AdHocRideid && r.PercentKeen == "In") || (r.AdHocRide.id == AdHocRideid && r.PercentKeen == "OnWay")).Count();
        }

        public UserExpand GetUserExpandByUserID(string UserId)
        {
            return context.UserExpands.Where(e => e.userId == UserId).FirstOrDefault();
        }

        public List<AdHocRider> GetRidersForAdHocRide(int AdHocRideid, TimeZoneInfo TimeZone)
        {
            List<AdHocRider> AList = context.AdHocRider.Where(r => r.AdHocRide.id == AdHocRideid).ToList();

            foreach (AdHocRider AdHoc in AList)
            {
                AdHoc.LeaveTime = TimeZoneInfo.ConvertTimeFromUtc(AdHoc.LeaveTime, TimeZone);
            }

            return AList.OrderBy(i => i.PercentKeen).ToList();
        }

        public List<AdHocRider> GetRidersForAdHocRideDontIncludeCurrentUser(int AdHocRideid, TimeZoneInfo TimeZone, string CurrentUserId)
        {
            List<AdHocRider> AList = context.AdHocRider.Where(r => r.AdHocRide.id == AdHocRideid).ToList();

            foreach (AdHocRider AdHoc in AList)
            {
                AdHoc.LeaveTime = TimeZoneInfo.ConvertTimeFromUtc(AdHoc.LeaveTime, TimeZone);
            }

            return AList.Where(g => g.userId != CurrentUserId).ToList();
        }

        public List<AdHocRider> GetRidersAndCommentersForAdHocRideDontIncludeCurrentUser(int AdHocRideid, TimeZoneInfo TimeZone, string CurrentUserId)
        {
            List<AdHocRider> AList = context.AdHocRider.Where(r => r.AdHocRide.id == AdHocRideid).ToList();
            Ad_HocRide AdHocRide = context.Ad_HocRide.Where(l => l.id == AdHocRideid).FirstOrDefault();

            foreach (AdHocRider AdHoc in AList)
            {
                AdHoc.LeaveTime = TimeZoneInfo.ConvertTimeFromUtc(AdHoc.LeaveTime, TimeZone);
            }

            AList.AddRange(GetCommentersForAdHocRide(AdHocRide));

            return AList.Where(g => g.userId != CurrentUserId).ToList();
        }

        public List<AdHocRider> GetCommentersForAdHocRide(Ad_HocRide _AdHocRide)
        {

            List<AdHocComment> Comments = context.AdHocComment.Where(k => k.AdHocRide.id == _AdHocRide.id).ToList();

            List<AdHocRider> Riders = new List<AdHocRider>();

            foreach (AdHocComment item in Comments)
            {

                AdHocRider r = context.AdHocRider.Where(u => u.Name == item.userName).FirstOrDefault();

                if (r == null)
                {
                    //Userdb 
                    AdHocRider CommentRider = new AdHocRider { Name = item.userName,  AdHocRide = _AdHocRide, userId = item.userId };
                    Riders.Add(CommentRider);
                }
                else
                {
                    Riders.Add(r);
                }

            }
            return Riders;
        }

        public int GetUserExpandCount()
        {
            return context.UserExpands.Count();
        }

        /// <summary>
        /// Finds the next date whose day of the week equals the specified day of the week.
        /// </summary>
        /// <param name="startDate">
        ///		The date to begin the search.
        /// </param>
        /// <param name="desiredDay">
        ///		The desired day of the week whose date will be returneed.
        /// </param>
        /// <returns>
        ///		The returned date occurs on the given date's week.
        ///		If the given day occurs before given date, the date for the
        ///		following week's desired day is returned.
        /// </returns>
        public static DateTime GetNextDateForDay(DateTime startDate, DayOfWeek desiredDay)
        {
            // Given a date and day of week,
            // find the next date whose day of the week equals the specified day of the week.
            return startDate.AddDays(DaysToAdd(startDate.DayOfWeek, desiredDay));
        }

        public void AddMember(string UserId, Group _Group)
        {
            List<Group> CurrentGroups = context.Groups.Where(u => u.Members.Any(m => m.userId == UserId 
                                                             && u.id == _Group.id)).ToList();

            if (CurrentGroups != null)
            {
                Member NewMember = new Member { userId = UserId, Group = _Group };
                context.Members.Add(NewMember);
                context.Entry(NewMember).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }      
        }

        public void AddRider(Rider _Rider, Group _Group)
        {
            Rider CurrentRiders = context.Riders.Where(o => o.userId == _Rider.userId && o.Ride.id == _Rider.Ride.id).FirstOrDefault();
           
            //Rider CurrentRiders = context.Rides.Where(r => r.Group.id == _Group.id && r.Riders.Any(t => t.userId == UserId))).FirstOrDefault();

            if (CurrentRiders != null)
            {
                if (CurrentRiders.id != 0)
                {
                    CurrentRiders.PercentKeen = _Rider.PercentKeen;
                    CurrentRiders.LeaveTime = _Rider.LeaveTime;
                    CurrentRiders.Name = _Rider.Name;
                    context.Entry(CurrentRiders).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges(); 
                }
                else
                {
                    Rider NewRider = new Rider { userId = _Rider.userId, PercentKeen = _Rider.PercentKeen, Ride = _Rider.Ride, Name = _Rider.Name, LeaveTime = _Rider.LeaveTime };
                    context.Riders.Add(NewRider);
                    context.Entry(NewRider).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();
                }
            }
            else
            {
                Rider NewRider = new Rider { userId = _Rider.userId, PercentKeen = _Rider.PercentKeen, Ride = _Rider.Ride, Name = _Rider.Name, LeaveTime = _Rider.LeaveTime };
                context.Riders.Add(NewRider);
                context.Entry(NewRider).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }
        }

        public void AddRideComment(string Comment, int RideId, string UserName, string UserId)
        {
            Comment _comment = new Comment
            {
                CommentText = Comment,
                Ride = context.Rides.Where(t => t.id == RideId).FirstOrDefault(),
                userName = UserName,
                userId = UserId,
                Date = DateTime.Now
            };

            context.Comment.Add(_comment);
            context.Entry(_comment).State = System.Data.Entity.EntityState.Added;
            context.SaveChanges();
        }

        public void AddGroup(Group _Group)
        {
            context.Groups.Add(_Group);
            context.Entry(_Group).State = System.Data.Entity.EntityState.Added;
            context.SaveChanges();
        }

        public void AddPrivateAdHocInvite(List<PrivateRandomUsers> _PrivateRandomUsers)
        {
            foreach (PrivateRandomUsers item in _PrivateRandomUsers)
            {
                context.PrivateRandomUsers.Add(item);
                context.Entry(item).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }
        }

        public void AddPrivateGroupInvite(List<PrivateGroupUsers> _PrivateGroupUsers)
        {
            foreach (PrivateGroupUsers item in _PrivateGroupUsers)
            {
                context.PrivateGroupUsers.Add(item);
                context.Entry(item).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }
        }

        public void AddUserExpand(UserExpand _UserExpand)
        {
            context.UserExpands.Add(_UserExpand);
            context.Entry(_UserExpand).State = System.Data.Entity.EntityState.Added;
            context.SaveChanges();
        }

        public void AddAdHocRide(Ad_HocRide _AdHocRide)
        {
            context.Ad_HocRide.Add(_AdHocRide);
            context.Entry(_AdHocRide).State = System.Data.Entity.EntityState.Added;
            context.SaveChanges();
        }

        public void AddLocation(Location _Location)
        {
            context.Locations.Add(_Location);
            context.Entry(_Location).State = System.Data.Entity.EntityState.Added;
            context.SaveChanges();
        }

        public void AddAdHocRider(AdHocRider _Rider, Ad_HocRide _Ride)
        {
            AdHocRider CurrentRiders = context.AdHocRider.Where(o => o.userId == _Rider.userId && o.AdHocRide.id == _Ride.id).FirstOrDefault();

            //Rider CurrentRiders = context.Rides.Where(r => r.Group.id == _Group.id && r.Riders.Any(t => t.userId == UserId))).FirstOrDefault();

            if (CurrentRiders != null)
            {
                if (CurrentRiders.id != 0)
                {
                    CurrentRiders.PercentKeen = _Rider.PercentKeen;
                    CurrentRiders.LeaveTime = _Rider.LeaveTime;
                    CurrentRiders.Name = _Rider.Name;
                    context.Entry(CurrentRiders).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    AdHocRider NewRider = new AdHocRider { userId = _Rider.userId, PercentKeen = _Rider.PercentKeen, AdHocRide = _Ride, Name = _Rider.Name, LeaveTime = _Rider.LeaveTime };
                    context.AdHocRider.Add(NewRider);
                    context.Entry(NewRider).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();
                }
            }
            else
            {
                AdHocRider NewRider = new AdHocRider { userId = _Rider.userId, PercentKeen = _Rider.PercentKeen, AdHocRide = _Ride, Name = _Rider.Name, LeaveTime = _Rider.LeaveTime };
                context.AdHocRider.Add(NewRider);
                context.Entry(NewRider).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }
        }

        public void AddAdHocRideComment(string Comment, int RideId, string UserName, string UserId)
        {
            AdHocComment _comment = new AdHocComment
            {
                CommentText = Comment,
                AdHocRide = context.Ad_HocRide.Where(t => t.id == RideId).FirstOrDefault(),
                userId = UserId,
                userName = UserName,
                Date = DateTime.Now
            };

            context.AdHocComment.Add(_comment);
            context.Entry(_comment).State = System.Data.Entity.EntityState.Added;
            context.SaveChanges();
        }

        public void AddFollowingUser(string CurrentUserId, string UserId)
        {
            UserFollowingUser _UserFollowingUser = new UserFollowingUser { userId = CurrentUserId, followedUserId = UserId };
            context.UserFollowingUsers.Add(_UserFollowingUser);
            context.SaveChanges();
        }

        public void PopulateRideDatesFromDate(Group _Group, DateTime _DateTime, TimeZoneInfo _TimeZoneInfo)
        {
            DateTime LocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _TimeZoneInfo);
            //_DateTime = TimeZoneInfo.ConvertTimeFromUtc(_DateTime, TZone); //Not needed as time passed in is in local time.
            _Group.RideDays = GetCycleDaysForGroup(_Group.id);
            List<DayOfWeek> RideDays = new List<DayOfWeek>();
            List<Ride> RidesToAdd = new List<Ride>();

            foreach (CycleDays item in _Group.RideDays)
            {
                if (item.DayOfWeek == "Sunday") { RideDays.Add(DayOfWeek.Sunday); }
                if (item.DayOfWeek == "Monday") { RideDays.Add(DayOfWeek.Monday); }
                if (item.DayOfWeek == "Tuesday") { RideDays.Add(DayOfWeek.Tuesday); }
                if (item.DayOfWeek == "Wednesday") { RideDays.Add(DayOfWeek.Wednesday); }
                if (item.DayOfWeek == "Thursday") { RideDays.Add(DayOfWeek.Thursday); }
                if (item.DayOfWeek == "Friday") { RideDays.Add(DayOfWeek.Friday); }
                if (item.DayOfWeek == "Saturday") { RideDays.Add(DayOfWeek.Saturday); }
            }

            foreach (DayOfWeek day in RideDays)
            {
                DateTime nextdate = GetNextDateForDay(_DateTime, day);
                Ride NewRide = new Ride { Group = _Group, RideTime = _Group.RideTime, RideDate = nextdate.Date.Add(new TimeSpan(_Group.RideHour, _Group.RideMinute, 0)) };
                RidesToAdd.Add(NewRide); 
            }

            foreach (Ride item in RidesToAdd.OrderBy(h => h.RideDate))
            {
                context.Rides.Add(item);
                context.Entry(item).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }
        }

        public Group PopulateRideDates(Group _Group, TimeZoneInfo _TimeZoneInfo)
        {

            DateTime LocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _TimeZoneInfo);

            List<DayOfWeek> RideDays = new List<DayOfWeek>();
            _Group.RideDays = GetCycleDaysForGroup(_Group.id);
            foreach (CycleDays item in _Group.RideDays)
            {
                if (item.DayOfWeek == "Sunday"){ RideDays.Add(DayOfWeek.Sunday); }
                if (item.DayOfWeek == "Monday") { RideDays.Add(DayOfWeek.Monday); }
                if (item.DayOfWeek == "Tuesday") { RideDays.Add(DayOfWeek.Tuesday); }
                if (item.DayOfWeek == "Wednesday") { RideDays.Add(DayOfWeek.Wednesday); }
                if (item.DayOfWeek == "Thursday") { RideDays.Add(DayOfWeek.Thursday); }
                if (item.DayOfWeek == "Friday") { RideDays.Add(DayOfWeek.Friday); }
                if (item.DayOfWeek == "Saturday") { RideDays.Add(DayOfWeek.Saturday); }
            }

            List<Ride> RidesToAdd = new List<Ride>();

            foreach (DayOfWeek day in RideDays)
            {
                if (LocalNow.DayOfWeek == day)
                {
                   if (LocalNow.TimeOfDay <= (new TimeSpan(_Group.RideHour, _Group.RideMinute, 0)))
                   {
                       Ride NewRide = new Ride { Group = _Group, RideTime = _Group.RideTime, RideDate = LocalNow.Date.Add(new TimeSpan(_Group.RideHour, _Group.RideMinute, 0)) };
                       RidesToAdd.Add(NewRide);
                   }
                   else
                   {
                       DateTime nextdate = GetNextDateForDay(LocalNow, day);
                       Ride NewRide = new Ride { Group = _Group, RideTime = _Group.RideTime, RideDate = nextdate.Date.Add(new TimeSpan(_Group.RideHour, _Group.RideMinute, 0)) };
                       RidesToAdd.Add(NewRide);                     
                   }
                }
                else
                {
                    DateTime nextdate = GetNextDateForDay(LocalNow, day);
                    Ride NewRide = new Ride { Group = _Group, RideTime = _Group.RideTime, RideDate = nextdate.Date.Add(new TimeSpan(_Group.RideHour, _Group.RideMinute, 0)) };
                    RidesToAdd.Add(NewRide);
                }
            }

            foreach (Ride item in RidesToAdd.OrderBy(h => h.RideDate))
            {
                context.Rides.Add(item);
                context.Entry(item).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }

            return _Group;
        }

        /// <summary>
        /// Calculates the number of days to add to the given day of
        /// the week in order to return the next occurrence of the
        /// desired day of the week.
        /// </summary>
        /// <param name="current">
        ///		The starting day of the week.
        /// </param>
        /// <param name="desired">
        ///		The desired day of the week.
        /// </param>
        /// <returns>
        ///		The number of days to add to <var>current</var> day of week
        ///		in order to achieve the next <var>desired</var> day of week.
        /// </returns>
        public static int DaysToAdd(DayOfWeek current, DayOfWeek desired)
        {
            // f( c, d ) = g( c, d ) mod 7, g( c, d ) > 7
            //           = g( c, d ), g( c, d ) < = 7
            //   where 0 <= c < 7 and 0 <= d < 7
            int c = (int)current;
            int d = (int)desired;
            int n = (7 - c + d);
            return (n > 7) ? n % 7 : n;
        }

        public bool PrivateRandomBunchInviteUserEmailNotSet(int id)
        {
            string Userid = context.PrivateRandomUsers.Where(i => i.id == id).Select(j => j.UserId).FirstOrDefault();
            if (Userid == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool PrivateBunchInviteUserEmailNotSet(int id)
        {
            string Userid = context.PrivateGroupUsers.Where(i => i.id == id).Select(j => j.UserId).FirstOrDefault();
            if (Userid == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsInvitedToPrivateBunch(int GroupId, string UserId)
        {
            if (IsGroupCreator(GroupId,UserId))
            {
                return true;
            }

            int _Count = context.PrivateGroupUsers.Where(g => g.GroupId == GroupId
                && g.UserId == UserId).Count();

            if (_Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool IsInvitedToPrivateRandomBunch(int RideId, string UserId)
        {
            if (IsAdHocCreator(RideId, UserId))
            {
                return true;
            } 

            int _Count = context.PrivateRandomUsers.Where(g => g.RideId == RideId
                && g.UserId == UserId).Count();

            if (_Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsAdHocCreator(int AdHocRideid, string UserId)
        {
            Ad_HocRide adHoc = GetAdHocRideByID(AdHocRideid);

            if (adHoc.CreatedBy == UserId)
            {
                return true;
            }
            else{

                return false;
            }
        }

        public bool IsInFavouriteList(int _GroupId, string UserId)
        {
            Member _Member = context.Members.Where(i => i.Group.id == _GroupId && i.userId == UserId).FirstOrDefault();

            if (_Member == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        //Check to see if CurrentUserId has followed UserId, used in the follow usersettings page to swap follow unfollow button.
        public bool IsFollowing(string CurrentUserId, string UserId)
        {
            int count = context.UserFollowingUsers.Where(c => c.userId == CurrentUserId && c.followedUserId == UserId).Count();
            if (count > 0)
                return true;
            else
                return false;
        }

        public bool IsGroupCreator(int _GroupId, string UserId)
        {
            Group CurrentGroup = GetGroupByIDNoIncludes(_GroupId);

            if (CurrentGroup == null)
            {
                return false;
            }

            if (CurrentGroup.CreatedBy == UserId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsIn(int RideId, string UserId)
        {
            int KeenCountForRider = context.Riders.Where(r => r.Ride.id == RideId && r.PercentKeen == "In" && r.userId == UserId).Count();
            if (KeenCountForRider > 0)
                return true;
            else
                return false;
        }

        public bool IsOut(int RideId, string UserId)
        {
            int KeenCountForRider = context.Riders.Where(r => r.Ride.id == RideId && r.PercentKeen == "Out" && r.userId == UserId).Count();
            if (KeenCountForRider > 0)
                return true;
            else
                return false;
        }

        public bool IsOnWay(int RideId, string UserId)
        {
            int KeenCountForRider = context.Riders.Where(r => r.Ride.id == RideId && r.PercentKeen == "OnWay" && r.userId == UserId).Count();
            if (KeenCountForRider > 0)
                return true;
            else
                return false;
        }

        public bool IsInRandom(int RideId, string UserId)
        {
            int KeenCountForRider = context.AdHocRider.Where(r => r.AdHocRide.id == RideId && r.PercentKeen == "In" && r.userId == UserId).Count();
            if (KeenCountForRider > 0)
                return true;
            else
                return false;
        }

        public bool IsOutRandom(int RideId, string UserId)
        {
            int KeenCountForRider = context.AdHocRider.Where(r => r.AdHocRide.id == RideId && r.PercentKeen == "Out" && r.userId == UserId).Count();
            if (KeenCountForRider > 0)
                return true;
            else
                return false;
        }

        public bool IsOnWayRandom(int RideId, string UserId)
        {
            int KeenCountForRider = context.AdHocRider.Where(r => r.AdHocRide.id == RideId && r.PercentKeen == "OnWay" && r.userId == UserId).Count();
            if (KeenCountForRider > 0)
                return true;
            else
                return false;
        }

        public void UpdateGroup(Group _Group)
        {
            Group CurrentGroup = context.Groups.Where(i => i.id == _Group.id).FirstOrDefault();
            CurrentGroup.name = _Group.name;
            CurrentGroup.AverageSpeed = _Group.AverageSpeed;
            CurrentGroup.Location = _Group.Location;
            CurrentGroup.ModifiedTimeStamp = _Group.ModifiedTimeStamp;
            CurrentGroup.RideHour = _Group.RideHour;
            CurrentGroup.RideMinute = _Group.RideMinute;
            CurrentGroup.RideTime = _Group.RideTime;
            CurrentGroup.StartLocation = _Group.StartLocation;
            CurrentGroup.CreatedTimeStamp = CurrentGroup.CreatedTimeStamp;
            CurrentGroup.CreatedBy = CurrentGroup.CreatedBy;
            CurrentGroup.MapUrl = _Group.MapUrl;
            CurrentGroup.IsPrivate = _Group.IsPrivate;
            CurrentGroup.Description = _Group.Description;
            CurrentGroup.CreatedByName = _Group.CreatedByName;
            context.Entry(CurrentGroup).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
        }

        public void UpdateAdHocRide(Ad_HocRide _AdHocRide)
        {
            Ad_HocRide CurrentAdHocRide = context.Ad_HocRide.Where(i => i.id == _AdHocRide.id).FirstOrDefault();
            CurrentAdHocRide.Name = _AdHocRide.Name;
            CurrentAdHocRide.AverageSpeed = _AdHocRide.AverageSpeed;
            CurrentAdHocRide.Location = _AdHocRide.Location;
            CurrentAdHocRide.ModifiedTimeStamp = _AdHocRide.ModifiedTimeStamp;
            CurrentAdHocRide.RideDate = _AdHocRide.RideDate;
            CurrentAdHocRide.RideHour = _AdHocRide.RideHour;
            CurrentAdHocRide.RideMinute = _AdHocRide.RideMinute;
            CurrentAdHocRide.RideTime = _AdHocRide.RideTime;
            CurrentAdHocRide.StartLocation = _AdHocRide.StartLocation;
            CurrentAdHocRide.CreatedTimeStamp = CurrentAdHocRide.CreatedTimeStamp;
            CurrentAdHocRide.CreatedBy = CurrentAdHocRide.CreatedBy;
            CurrentAdHocRide.MapUrl = _AdHocRide.MapUrl;
            CurrentAdHocRide.Description = _AdHocRide.Description;
            CurrentAdHocRide.IsPrivate = _AdHocRide.IsPrivate;
            CurrentAdHocRide.CreatedByName = _AdHocRide.CreatedByName;
            context.Entry(CurrentAdHocRide).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
        }

        public void UpdateUserExpand(UserExpand _UserExpand)
        {
            UserExpand CurrentUserExpand = context.UserExpands.Where(e => e.userId == _UserExpand.userId).FirstOrDefault();
            CurrentUserExpand.FirstBunch = _UserExpand.FirstBunch;
            CurrentUserExpand.FirstComment = _UserExpand.FirstComment;
            CurrentUserExpand.FirstKeen = _UserExpand.FirstKeen;
            CurrentUserExpand.SecondBunch = _UserExpand.SecondBunch;
            CurrentUserExpand.SecondKeen = _UserExpand.SecondKeen;
            CurrentUserExpand.SecondComment = _UserExpand.SecondComment;
            context.Entry(CurrentUserExpand).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
        }

        public void UpdateRideTimes(Group _Group, TimeZoneInfo TimeZone)
        {
            //Need to do work here if the time is less than current time and same day then might need to create new ride date 
            Group CurrentGroup = context.Groups.Where(i => i.id == _Group.id).FirstOrDefault();
            foreach (Ride _Ride in CurrentGroup.Rides)
            {
                _Ride.RideDate = new DateTime(_Ride.RideDate.Year, _Ride.RideDate.Month, _Ride.RideDate.Day, _Group.RideHour, _Group.RideMinute, 0);
                _Ride.RideTime = _Group.RideTime;
            }
            context.Entry(CurrentGroup).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
        }

        public void UpdateInvitePrivateUser(string UserId, string UserEmail, int id)
        {
            PrivateGroupUsers _CurrentPrivateGroupUser = context.PrivateGroupUsers.Where(i => i.id == id).FirstOrDefault();
            _CurrentPrivateGroupUser.UserId = UserId;
            _CurrentPrivateGroupUser.Email = UserEmail;
            context.Entry(_CurrentPrivateGroupUser).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
        }

        public void UpdateInviteRandomPrivateUser(string UserId, string UserEmail, int id)
        {
            PrivateRandomUsers _CurrentPrivateRandomUser = context.PrivateRandomUsers.Where(i => i.id == id).FirstOrDefault();
            _CurrentPrivateRandomUser.UserId = UserId;
            _CurrentPrivateRandomUser.Email = UserEmail;
            context.Entry(_CurrentPrivateRandomUser).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
        }

        public void DeleteGroup(int GroupId)
        {
            Group CurrentGroup = context.Groups.Include("Members").Where(g => g.id == GroupId).FirstOrDefault();
            foreach (Member _Member in CurrentGroup.Members.ToList())
            {
                context.Entry(_Member).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }

            context.Groups.Remove(CurrentGroup);
            context.Entry(CurrentGroup).State = System.Data.Entity.EntityState.Deleted;
            context.SaveChanges();
        }

        public void DeleteHomePageRide(string UserId)
        {
            HomePageRide CurrentHomePageRide = context.HomePageRide.Where(u => u.Userid == UserId).FirstOrDefault();
            context.HomePageRide.Remove(CurrentHomePageRide);
            context.Entry(CurrentHomePageRide).State = System.Data.Entity.EntityState.Deleted;
            context.SaveChanges();
        }

        public void DeleteAdHocRide(int AdHocId)
        {
            Ad_HocRide CurrentAdHocRide = context.Ad_HocRide.Where(g => g.id == AdHocId).FirstOrDefault();
            context.Ad_HocRide.Remove(CurrentAdHocRide);
            context.Entry(CurrentAdHocRide).State = System.Data.Entity.EntityState.Deleted;
            context.SaveChanges();
        }

        public void DeleteOldRandomRide(int RandomRideId, TimeZoneInfo TimeZone)
        {          
            DateTime LocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZone);
            Ad_HocRide _Ad_HocRide = context.Ad_HocRide.Where(h => h.id == RandomRideId).FirstOrDefault();

            if (_Ad_HocRide != null)
            {
                if (_Ad_HocRide.RideDate.AddHours(1) < LocalNow)
                {
                    if (_Ad_HocRide.Riders != null)
                    {
                        foreach (AdHocRider _Rider in _Ad_HocRide.Riders.ToList())
                        {
                            context.Entry(_Rider).State = System.Data.Entity.EntityState.Deleted;
                            context.SaveChanges();
                        }
                    }
                    context.Entry(_Ad_HocRide).State = System.Data.Entity.EntityState.Deleted;
                    context.SaveChanges();
                }
            }          
        }

        public void DeleteOldRides(int GroupId, TimeZoneInfo TimeZone)
        {
            Group CurrentGroup = context.Groups.Include("Rides").Where(g => g.id == GroupId).FirstOrDefault();
            DateTime LocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZone);

            foreach (Ride _Ride in CurrentGroup.Rides.ToList())
            {
                if(_Ride.RideDate.AddHours(1) < LocalNow)
                {
                    if (_Ride.Riders != null)
                    {
                        foreach (Rider _Rider in _Ride.Riders.ToList())
                        {
                            context.Entry(_Rider).State = System.Data.Entity.EntityState.Deleted;
                            context.SaveChanges();
                        } 
                    }
                    context.Entry(_Ride).State = System.Data.Entity.EntityState.Deleted;
                    context.SaveChanges();
                }                
            }
        }

        public void RemoveMember(string UserId, Group _Group)
        {
            Member CurrentMember = context.Members.Where(g => g.Group.id == _Group.id && g.userId == UserId).FirstOrDefault();
            context.Members.Remove(CurrentMember);
            context.Entry(CurrentMember).State = System.Data.Entity.EntityState.Deleted;
            context.SaveChanges();
        }

        public void DeleteFollowingUser(string CurrentUserId, string UserId)
        {
            UserFollowingUser _UserFollowingUser = context.UserFollowingUsers.Where(c => c.userId == CurrentUserId && c.followedUserId == UserId).FirstOrDefault();
            if (_UserFollowingUser != null)
            {
                context.UserFollowingUsers.Remove(_UserFollowingUser);
                context.Entry(_UserFollowingUser).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
        }

        public void PopulateUserHomePageRides(List<HomePageRide> _HomePageRides)
        {
            context.Database.ExecuteSqlCommand("TRUNCATE TABLE [HomePageRides]");

            foreach (HomePageRide item in _HomePageRides)
            {
                context.HomePageRide.Add(item);
                context.Entry(item).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }
        }

        public void PopulateInitialExpandValues(string UserId)
        {
            UserExpand CurrentUserExpand = context.UserExpands.Where(y => y.userId == UserId).FirstOrDefault();
            if (CurrentUserExpand == null)
            {
                UserExpand UserExpands = new UserExpand
                {
                    FirstBunch = true,
                    FirstKeen = true,
                    FirstComment = true,
                    SecondBunch = true,
                    SecondComment = true,
                    SecondKeen = true,
                    userId = UserId
                };
                context.UserExpands.Add(UserExpands);
                context.Entry(UserExpands).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }        
        }

        public void Save()
        {
            context.SaveChanges();
        }    
    }
}