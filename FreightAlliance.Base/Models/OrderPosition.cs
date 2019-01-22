using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FreightAlliance.Base.Models
{
    using FreightAlliance.Base.Properties;



    public class OrderPosition 
    {

        public int OrderPositionId { get; set; }

        [ImportingConstructor]
        public OrderPosition()
        {
            
            this.Unit = "pc";
            this.Description = string.Empty;
            this.PartNumberType = string.Empty;
            this.Curency = CurencyEnum.RUB;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid OrderPositionGuid { get; set; }

        public Guid OrderGuid { get; set; }

        public Guid ShipPositionGuid { get; set; }
        

        public string Description { get; set; }

        public string Unit { get; set; }

        public string PartNumberType { get; set; }
        
        public float Quantity { get; set; }

        public float Price { get; set; }

        public CurencyEnum Curency { get; set; }

        public override string ToString()
        {
            return string.Empty;
        }

    }
}
