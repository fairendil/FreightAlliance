
using System;
using System.Collections.Generic;

namespace FreightAlliance.Base.Models
{
    public class Supplier 
    {
        public int SupplierId { get; set; }
        public Supplier()
        {
            this.Name = string.Empty;
            this.OrderGuids = new List<Guid>();
        }

        public List<Guid> OrderGuids { get; set; }
        public Guid OrderGuid { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
