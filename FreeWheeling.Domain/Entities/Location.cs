using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheeling.Domain.Entities
{
    public class Location
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string TimeZoneInfo { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
    }
}
