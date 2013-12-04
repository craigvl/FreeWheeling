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
        public List<Member> Members { get; set; }
        public List<Ride> Rides { get; set; }
        public List<Route> Routes { get; set; }
        public List<CycleDays> RideDays { get; set; }
        public string RideTime { get; set; }
    }
}
