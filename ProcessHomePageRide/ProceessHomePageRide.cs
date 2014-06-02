using Microsoft.WindowsAzure.Jobs;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using FreeWheeling.Domain.Entities;
using FreeWheeling.Domain.DataContexts;
using FreeWheeling.Domain.Concrete;
using FreeWheeling.UI.Infrastructure;

namespace ProcessHomePageRide
{
    public class ProceessHomePageRide
    {
        private static CycleRepository _CycleRepository;

        static void Main()
        {
            JobHost host = new JobHost();
            host.RunAndBlock();
        }

        public static void ProcessQueueMessage([QueueInput("updatehomepage")] string inputText,
                                      [BlobOutput("containername/blobname")]TextWriter writer)
        {
            PopulateHomePageRide();
            writer.WriteLine(inputText);
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

            foreach (Ad_HocRide _RandomRide in _CycleRepository.GetRandomRidesWithRiders())
            {
                CycleRepository _CycleRepository2 = new CycleRepository();
                foreach (AdHocRider _Rider in _RandomRide.Riders)
                {
                    if (_CycleRepository2.IsOnWayRandom(_RandomRide.id, _Rider.userId))
                    {
                        _ListOfRides.Add(new ListOfRides { RideId = _RandomRide.id, userId = _Rider.userId, RideDate = _RandomRide.RideDate, IsRandomRide = true  });
                    }

                    if (_CycleRepository2.IsInRandom(_RandomRide.id, _Rider.userId))
                    {
                        _ListOfRides.Add(new ListOfRides { RideId = _RandomRide.id, userId = _Rider.userId, RideDate = _RandomRide.RideDate, IsRandomRide = true });
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

        public class ListOfRides
        {
            public int RideId { get; set; }
            public string userId { get; set; }
            public DateTime RideDate { get; set; }
            public bool IsRandomRide { get; set; }
        }

    }
}
