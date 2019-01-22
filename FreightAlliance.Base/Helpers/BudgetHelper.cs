using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreightAlliance.Base.Helpers
{
    public class BudgetHelper
    {
        public string CodeName { get; set; }

        public string Comment { get; set; }
        public float PlanAmount { get; set; }
        public float FactAmount { get; set; }
        public int CodeNum { get; set; }
        public string CodeTypeName { get; set; }
        public int CodeType { get; set; }

    }
}
