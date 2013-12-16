using FreeWheeling.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FreeWheeling.UI.Models
{

    public class GroupModel
    {
        public string Name;
        public List<Group> _Groups;
        public List<int> CurrentGroupMembership;
        public List<NextRideDetails> _NextRideDetails;
        public string UserLocation;
        public string title;
    }

    public class MyGroupsModel
    {
        public int id { get; set; }
        public List<Group> CycleGroups { get;set; }

    }

    public class NextRideDetails
    {
        public int GroupId { get; set; }
        public DateTime Date { get; set; }
        public int NumberofRiders { get; set; }
    }
}