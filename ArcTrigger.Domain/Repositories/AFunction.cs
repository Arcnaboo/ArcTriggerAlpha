using ArcTrigger.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcTrigger.Domain.Repositories
{
    public class AFunction : Iexample
    {
        void Iexample.Function()
        {
            // A function
            Console.WriteLine("A");
        }
    }
}
