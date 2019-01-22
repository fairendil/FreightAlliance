using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreightAlliance.Base.Models
{
    public class Position
    {
        [Key]
        public int PositionID { get; set; }

        public string PositionEng { get; set; }

        public string PositionRus { get; set; }
    }
}
