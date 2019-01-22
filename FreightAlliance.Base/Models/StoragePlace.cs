using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreightAlliance.Base.Models
{
    public class StoragePlace
    {
        [Key]
        public int StoragePlacePK { get; set; }

        public string Place { get; set; }
    }
}
