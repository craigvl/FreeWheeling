using FreeWheeling.Domain.Abstract;
using FreeWheeling.Domain.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheeling.Domain.Concrete
{
    public class CycleRepository : ICycleRepository
    {
        private CycleDb context = new CycleDb();

        public IEnumerable<Group> GetGroups()
        {
            return context.Groups.ToList(); 
        }

        public Group GetGroupByID(int id)
        {
            Group group = context.Groups.Find(id);

            return group;
        }

        public void AddMember(string UserId, Group _Group)
        {
            Member NewMember = new Member { userId = UserId, Group = _Group };
            context.Members.Add(NewMember);
            context.Entry(NewMember).State = System.Data.Entity.EntityState.Added;
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
