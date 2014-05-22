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

        public SingleRideAndRandomRideViewModel PopulateAdHocModel(int adhocrideid, string UserId)
        {
            Ad_HocRide Ah = repository.GetAdHocRideByID(adhocrideid);
            SingleRideAndRandomRideViewModel _SingleRideRandomRideViewModel = new SingleRideAndRandomRideViewModel { RandomRide = Ah, RideDate = Ah.RideDate, RideTime = Ah.RideTime, MapUrl = Ah.MapUrl };
            _SingleRideRandomRideViewModel.CommentCount = repository.GetCommentCountForAdHocRide(adhocrideid);
            _SingleRideRandomRideViewModel.IsOwner = repository.IsAdHocCreator(adhocrideid, UserId);

            if (_SingleRideRandomRideViewModel.MapUrl != null)
            {
                _SingleRideRandomRideViewModel.MapUrl =
                string.Concat("<iframe id=mapmyfitness_route src=https://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=http://veloroutes.org/k/%3Fr%3D", Ah.MapUrl, "&output=embed height=300px width=300px frameborder=0></iframe>");
                //https://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=http://veloroutes.org/k/%3Fr%3D108681
            }
            CultureHelper _CultureHelper = new CultureHelper(repository);
            TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(Ah.Location.id);
            _SingleRideRandomRideViewModel.RandomRiders = repository.GetRidersForAdHocRide(adhocrideid, TZone);
            _SingleRideRandomRideViewModel.KeenCount = repository.GetKeenCountForAdHocRide(Ah.id);
            _SingleRideRandomRideViewModel.RandomComments = repository.GetTop2CommentsForAdHocRide(adhocrideid);

            return _SingleRideRandomRideViewModel;
        }

        public SingleRideAndRandomRideViewModel PopulateSingleRideModel(int RideId, string UserId)
        {
            Ride _Ride = repository.GetRideByIDIncludeGroup(RideId);
            SingleRideAndRandomRideViewModel _SingleRideRandomRideViewModel = new SingleRideAndRandomRideViewModel
            {
                Ride = _Ride,
                RideDate = _Ride.RideDate,
                RideTime = _Ride.RideTime,
                MapUrl = _Ride.Group.MapUrl
            };
            _SingleRideRandomRideViewModel.CommentCount = repository.GetCommentCountForRide(RideId);
            _SingleRideRandomRideViewModel.IsOwner = repository.IsGroupCreator(_Ride.Group.id, UserId);

            if (_SingleRideRandomRideViewModel.MapUrl != null)
            {
                _SingleRideRandomRideViewModel.MapUrl =
                string.Concat("<iframe id=mapmyfitness_route src=https://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=http://veloroutes.org/k/%3Fr%3D", _Ride.Group.MapUrl, "&output=embed height=300px width=300px frameborder=0></iframe>");
                //https://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=http://veloroutes.org/k/%3Fr%3D108681
            }
            CultureHelper _CultureHelper = new CultureHelper(repository);
            Group _Group = repository.GetGroupByID(_Ride.Group.id);
            TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(_Group.Location.id);
            Ride ClosestRide = repository.GetClosestNextRide(_Ride.Group, TZone);
            _SingleRideRandomRideViewModel.PusherChannel = ClosestRide.id;
            _SingleRideRandomRideViewModel.Riders = repository.GetRidersForRide(RideId, TZone);
            _SingleRideRandomRideViewModel.KeenCount = repository.GetKeenCountForRide(RideId);
            _SingleRideRandomRideViewModel.Comments = repository.GetTop2CommentsForRide(RideId);

            return _SingleRideRandomRideViewModel;
        }

        public RideModelIndex PopulateRideModel(int RideId, int GroupId, string UserId, bool NeedPreviousRide)
        {
            string ChevronClassDown = "glyphicon glyphicon glyphicon-minus";
            string ChevronClassUp = "glyphicon glyphicon glyphicon-plus";

            string PanelClassDown = "panel-collapse collapse in";
            string PanelClassUp = "panel-collapse collapse";

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
                _Ride = repository.GetClosestNextRide(_Group, TZone);
            }
            
            RideModelIndex RideModel = new RideModelIndex();

            RideModel.Ride = _Ride;
            RideModel.RideDate = RideModel.Ride.RideDate; 
            RideModel.Group = _Group;
            RideModel.Comments = repository.GetTop2CommentsForRide(RideModel.Ride.id);
            RideModel.CommentCount = repository.GetCommentCountForRide(RideModel.Ride.id);
            RideModel.KeenCount = repository.GetKeenCountForRide(RideModel.Ride.id);
            RideModel.IsOwner = repository.IsGroupCreator(_Group.id, UserId);
            RideModel.InFirst = repository.IsIn(RideModel.Ride.id,UserId);
            RideModel.OutFirst = repository.IsOut(RideModel.Ride.id, UserId);
            RideModel.OnWayFirst = repository.IsOnWay(RideModel.Ride.id, UserId);

            RideModel.NextRide = _Group.Rides.Where(u => u.RideDate > RideModel.Ride.RideDate).OrderBy(i => i.RideDate).FirstOrDefault();
            if (RideModel.NextRide == null)
            {
                //If no next ride need to poulate from the latest date, will catch some issues.
                repository.PopulateRideDatesFromDate(_Group,RideModel.RideDate,TZone);
                RideModel.NextRide = _Group.Rides.Where(u => u.RideDate > RideModel.Ride.RideDate).OrderBy(i => i.RideDate).FirstOrDefault();
            }
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
                RideModel.FirstBunch = _UserExpands.FirstBunch;
                if (RideModel.FirstBunch)
                {
                    RideModel.FirstBunchChevronClass = ChevronClassDown;
                    RideModel.FirstBunchPanelClass = PanelClassDown;
                }
                else
                {
                    RideModel.FirstBunchChevronClass = ChevronClassUp;
                    RideModel.FirstBunchPanelClass = PanelClassUp;
                }
               
                RideModel.FirstKeen = _UserExpands.FirstKeen;
                if (RideModel.FirstKeen)
                {
                    RideModel.FirstKeenChevronClass = ChevronClassDown;
                    RideModel.FirstKeenPanelClass = PanelClassDown;
                }
                else
                {
                    RideModel.FirstKeenChevronClass = ChevronClassUp;
                    RideModel.FirstKeenPanelClass = PanelClassUp;
                }

                RideModel.FirstComment = _UserExpands.FirstComment;
                if (RideModel.FirstComment)
                {
                    RideModel.FirstCommentChevronClass = ChevronClassDown;
                    RideModel.FirstCommentPanelClass = PanelClassDown;
                }
                else
                {
                    RideModel.FirstCommentChevronClass = ChevronClassUp;
                    RideModel.FirstCommentPanelClass = PanelClassUp;
                }

                RideModel.SecondBunch = _UserExpands.SecondBunch;
                if (RideModel.SecondBunch)
                {
                    RideModel.SecondBunchChevronClass = ChevronClassDown;
                    RideModel.SecondBunchPanelClass = PanelClassDown;
                }
                else
                {
                    RideModel.SecondBunchChevronClass = ChevronClassUp;
                    RideModel.SecondBunchPanelClass = PanelClassUp;
                }
                RideModel.SecondKeen = _UserExpands.SecondKeen;
                if (RideModel.SecondKeen)
                {
                    RideModel.SecondKeenChevronClass = ChevronClassDown;
                    RideModel.SecondKeenPanelClass = PanelClassDown;
                }
                else
                {
                    RideModel.SecondKeenChevronClass = ChevronClassUp;
                    RideModel.SecondKeenPanelClass = PanelClassUp;
                }

                RideModel.SecondComment = _UserExpands.SecondComment;
                if (RideModel.SecondComment)
                {
                    RideModel.SecondCommentChevronClass = ChevronClassDown;
                    RideModel.SecondCommentPanelClass = PanelClassDown;
                }
                else
                {
                    RideModel.SecondCommentChevronClass = ChevronClassUp;
                    RideModel.SecondCommentPanelClass = PanelClassUp;
                }
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
        public string FromHome { get; set; }

        //Expand items
        public Boolean FirstBunch { get; set; }
        public String  FirstBunchChevronClass { get; set; }
        public String  FirstBunchPanelClass { get; set; }
        public Boolean FirstKeen { get; set; }
        public String  FirstKeenChevronClass { get; set; }
        public String  FirstKeenPanelClass { get; set; }
        public Boolean FirstComment { get; set; }
        public String  FirstCommentChevronClass { get; set; }
        public String  FirstCommentPanelClass { get; set; }
        public Boolean SecondBunch { get; set; }
        public String  SecondBunchChevronClass { get; set; }
        public String  SecondBunchPanelClass { get; set; }
        public Boolean SecondKeen { get; set; }
        public String  SecondKeenChevronClass { get; set; }
        public String  SecondKeenPanelClass { get; set; }
        public Boolean SecondComment { get; set; }
        public String  SecondCommentChevronClass { get; set; }
        public String  SecondCommentPanelClass { get; set; }

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

    public class SingleRideAndRandomRideViewModel
    {
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime RideDate { get; set; }
        public string RideTime { get; set; }
        public Ad_HocRide RandomRide { get; set; }
        public Ride Ride { get; set; }
        public List<AdHocRider> RandomRiders { get; set; }
        public List<Rider> Riders { get; set; }
        public List<Route> Routes { get; set; }
        public List<AdHocComment> RandomComments { get; set; }
        public List<Comment> Comments { get; set; }
        public int CommentCount { get; set; }
        public Boolean IsOwner { get; set; }
        public string MapUrl { get; set; }
        public int KeenCount { get; set; }
        public int PusherChannel { get; set; }
        public string FromHome { get; set; }
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
        public int PreviousID { get; set; }
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
        public int PreviousID { get; set; }
        public List<InviteUser> InviteUsers { get; set; }
    }

    public class InviteOthersToAdHocBunchModel
    {
       public int adhocrideid { get; set; }
       public string Name { get; set; }
       public string RideDate { get; set; }
       public List<InviteUser> InviteUsers { get; set; }
       public bool IsPrivate { get; set; }
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
        public bool IsPrivate { get; set; }
    }

    public class DeleteAdHocRideModel
    {
        public int AdHocId { get; set; }
        public string Name { get; set; }
    }
}