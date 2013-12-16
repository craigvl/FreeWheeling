using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheeling.Domain.Entities
{
    public class Member
    {
        public int id { get; set; }
        public string userId { get; set; }
        public Group Group { get; set; }
    }

}
