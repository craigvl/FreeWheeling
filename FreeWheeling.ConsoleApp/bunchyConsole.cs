using FreeWheeling.Domain.Entities;
using FreeWheeling.Domain.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheeling.ConsoleApp
{
    public class bunchyConsole
    {
        static void Main()    
        {
            PopulateHomePageRide();
            Console.WriteLine("Hello World");
            Console.ReadLine();
        }

        private static void PopulateHomePageRide()
        {



            using (var context= new CycleDb())
            {
                List<Rider> _Rider = context.Riders.ToList();
                foreach (Rider item in _Rider)
                {
                    Console.WriteLine(item.Name);
                }
            }
        }

    }
}
