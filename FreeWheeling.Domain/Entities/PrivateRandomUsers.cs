using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheeling.Domain.Entities
{
    public class PrivateRandomUsers
    {
        public int id { get; set; }
        public int RideId { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
    }
}