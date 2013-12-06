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
            return context.Groups.Include("Members").Include("Rides").ToList(); 
        }

        public Group GetGroupByID(int id)
        {
            Group group = context.Groups.Include("Members").Include("Rides").Where(i => i.id == id).FirstOrDefault();

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

        public void AddRider(string UserId, string RiderName, Ride _Ride, Group _Group, string Percent)
        {
            Rider CurrentRiders = context.Rides.Where(r => r.Group.id == _Group.id && r.Riders.Any(t => t.userId == UserId)).Select(y => y.Riders.FirstOrDefault()).FirstOrDefault();

            if (CurrentRiders != null)
            {

                if (CurrentRiders.id != 0)
                {
                    CurrentRiders.PercentKeen = Percent;
                    CurrentRiders.Name = RiderName;
                    context.Riders.Attach(CurrentRiders);
                    context.Entry(CurrentRiders).State = System.Data.Entity.EntityState.Modified; 
                }
                else
                {

                    Rider NewRider = new Rider { userId = UserId, PercentKeen = Percent, Ride = _Ride, Name = RiderName };
                    context.Riders.Add(NewRider);
                    context.Entry(NewRider).State = System.Data.Entity.EntityState.Added;

                }

            }
            else
            {

                Rider NewRider = new Rider { userId = UserId, PercentKeen = Percent, Ride = _Ride, Name = RiderName };
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
    }
}
