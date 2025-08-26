using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcTrigger.Core.Entities
{
    public class Order
    {

        public Guid Id { get;  }
        public int intid { get;  }
        public Guid UserId { get;  }
        public string Symbol { get; set; }

        public DateTime DateTime { get; set; }

        private Order() { }
        public Order(int intid, Guid userId, string symbol)
        {
            Id = Guid.NewGuid();
            this.intid = intid;
            UserId = userId;
            Symbol = symbol;
            DateTime = DateTime.UtcNow;
        }
    }
}
