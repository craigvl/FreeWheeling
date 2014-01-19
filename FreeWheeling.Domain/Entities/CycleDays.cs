using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheeling.Domain.Entities
{
    public class CycleDays
    {
        public int id { get; set; }
        public string DayOfWeek { get; set; }
        [Required]
        public Group Group { get; set; }
    }
}
