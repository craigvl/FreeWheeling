using FreeWheeling.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheeling.Domain.Abstract
{
    public interface ICycleRepository
    {

        //Group
        IEnumerable<Group> GetGroups();
        Group GetGroupByID(int id);

        //Member
        void AddMember(string UserId, Group _Group);

        void Save();
       

    }
}
