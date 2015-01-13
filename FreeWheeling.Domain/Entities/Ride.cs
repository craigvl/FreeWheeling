using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheeling.Domain.Entities
{
    public class Ride
    {
        public int id { get; set; }
        public DateTime RideDate { get; set; }
        public string RideTime { get; set; }
        [Required]
        public Group Group { get; set; }
        public List<Rider> Riders { get; set; }
        public List<Comment> Comments { get; set; }
        public List<RouteVote> RoutesVotes { get; set; }
    }
}
