using FreeWheeling.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FreeWheeling.UI.Models
{

    public class HomeIndexModel
    {
        public string LocationsId { get; set; }
        public List<Location> Locations { get; set; }
    }

}