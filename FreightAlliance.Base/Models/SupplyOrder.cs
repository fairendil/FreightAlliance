namespace FreightAlliance.Base.Models
{
    public class SupplyOrder : Order
    {
        public SupplyOrder() : base()
        {
           
        }

        public SupplyOrder(User user)
            : base(user)
        {
           
        }

        public int SupplyOrderId { get; set; }

        public override int OrderId
        {
            get { return this.SupplyOrderId; }
        }
        public string Person { get; set; }

        public int Perso_PersonId { get; set; }


    }
}
