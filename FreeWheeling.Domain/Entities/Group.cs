using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheeling.Domain
{
    public class Group
    {

        public int id { get; set; }
        public string name { get; set; }
        public bool IsPrivate {get;set;}
        public List<Member> Members { get; set; }


    }
}
