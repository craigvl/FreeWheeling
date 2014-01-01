﻿using FreeWheeling.Domain.Abstract;
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
            return context.Groups.Include("Members").Include("Rides").Include("Location").Include("RideDays").ToList(); 
        }

        public IEnumerable<Group> GetGroupsByLocation(int? LocationID)
        {
            return context.Groups.Include("Members").Include("Rides").Include("Location").Include("RideDays").Where(g => g.Location.id == LocationID).ToList();
        }

        public Group GetGroupByID(int id)
        {
            Group group = context.Groups.Include("Members").Include("Rides").Include("Location").Include("RideDays").Where(i => i.id == id).FirstOrDefault();

            return group;
        }

        public IEnumerable<Group> GetGroupsWithRiders()
        {
            return context.Groups.Include("Members").Include("Rides").ToList();
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
            }
       
        }

        public string GetLocationName(int? id)
        {


            return context.Locations.Where(l => l.id == id).Select(o => o.Name).FirstOrDefault();

        }

        public void RemoveMember(string UserId, Group _Group)
        {
            Member CurrentMember = context.Members.Where(g => g.Group.id == _Group.id && g.userId == UserId).FirstOrDefault();
            context.Members.Remove(CurrentMember);
            context.Entry(CurrentMember).State = System.Data.Entity.EntityState.Deleted;
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
                }
                else
                {

                    Rider NewRider = new Rider { userId = _Rider.userId, PercentKeen = _Rider.PercentKeen, Ride = _Rider.Ride, Name = _Rider.Name, LeaveTime = _Rider.LeaveTime };
                    context.Riders.Add(NewRider);
                    context.Entry(NewRider).State = System.Data.Entity.EntityState.Added;

                }

            }
            else
            {

                Rider NewRider = new Rider { userId = _Rider.userId, PercentKeen = _Rider.PercentKeen, Ride = _Rider.Ride, Name = _Rider.Name, LeaveTime = _Rider.LeaveTime };
                context.Riders.Add(NewRider);
                context.Entry(NewRider).State = System.Data.Entity.EntityState.Added;

            }

        }

        public void Save()
        {
            context.SaveChanges();
        }

        public Ride GetRideByID(int id)
        {
            return context.Rides.Where(r => r.id == id).FirstOrDefault();
        }

        public List<Rider> GetRidersForRide(int id)
        {
            return context.Riders.Where(r => r.Ride.id == id).ToList();
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

        public Ride GetNextRideForGroup(Group _Group)
        {
            
            Ride _Ride = context.Rides.Include("Riders").Where(t => t.Group.id == _Group.id && t.RideDate >= DateTime.Now).OrderBy(r => r.RideDate).FirstOrDefault();

            if (context.Rides.Where(t => t.Group.id == _Group.id && t.RideDate >= DateTime.Now).Count() == 1)
            {

                PopulateRideDatesFromDate(_Group, _Ride.RideDate); 

            }

            return _Ride; 
        }

        public Ride GetPreviousRideForGroup(Group _Group)
        {
            Ride _NextRide = context.Rides.Include("Riders").Where(t => t.Group.id == _Group.id && t.RideDate >= DateTime.Now).OrderBy(r => r.RideDate).FirstOrDefault();
            Ride PreviousRide = context.Rides.Include("Riders").Where(x => x.Group.id == _Group.id && x.RideDate <= _NextRide.RideDate).OrderBy(r => r.RideDate).FirstOrDefault();
            return PreviousRide;
        }


        public void AddRideComment(string Comment, int RideId, string UserName)
        {
          
            Comment _comment = new Comment { CommentText = Comment, Ride = context.Rides.Where(t => t.id == RideId).FirstOrDefault(),
                                              userName = UserName , Date = DateTime.Now };

            context.Comment.Add(_comment);
            context.Entry(_comment).State = System.Data.Entity.EntityState.Added;

        }

        public IEnumerable<Location> GetLocations()
        {
            return context.Locations.ToList();
        }


        public Member GetMemberByUserID(string id)
        {
            return context.Members.Where(m => m.userId == id).FirstOrDefault();
        }


        public List<Comment> GetCommentsForRide(int Rideid)
        {
            return context.Comment.Where(c => c.Ride.id == Rideid).ToList();
        }


        public void AddGroup(Group _Group)
        {
            context.Groups.Add(_Group);
            context.Entry(_Group).State = System.Data.Entity.EntityState.Added;
        }

        public void AddAdHocRide(Ad_HocRide _AdHocRide)
        {
            context.Ad_HocRide.Add(_AdHocRide);
            context.Entry(_AdHocRide).State = System.Data.Entity.EntityState.Added;
        }


        public void PopulateRideDatesFromDate(Group _Group, DateTime _DateTime)
        {
            List<DayOfWeek> RideDays = new List<DayOfWeek>();

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
                Ride NewRide = new Ride { Group = _Group, RideTime = _Group.RideTime, RideDate = nextdate };
                _Group.Rides.Add(NewRide);
            }

            context.Entry(_Group).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
        }

        public Group PopulateRideDates(Group _Group)
        {

            List<DayOfWeek> RideDays = new List<DayOfWeek>();

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

            foreach (DayOfWeek day in RideDays)
            {
                
                DateTime nextdate = GetNextDateForDay(DateTime.Now, day );
                Ride NewRide = new Ride { Group = _Group, RideTime = _Group.RideTime, RideDate = nextdate };
                _Group.Rides.Add(NewRide);
            }

            context.Entry(_Group).State = System.Data.Entity.EntityState.Modified;
            return _Group;

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

        public int GetUpCommingAd_HocCount(Location _Location)
        {
            return context.Ad_HocRide.Where(l => l.Location.id == _Location.id && l.RideDate >= DateTime.Now).Count();
        }


        public List<Ad_HocRide> GetAdHocRides(Location _Location)
        {
            return context.Ad_HocRide.Where(l => l.Location.id == _Location.id && l.RideDate >= DateTime.Now).ToList();          
        }


        public Ad_HocRide GetAdHocRideByID(int id)
        {
            Ad_HocRide Ad = new Ad_HocRide();

            Ad = context.Ad_HocRide.Where(i => i.id == id).FirstOrDefault();

            return Ad;
        }


        public List<AdHocComment> GetCommentsForAdHocRide(int AdHocRideid)
        {
            return context.AdHocComment.Where(r => r.AdHocRide.id == AdHocRideid).ToList();
        }

        public List<AdHocRider> GetRidersForAdHocRide(int AdHocRideid)
        {
            return context.AdHocRider.Where(r => r.AdHocRide.id == AdHocRideid).ToList();
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

                }

            }
            else
            {

                AdHocRider NewRider = new AdHocRider { userId = _Rider.userId, PercentKeen = _Rider.PercentKeen, AdHocRide = _Ride, Name = _Rider.Name, LeaveTime = _Rider.LeaveTime };
                context.AdHocRider.Add(NewRider);
                context.Entry(NewRider).State = System.Data.Entity.EntityState.Added;

            }

        }

        public void AddAdHocRideComment(string Comment, int RideId, string UserName)
        {
            AdHocComment _comment = new AdHocComment
            {
                CommentText = Comment,
                AdHocRide = context.Ad_HocRide.Where(t => t.id == RideId).FirstOrDefault(),
                userName = UserName,
                Date = DateTime.Now
            };

            context.AdHocComment.Add(_comment);
            context.Entry(_comment).State = System.Data.Entity.EntityState.Added;
        }
    }
}