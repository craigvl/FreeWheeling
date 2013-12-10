using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheeling.Domain.Entities
{
    public class Comment
    {
        public int id { get; set; }
        public string CommentText { get; set; }
        public DateTime Date { get; set; }
        public Ride Ride { get; set; }
        public Rider Rider { get; set; }
    }
}
