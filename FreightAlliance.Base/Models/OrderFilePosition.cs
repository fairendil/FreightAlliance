using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreightAlliance.Base.Models
{
    public class OrderFilePosition 
    {
        public int OrderFilePositionId { get; set; }

        public int ParentOrderId { get; set; }
        public int OrderId { get; set; }
        public string Name { get; set; }

        public Guid OrderGuid { get; set; }

        public int Number { get; set; }

        public bool Cheked { get; set; }

    }
}
