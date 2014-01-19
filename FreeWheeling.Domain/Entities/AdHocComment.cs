using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheeling.Domain.Entities
{
    public class AdHocComment
    {
        public int id { get; set; }
        public string CommentText { get; set; }
        public DateTime Date { get; set; }
        [Required]
        public Ad_HocRide AdHocRide { get; set; }
        public string userName { get; set; }
    }
}
