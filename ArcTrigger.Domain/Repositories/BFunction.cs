using ArcTrigger.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcTrigger.Domain.Repositories
{
    public class BFunction : Iexample
    {
        void Iexample.Function()
        {
            Console.WriteLine("B");
        }
    }
}
