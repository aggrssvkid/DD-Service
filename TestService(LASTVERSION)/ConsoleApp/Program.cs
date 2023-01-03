using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestService;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            TotalCost hz = new TotalCost();
            DateTime timeone = new DateTime(2022, 12, 14);
            DateTime timetwo = new DateTime(2022, 12, 14);
            string port = "MOW";
            var kek = hz.GetTotalCost(timeone, timetwo, port);
            Console.WriteLine(kek);
        }
    }
}
