using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheeling.Domain.Entities
{
    public class RouteVote
    {
        public int id { get; set; }
        public Route route { get; set; }
        public string UserID { get; set; }
        [Required]
        public Ride ride { get; set; }
    }
}
