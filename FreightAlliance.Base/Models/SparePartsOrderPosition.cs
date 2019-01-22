namespace FreightAlliance.Base.Models
{
    public class SparePartsOrderPosition: OrderPosition
    {
        public SparePartsOrderPosition() : base()
        {
            this.ItemCode = string.Empty;
            this.PosInDrawning = string.Empty;
            this.Remarks = string.Empty;
            this.StoragePlace = string.Empty;
        }

        public int SparePartsOrderPositionId { get; set; }
        public string ItemCode { get; set; }
        public string PosInDrawning { get; set; }
        public string Remarks { get; set; }
        public string PartSerialNumber { get; set; }
        public bool Received { get; set; }
        public string StoragePlace { get; set; }
    }
}