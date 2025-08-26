using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcTrigger.Core.Entities
{
    public class PreviousOrder
    {
        public Guid Id { get; set; }
        public int Intid { get; set; }
        public Guid UserId { get; set; }
        public string Symbol { get; set; }
        public DateTime DateTime { get; set; }
        private PreviousOrder() { }


        public PreviousOrder(int intid, Guid userId, string symbol)
        {
            Id = Guid.NewGuid();
            this.Intid = intid;
            UserId = userId;
            Symbol = symbol;
            DateTime = DateTime.UtcNow;
        }
    }
}
