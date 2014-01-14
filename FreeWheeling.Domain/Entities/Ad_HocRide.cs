using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheeling.Domain.Entities
{
    public class Ad_HocRide
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Creator { get; set; }
        public DateTime RideDate { get; set; }
        public string RideTime { get; set; }
        public Location Location { get; set; }
        public List<AdHocRider> Riders { get; set; }
        public List<AdHocComment> Comments { get; set; }
        public string StartLocation { get; set; }
        public string AverageSpeed { get; set; }
        public int RideHour { get; set; }
        public int RideMinute { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTimeStamp { get; set; }
        public DateTime ModifiedTimeStamp { get; set; }
    }
}
