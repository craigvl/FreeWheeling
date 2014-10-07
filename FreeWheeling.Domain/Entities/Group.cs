using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheeling.Domain.Entities
{
    public class Group
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool IsPrivate {get;set;}
        public Location Location { get; set; }
        public List<Member> Members { get; set; }
        public List<Ride> Rides { get; set; }
        public List<Route> Routes { get; set; }
        public List<CycleDays> RideDays { get; set; }
        public string RideTime { get; set; }
        public string StartLocation { get; set; }
        public string AverageSpeed { get; set; }
        public int RideHour { get; set; }
        public int RideMinute { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedTimeStamp { get; set; }
        public DateTime ModifiedTimeStamp { get; set; }
        public string Description { get; set; }
        public string MapUrl { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
    }
}
