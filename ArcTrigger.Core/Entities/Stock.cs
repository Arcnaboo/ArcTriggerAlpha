using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcTrigger.Core.Entities
{
    public class Stock
    {
        public Guid Id { get; set; }
        public string Symbol { get; set; }


        private Stock()
        {
        }


        public Stock(string symbol)
        {
            Id = Guid.NewGuid();
            this.Symbol = symbol;  
        }
    }
}
