using FreeWheeling.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FreeWheeling.UI.Models
{
    public class UserSettingsModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public int LocationsId { get; set; }
        public string CurrentUserLocation { get; set; }
        public List<Location> Locations { get; set; }
        public bool ReceiveEmails { get; set; }
    }

    public class FollowingModel
    {
        public string UserID { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Display(Name = "Following")]
        public bool following { get; set; }
    }       
}