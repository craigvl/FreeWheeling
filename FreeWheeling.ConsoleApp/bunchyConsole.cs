using FreeWheeling.Domain.Entities;
using FreeWheeling.Domain.DataContexts;
using FreeWheeling.Domain.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeWheeling.UI.Infrastructure;
using FreeWheeling.UI.DataContexts;
using Microsoft.AspNet.Identity.EntityFramework;
using FreeWheeling.UI.Models;


namespace FreeWheeling.ConsoleApp
{
    public class bunchyConsole
    {
        private static CycleRepository _CycleRepository;
        private static IdentityDb _idb;

        static void Main()    
        {
            _CycleRepository = new CycleRepository();
            _idb = new IdentityDb();
            DeleteOldRidesAndCreateNew();
            PopulateHomePageRide();
            CheckUserExpands(_idb);
        }

        private static void CheckUserExpands(IdentityDb context)
        {
            int Totalusers = _idb.Users.Count();
            int TotalExpandConfig = _CycleRepository.GetUserExpandCount();

            if (Totalusers != TotalExpandConfig)
            {
                foreach (var item in _idb.Users)
                {
                    _CycleRepository.PopulateInitialExpandValues(item.Id.ToString());
                }
            }

        }

        private static void PopulateHomePageRide()
        {
            _CycleRepository = new CycleRepository();
            List<ListOfRides> _ListOfRides = new List<ListOfRides>();
            CultureHelper _CultureHelper = new CultureHelper(_CycleRepository);

            foreach (Member item in _CycleRepository.GetMembersWithGroupsIncludePrivate().ToList())
            {
                Group _Group = _CycleRepository.GetGroupByID(item.Group.id);
                Location _Location = _CycleRepository.GetLocations().Where(l => l.id == _Group.Location.id).FirstOrDefault();
                TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(_Location.id);
                Ride _ClosestRide = _CycleRepository.GetClosestNextRide(item.Group, TZone);
                _ListOfRides.Add(new ListOfRides { RideId = _ClosestRide.id, userId = item.userId, RideDate = _ClosestRide.RideDate });
            }

            foreach (Ride _Ride in _CycleRepository.GetRidesWithRiders())
            {
                CycleRepository _CycleRepository1 = new CycleRepository();
                foreach (Rider _Rider in _Ride.Riders)
                {
                    if (_CycleRepository1.IsOnWay(_Ride.id, _Rider.userId))
                    {
                        _ListOfRides.Add(new ListOfRides { RideId = _Ride.id, userId = _Rider.userId, RideDate = _Ride.RideDate });
                    }

                    if (_CycleRepository1.IsIn(_Ride.id, _Rider.userId))
                    {
                        _ListOfRides.Add(new ListOfRides { RideId = _Ride.id, userId = _Rider.userId, RideDate = _Ride.RideDate });
                    }
                }
            }

            _ListOfRides = _ListOfRides.Distinct().ToList();
            _ListOfRides = _ListOfRides.GroupBy(x => x.userId).Select(x => x.OrderBy(y => y.RideDate)).Select(x => x.First()).ToList();
            List<HomePageRide> _HomePageRide = new List<HomePageRide>();

            foreach (ListOfRides item in _ListOfRides)
            {
                _HomePageRide.Add(new HomePageRide { Rideid = item.RideId, Userid = item.userId, IsRandomRide = item.IsRandomRide });
            }

            _CycleRepository.PopulateUserHomePageRides(_HomePageRide);
        }

        private static void DeleteOldRidesAndCreateNew()
        {
            CultureHelper _CultureHelper = new CultureHelper(_CycleRepository);
            foreach (Group item in _CycleRepository.GetGroupsIncludePrivate())
            {
                Location _Location = _CycleRepository.GetLocations().Where(l => l.id == item.Location.id).FirstOrDefault();
                TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(_Location.id);
                _CycleRepository.DeleteOldRides(item.id, TZone);
                _CycleRepository.Save();
            }

            foreach (Group item in _CycleRepository.GetGroupsIncludePrivate())
            {
                Location _Location = _CycleRepository.GetLocations().Where(l => l.id == item.Location.id).FirstOrDefault();
                TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(_Location.id);
                Ride NextRide = _CycleRepository.GetClosestNextRide(item, TZone);
                if (NextRide == null)
                {
                    if (item.RideDays != null)
                    {
                        _CycleRepository.PopulateRideDates(item, TZone);
                        _CycleRepository.Save();
                    }
                }
            }          
        }
    }

    public class ListOfRides
    {
        public int RideId { get; set; }
        public string userId {get;set;}
        public DateTime RideDate { get; set; }
        public bool IsRandomRide { get; set; }
    }
}
