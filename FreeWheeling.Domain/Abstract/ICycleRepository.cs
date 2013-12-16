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

        Member GetMemberByUserID(string id);
        string GetLocationName(int? id);
        List<int> CurrentGroupsForUser(string UserId);

        //Member
        void AddMember(string UserId, Group _Group);

        void RemoveMember(string UserId, Group _Group);

        void AddRider(Rider _Rider, Group _Group);

        void AddRideComment(string Comment, int RideId, string UserId);

        void Save();
      
    }
}
