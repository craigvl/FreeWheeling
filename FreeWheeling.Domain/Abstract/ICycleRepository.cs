using FreeWheeling.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheeling.Domain.Abstract
{
    public interface ICycleRepository
    {

        //Group
        IEnumerable<Group> GetGroups();
        IEnumerable<Group> GetGroupsByLocation(int? LocationID);
        IEnumerable<Group> GetGroupsWithRiders();
        IEnumerable<Location> GetLocations();

        Group GetGroupByID(int id);
        Ride GetRideByID(int id);
        List<Rider> GetRidersForRide(int id);
        Ride GetNextRideForGroup(Group _Group);
        Ride GetPreviousRideForGroup(Group _Group);
        List<Comment> GetCommentsForRide(int Rideid);

        List<Ad_HocRide> GetAdHocRides(Location _Location);

        Member GetMemberByUserID(string id);
        string GetLocationName(int? id);
        List<int> CurrentGroupsForUser(string UserId);
        int GetUpCommingAd_HocCount(Location _Location);

        //Member
        void AddMember(string UserId, Group _Group);

        void RemoveMember(string UserId, Group _Group);

        Group PopulateRideDates(Group _Group);

        void PopulateRideDatesFromDate(Group _Group, DateTime _DateTime);

        void AddRider(Rider _Rider, Group _Group);

        void AddGroup(Group _Group);

        void AddAdHocRide(Ad_HocRide _AdHocRide);

        void AddRideComment(string Comment, int RideId, string UserName);

        void Save();
      
    }
}
