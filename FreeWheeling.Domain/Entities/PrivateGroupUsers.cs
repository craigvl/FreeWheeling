using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheeling.Domain.Entities
{
    public class PrivateGroupUsers
    {
        public int id { get; set; }
        public int GroupId { get; set; }
        public string UserId { get; set; }
    }
}
