using FreeWheeling.Domain.Abstract;
using FreeWheeling.Domain.Entities;
using FreeWheeling.UI.DataContexts;
using FreeWheeling.UI.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FreeWheeling.UI.Models
{
    public class RideModelHelper
    {
        private IdentityDb idb = new IdentityDb();
        private ICycleRepository repository;

        public RideModelHelper(ICycleRepository repoParam)
        {
            repository = repoParam;
        }

        public AdHocViewModel PopulateAdHocModel(int adhocrideid, string UserId)
        {
            Ad_HocRide Ah = repository.GetAdHocRideByID(adhocrideid);
            AdHocViewModel adHocViewModel = new AdHocViewModel { Ride = Ah, RideDate = Ah.RideDate, RideTime = Ah.RideTime };
            adHocViewModel.CommentCount = repository.GetCommentCountForAdHocRide(adhocrideid);
            adHocViewModel.IsOwner = repository.IsAdHocCreator(adhocrideid, UserId);

            if (adHocViewModel.MapUrl != null)
            {
                adHocViewModel.MapUrl =
                string.Concat("<iframe id=mapmyfitness_route src=https://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=http://veloroutes.org/k/%3Fr%3D", Ah.MapUrl, "&output=embed height=300px width=300px frameborder=0></iframe>");
                //https://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=http://veloroutes.org/k/%3Fr%3D108681
            }
            CultureHelper _CultureHelper = new CultureHelper(repository);
            TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(Ah.Location.id);
            adHocViewModel.Riders = repository.GetRidersForAdHocRide(adhocrideid, TZone);
            adHocViewModel.KeenCount = repository.GetKeenCountForAdHocRide(Ah.id);
            adHocViewModel.Comments = repository.GetTop2CommentsForAdHocRide(adhocrideid);

            return adHocViewModel;
        }

        public SingleRideViewModel PopulateSingleRideModel(int RideId, string UserId)
        {
            Ride _Ride = repository.GetRideByIDIncludeGroup(RideId);
            SingleRideViewModel _SingleRideViewModel = new SingleRideViewModel
            {
                Ride = _Ride,
                RideDate = _Ride.RideDate,
                RideTime = _Ride.RideTime
            };
            _SingleRideViewModel.CommentCount = repository.GetCommentCountForRide(RideId);
            _SingleRideViewModel.IsOwner = repository.IsGroupCreator(_Ride.Group.id,UserId);

            if (_SingleRideViewModel.MapUrl != null)
            {
                _SingleRideViewModel.MapUrl =
                string.Concat("<iframe id=mapmyfitness_route src=https://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=http://veloroutes.org/k/%3Fr%3D", _Ride.Group.MapUrl, "&output=embed height=300px width=300px frameborder=0></iframe>");
                //https://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=http://veloroutes.org/k/%3Fr%3D108681
            }
            CultureHelper _CultureHelper = new CultureHelper(repository);
            Group _Group = repository.GetGroupByID(_Ride.Group.id);
            TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(_Group.Location.id);        
            _SingleRideViewModel.Riders = repository.GetRidersForRide(RideId, TZone);
            _SingleRideViewModel.KeenCount = repository.GetKeenCountForRide(RideId);
            _SingleRideViewModel.Comments = repository.GetTop2CommentsForRide(RideId);

            return _SingleRideViewModel;
        }

        public RideModelIndex PopulateRideModel(int RideId, int GroupId, string UserId, bool NeedPreviousRide, bool FromFavPage)
        {
            Ride _Ride = new Ride();
            Group _Group = new Group();
            _Group = repository.GetGroupByID(GroupId);

            if(_Group == null)
            {

                _Group = repository.GetGroupByRideID(RideId);

            }

            CultureHelper _CultureHelper = new CultureHelper(repository);
            TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(_Group.Location.id);
            DateTime LocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZone);
            UserExpand _UserExpands = repository.GetUserExpandByUserID(UserId);

            if (RideId == -1) // this is the case when first enter view ride screen as rideid is unkown at that point.
            {
                _Ride = _Group.Rides.Where(u => u.RideDate.AddHours(2) >= LocalNow).OrderBy(i => i.RideDate).FirstOrDefault();
            }
            else
            {
                _Ride = repository.GetRideByID(RideId);            
            }
            
            RideModelIndex RideModel = new RideModelIndex();

            RideModel.Ride = _Ride;
            RideModel.RideDate = RideModel.Ride.RideDate; 
            RideModel.Group = _Group;
            RideModel.Comments = repository.GetTop2CommentsForRide(RideModel.Ride.id);
            RideModel.CommentCount = repository.GetCommentCountForRide(RideModel.Ride.id);
            RideModel.KeenCount = repository.GetKeenCountForRide(RideModel.Ride.id);
            RideModel.IsOwner = repository.IsGroupCreator(_Group.id, UserId);
            RideModel.FromFavPage = FromFavPage;
            RideModel.InFirst = repository.IsIn(RideModel.Ride.id,UserId);
            RideModel.OutFirst = repository.IsOut(RideModel.Ride.id, UserId);
            RideModel.OnWayFirst = repository.IsOnWay(RideModel.Ride.id, UserId);

            RideModel.NextRide = _Group.Rides.Where(u => u.RideDate > RideModel.Ride.RideDate).OrderBy(i => i.RideDate).FirstOrDefault();
            RideModel.NextRideDate = RideModel.NextRide.RideDate;
            RideModel.NextComments = repository.GetTop2CommentsForRide(RideModel.NextRide.id);
            RideModel.NextCommentCount = repository.GetCommentCountForRide(RideModel.NextRide.id);
            RideModel.NextKeenCount = repository.GetKeenCountForRide(RideModel.NextRide.id);
            RideModel.NextRiders = repository.GetRidersForRide(RideModel.NextRide.id, TZone);
            RideModel.InSecond = repository.IsIn(RideModel.NextRide.id, UserId);
            RideModel.OutSecond = repository.IsOut(RideModel.NextRide.id, UserId);
            RideModel.OnWaySecond = repository.IsOnWay(RideModel.NextRide.id, UserId);
            
            if (_UserExpands != null)
            {
                RideModel.FirstKeen = _UserExpands.FirstKeen;
                RideModel.FirstBunch = _UserExpands.FirstBunch;
                RideModel.FirstComment = _UserExpands.FirstComment;
                RideModel.SecondBunch = _UserExpands.SecondBunch;
                RideModel.SecondKeen = _UserExpands.SecondKeen;
                RideModel.SecondComment = _UserExpands.SecondComment;
            }

            if (RideModel.Group.MapUrl != null)
            {
                RideModel.MapUrl = RideModel.MapUrl = string.Concat("<iframe id=mapmyfitness_route src=https://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=http://veloroutes.org/k/%3Fr%3D", RideModel.Group.MapUrl, "&output=embed height=300px width=300px frameborder=0></iframe>");
            }

            RideModel.Riders = repository.GetRidersForRide(RideModel.Ride.id, TZone);
            return (RideModel);
        }
    }

    public class RideModelIndex
    {
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime RideDate { get; set; }
        public string RideTime { get; set; }
        public Group Group { get; set; }
        public Ride Ride { get; set; }
        public List<Rider> Riders { get; set; } 
        public List<Route> Routes { get; set; }
        public List<Comment> Comments { get; set; }
        public int CommentCount { get; set; }
        public int KeenCount { get; set; }
        public Boolean IsOwner { get; set; }
        public string MapUrl { get; set; }
        public bool FromFavPage { get; set; }

        //Expand items
        public Boolean FirstBunch { get; set; }
        public Boolean FirstKeen { get; set; }
        public Boolean FirstComment { get; set; }
        public Boolean SecondBunch { get; set; }
        public Boolean SecondKeen { get; set; }
        public Boolean SecondComment { get; set; }

        //Next ride
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime NextRideDate { get; set; }
        public string NextRideTime { get; set; }
        public Ride NextRide { get; set; }
        public List<Rider> NextRiders { get; set; }
        public List<Route> NextRoutes { get; set; }
        public List<Comment> NextComments { get; set; }
        public int NextCommentCount { get; set; }
        public int NextKeenCount { get; set; }

        //Checks
        public bool InFirst { get; set; }
        public bool OutFirst { get; set; }
        public bool OnWayFirst { get; set; }
        public bool InSecond { get; set; }
        public bool OutSecond { get; set; }
        public bool OnWaySecond { get; set; }
    }

    public class RideCommentModel
    {
        public int RideId { get; set; }
        public Ride Ride { get; set; }
        public string Comment { get; set; }
        public int GroupId { get; set; }
        public Ride NextRide { get; set; }
        public Ride PreviousRide { get; set; }
    }

    public class AdHocViewModel
    {
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime RideDate { get; set; }
        public string RideTime { get; set; }
        public Ad_HocRide Ride { get; set; }
        public List<AdHocRider> Riders { get; set; }
        public List<Route> Routes { get; set; }
        public List<AdHocComment> Comments { get; set; }
        public int CommentCount { get; set; }
        public Boolean IsOwner { get; set; }
        public string MapUrl { get; set; }
        public int KeenCount { get; set; }
    }

    public class SingleRideViewModel
    {
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime RideDate { get; set; }
        public string RideTime { get; set; }
        public Ride Ride { get; set; }
        public List<Rider> Riders { get; set; }
        public List<Route> Routes { get; set; }
        public List<Comment> Comments { get; set; }
        public int CommentCount { get; set; }
        public Boolean IsOwner { get; set; }
        public string MapUrl { get; set; }
        public int KeenCount { get; set; }
    }

    public class AdHocRideCommentModel
    {
        public int adhocrideid { get; set; }
        public Ad_HocRide Ride { get; set; }
        public string Comment { get; set; }
    }

    public class AllRideComments
    {
        public List<Comment> Comments { get; set; }
        public int RideId { get; set; }
        public int GroupId { get; set; }
    }

    public class AllAdHocRideComments
    {
        public List<AdHocComment> Comments { get; set; }
        public int adhocrideid { get; set; }
    }

    public class InviteOthersToBunchModel
    {
        public int RideId { get; set; }
        public string Name { get; set; }
        public string RideDate { get; set; }
        public List<InviteUser> InviteUsers { get; set; }
    }

    public class InviteOthersToAdHocBunchModel
    {
       public int adhocrideid { get; set; }
       public string Name { get; set; }
       public string RideDate { get; set; }
       public List<InviteUser> InviteUsers { get; set; }
    }

    public class EditAdHocRideModel
    {

        public string Name { get; set; }
        public DateTime RideDate { get; set; }
        public string RideTime { get; set; }
        public List<Location> Locations { get; set; }
        public string DateString { get; set; }
        [Required(ErrorMessage = "Start location is required")]
        public string StartLocation { get; set; }
        public string AverageSpeed { get; set; }
        [Required(ErrorMessage = "Hour is required")]
        [Range(0, 24, ErrorMessage = "Between 0 and 24")]
        public int RideHour { get; set; }
        [Required(ErrorMessage = "Minute is required")]
        [Range(0, 60, ErrorMessage = "Between 0 and 60")]
        public int RideMinute { get; set; }
        public int LocationsId { get; set; }
        public int adhocrideid { get; set; }
        public string Description { get; set; }
        public string MapUrl { get; set; }

    }

    public class DeleteAdHocRideModel
    {
        public int AdHocId { get; set; }
        public string Name { get; set; }
    }
}