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
            return context.Groups.Include("Members").Include("Rides").Include("Location").ToList(); 
        }

        public IEnumerable<Group> GetGroupsByLocation(int LocationID)
        {
            return context.Groups.Include("Members").Include("Rides").Include("Location").Where(g => g.Location.id == LocationID).ToList();
        }

        public Group GetGroupByID(int id)
        {
            Group group = context.Groups.Include("Members").Include("Rides").Include("Location").Where(i => i.id == id).FirstOrDefault();

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
            //Group _group = context.Groups.Include("Rides").Where(t => t.id == _Group.id).FirstOrDefault();
            Ride _Ride = context.Rides.Include("Riders").Where(t => t.Group.id == _Group.id && t.RideDate >= DateTime.Now).OrderBy(r => r.RideDate).FirstOrDefault();
            return _Ride; 
        }


        public void AddRideComment(string Comment, int RideId, string UserId)
        {
            Comment _comment = new Comment { CommentText = Comment, Ride = context.Rides.Where(t => t.id == RideId).FirstOrDefault(),
                                             Rider = context.Riders.Where(e => e.userId == UserId).FirstOrDefault(), Date = DateTime.Now };

            context.Comment.Add(_comment);
            context.Entry(_comment).State = System.Data.Entity.EntityState.Added;

        }

        public IEnumerable<Location> GetLocations()
        {
            return context.Locations.ToList();
        }


        public void SetMemberLocation(string UserId, int Locationid)
        {

            Member _Member = context.Members.Where(i => i.userId == UserId).FirstOrDefault();
            Location _Location = context.Locations.Where(l => l.id == Locationid).FirstOrDefault();

            _Member.Location = _Location;
            context.Entry(_Member).State = System.Data.Entity.EntityState.Modified;


        }


        public Location GetMemberLocation(string UserId)
        {
            Member _Member = context.Members.Include("Location").Where(i => i.userId == UserId).FirstOrDefault();
            
            return _Member.Location;
        }


        public Member GetMemberByUserID(string id)
        {
            return context.Members.Include("Location").Where(m => m.userId == id).FirstOrDefault();
        }
    }
}