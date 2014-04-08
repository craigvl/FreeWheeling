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

        //Get .. Group
        IEnumerable<Group> GetGroups();
        IEnumerable<Group> GetGroupsByLocation(int? LocationID);
        IEnumerable<Group> GetGroupsByLocationWithSearch(int? LocationID, string SearchString);
        IEnumerable<Group> GetFavouriteGroupsByLocation(int? LocationID, string CurrentUserId);
        IEnumerable<Group> GetFavouriteGroupsByLocationWithSearch(int? LocationID, string SearchString, string CurrentUserId);
        IEnumerable<Group> GetGroupsWithRiders();
        Group GetGroupByID(int id);
        Group GetGroupByRideID(int Rideid);
        Group GetGroupByIDNoIncludes(int id);
        List<CycleDays> GetCycleDaysForGroup(int GroupId);
        //Ride GetPreviousRideForGroup(Group _Group);
        List<int> CurrentGroupsForUser(string UserId);

        //Get .. Location
        IEnumerable<Location> GetLocations();
        string GetLocationName(int? id);

        //Get .. Ride
        IEnumerable<Ride> GetRides();
        IEnumerable<Ride> GetRidesWithRiders();
        Ride GetRideByID(int id);
        Ride GetRideByIDIncludeGroup(int id);
        //Ride GetNextRideForGroup(Group _Group, TimeZoneInfo TimeZone);
        Ride GetClosestNextRide(Group _Group, TimeZoneInfo TimeZone);
        Ride Get2ndClosestNextRide(Group _Group, TimeZoneInfo TimeZone);
        List<Ad_HocRide> GetAdHocRides(Location _Location, TimeZoneInfo TimeZone);

        //Get .. AdHoc
        Ad_HocRide GetAdHocRideByID(int id);
        List<AdHocComment> GetTop2CommentsForAdHocRide(int AdHocRideid);
        List<AdHocComment> GetAllCommentsForAdHocRide(int AdHocRideid);
        int GetCommentCountForAdHocRide(int AdHocRideid);
        int GetUpCommingAd_HocCount(Location _Location, TimeZoneInfo TimeZone);

        //Get .. Riders
        IEnumerable<Rider> GetRiders();
        List<Rider> GetRidersForRide(int id, TimeZoneInfo TimeZone);
        List<AdHocRider> GetRidersForAdHocRide(int AdHocRideid, TimeZoneInfo TimeZone);
        int GetKeenCountForRide(int Rideid);
        int GetKeenCountForAdHocRide(int AdHocRideid);
        List<Rider> GetRidersForRideDontIncludeCurrentUser(int id, TimeZoneInfo TimeZone, string CurrentUserId);
        List<Rider> GetRidersAndCommentersForRideDontIncludeCurrentUser(int id, TimeZoneInfo TimeZone, string CurrentUserId);
        List<AdHocRider> GetRidersForAdHocRideDontIncludeCurrentUser(int AdHocRideid, TimeZoneInfo TimeZone, string CurrentUserId);
        List<AdHocRider> GetRidersAndCommentersForAdHocRideDontIncludeCurrentUser(int AdHocRideid, TimeZoneInfo TimeZone, string CurrentUserId);

        //Get .. Comments
        List<Comment> GetTop2CommentsForRide(int Rideid);
        int GetCommentCountForRide(int Rideid);
        List<Comment> GetAllCommentsForRide(int Rideid);

        //Get .. Members
        IEnumerable<Member> GetMembersWithGroups();
        Member GetMemberByUserID(string id);
        
        //Get .. UserExpand
        UserExpand GetUserExpandByUserID(string UserId);
        
        //Add
        void AddMember(string UserId, Group _Group);
        void AddRider(Rider _Rider, Group _Group);
        void AddAdHocRider(AdHocRider _Rider, Ad_HocRide _Ride);
        void AddGroup(Group _Group);
        void AddAdHocRide(Ad_HocRide _AdHocRide);
        void AddRideComment(string Comment, int RideId, string UserName, string UserId);
        void AddAdHocRideComment(string Comment, int RideId, string UserName, string UserId);
        void AddUserExpand(UserExpand _UserExpand);

        //Delete
        void RemoveMember(string UserId, Group _Group);
        void DeleteGroup(int GroupId);
        void DeleteOldRides(int GroupId, TimeZoneInfo TimeZone);
        void DeleteAdHocRide(int AdHocId);
        
        //Update
        void UpdateGroup(Group _Group);
        void UpdateRideTimes(Group _Group, TimeZoneInfo TimeZone);
        void UpdateAdHocRide(Ad_HocRide _AdHocRide);
        void UpdateUserExpand(UserExpand _UserExpand);

        //Checks
        Boolean IsAdHocCreator(int AdHocRideid, string UserId);
        Boolean IsGroupCreator(int _GroupId, string UserId);
        Boolean IsIn(int RideId, string UserId);
        Boolean IsOut(int RideId, string UserId);
        Boolean IsOnWay(int RideId, string UserId);
        
        //Populate
        Group PopulateRideDates(Group _Group, TimeZoneInfo _TimeZoneInfo);
        void PopulateRideDatesFromDate(Group _Group, DateTime _DateTime, TimeZoneInfo _TimeZoneInfo);
        void PopulateUserHomePageRides(List<HomePageRide> _HomePageRides);

        void Save();
      
    }
}
