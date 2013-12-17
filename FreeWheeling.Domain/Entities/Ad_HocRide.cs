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
        public List<Rider> Riders { get; set; }
        public List<Comment> Comments { get; set; }
        public string StartLocation { get; set; }
        public string AverageSpeed { get; set; }
    }
}
