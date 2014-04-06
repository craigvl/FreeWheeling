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


namespace FreeWheeling.ConsoleApp
{
    public class bunchyConsole
    {
        private static CycleRepository _CycleRepository;

        static void Main()    
        {
            _CycleRepository = new CycleRepository();
            PopulateHomePageRide();
            //Console.ReadLine();
        }

        private static void PopulateHomePageRide()
        {

          List<ListOfRides> _ListOfRides = new List<ListOfRides>();
          CultureHelper _CultureHelper = new CultureHelper(_CycleRepository);

          foreach (Member item in _CycleRepository.GetMembersWithGroups().ToList())
                {
                    Group _Group = _CycleRepository.GetGroupByID(item.Group.id);
 
                    Location _Location = _CycleRepository.GetLocations().Where(l => l.id == _Group.Location.id).FirstOrDefault();
                    
                    TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(_Location.id);

                    Ride _ClosestRide = _CycleRepository.GetClosestNextRide(item.Group,TZone);

                    _ListOfRides.Add(new ListOfRides { RideId = _ClosestRide.id, userId = item.userId, RideDate = _ClosestRide.RideDate });
                }

                foreach (Ride _Ride in _CycleRepository.GetRidesWithRiders())
                {
                    CycleRepository _CycleRepository1 = new CycleRepository();
                    foreach (Rider _Rider in _Ride.Riders)
                    {
                        
                        if (_CycleRepository1.IsOnWay(_Ride.id,_Rider.userId))
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

                foreach (ListOfRides item in _ListOfRides)
                {
                   // Console.WriteLine(item.RideId + ", " + item.RideDate + ", " + item.userId);                 
                }

        }

    }

    public class ListOfRides
    {

        public int RideId { get; set; }
        public string userId {get;set;}
        public DateTime RideDate { get; set; }


    }
}
