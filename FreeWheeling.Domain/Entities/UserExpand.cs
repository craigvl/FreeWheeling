using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheeling.Domain.Entities
{
    public class UserExpand
    {
        public int id { get; set; }
        public string userId { get; set; }
        public Boolean FirstBunch { get; set; }
        public Boolean FirstKeen { get; set; }
        public Boolean FirstComment { get; set; }
        public Boolean SecondBunch { get; set; }
        public Boolean SecondKeen { get; set; }
        public Boolean SecondComment { get; set; }
    }
}