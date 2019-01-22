namespace FreightAlliance.Base.Models
{
    using System;

    public class SparePartsOrder : Order
    {
        public SparePartsOrder():base()
        {
            //this.ManufacturedYear = DateTime.Now.AddYears(-100);
            //this.Drawing = string.Empty;
            //this.Manufactured = string.Empty;
            //this.Plate = string.Empty;
            //this.PartNumberType = string.Empty;
            //this.SuplyFor = string.Empty;
            //this.PartSerialNumber = string.Empty;
        }

        public SparePartsOrder(User user)
            : base(user)
        {
            this.ManufacturedYear = DateTime.Now.AddYears(-100);
            this.Drawing = string.Empty;
            this.Manufactured = string.Empty;
            this.Plate = string.Empty;
            this.PartNumberType = string.Empty;
            this.SuplyFor = string.Empty;
            this.PartSerialNumber = string.Empty;
            
        }

        public int SparePartsOrderId { get; set; }

        public override int OrderId
        {
            get { return this.SparePartsOrderId; }
        }

        public string Drawing { get; set; }

        public string Manufactured { get; set; }

        public DateTime ManufacturedYear { get; set; }

        public string PartSerialNumber { get; set; }

        public string Plate { get; set; }

        public string PartNumberType { get; set; }

        public string SuplyFor { get; set; }

        public string My { get; set; }
    }
}
