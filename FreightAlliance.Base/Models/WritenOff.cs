using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreightAlliance.Base.Models
{
    public class WritenOff
    {
        public int WritenOffId { get; set; }

        public int OrderPositionId { get; set; }

        public int Quantity { get; set; }

        public DateTime Date { get; set; }

    }
}
