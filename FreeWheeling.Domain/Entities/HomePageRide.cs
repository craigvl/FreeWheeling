using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheeling.Domain.Entities
{
    public class HomePageRide
    {
        public int id { get; set; }
        public int Rideid { get; set; }
        public string Userid { get; set; }
        public bool IsRandomRide { get; set; }
    }
}
