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
        Ad_HocRide GetAdHocRideByID(int id);
        List<AdHocComment> GetTop2CommentsForAdHocRide(int AdHocRideid);
        List<AdHocComment> GetAllCommentsForAdHocRide(int AdHocRideid);
        int GetCommentCountForAdHocRide(int AdHocRideid);
        List<AdHocRider> GetRidersForAdHocRide(int AdHocRideid, TimeZoneInfo TimeZone);

        List<Rider> GetRidersForRide(int id, TimeZoneInfo TimeZone);
        Ride GetNextRideForGroup(Group _Group, TimeZoneInfo TimeZone);
        Ride GetPreviousRideForGroup(Group _Group);
        List<Comment> GetTop2CommentsForRide(int Rideid);
        int GetCommentCountForRide(int Rideid);
        List<Comment> GetAllCommentsForRide(int Rideid);

        List<Ad_HocRide> GetAdHocRides(Location _Location, TimeZoneInfo TimeZone);

        Member GetMemberByUserID(string id);
        int GetUpCommingAd_HocCount(Location _Location, TimeZoneInfo TimeZone);
        string GetLocationName(int? id);


        List<int> CurrentGroupsForUser(string UserId);

        Boolean IsAdHocCreator(int AdHocRideid, string UserId);
        Boolean IsGroupCreator(int _GroupId, string UserId);
        
        //Member
        void AddMember(string UserId, Group _Group);

        void RemoveMember(string UserId, Group _Group);

        Group PopulateRideDates(Group _Group);

        void PopulateRideDatesFromDate(Group _Group, DateTime _DateTime);

        void AddRider(Rider _Rider, Group _Group);
        void AddAdHocRider(AdHocRider _Rider, Ad_HocRide _Ride);


        void AddGroup(Group _Group);
        void UpdateGroup(Group _Group);
        void UpdateRideTimes(Group _Group);

        void AddAdHocRide(Ad_HocRide _AdHocRide);
        void UpdateAdHocRide(Ad_HocRide _AdHocRide);

        void AddRideComment(string Comment, int RideId, string UserName);
        void AddAdHocRideComment(string Comment, int RideId, string UserName);

        void Save();
      
    }
}
