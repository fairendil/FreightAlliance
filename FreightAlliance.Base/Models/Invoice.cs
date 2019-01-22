using System;
using System.Collections.Generic;


namespace FreightAlliance.Base.Models
{
    public class Invoice 
    {
        public int InvoiceId { get; set; }

        public Invoice()
        {
            this.Date = DateTime.Now.AddYears(-100);
            this.InvoicePositions = new List<OrderFilePosition>();
            
        }

        public Invoice(Order order)
        {
            this.Date = DateTime.Now.AddYears(-100);
            this.InvoicePositions = new List<OrderFilePosition>();
            OrderId = order.OrderId;

        }
        public float Amount { get; set; }

        public DateTime Date { get; set; }

        public override string ToString()
        {
            return string.Format("No:{0}, {1}, {2}", this.InvoiceId, this.Date, this.Amount);
        }

        public ICollection<string> Attachments { get; set; }

        public float DeliveryAmount { get; set; }

        public float PaidAmount { get; set; }

        public virtual Currency Currency { get; set; }

        public CurencyEnum CurrencyTemp { get; set; }

        public bool FullyPaid { get; set; }



        public int OrderId { get; set; }
        public Guid OrderGuid { get; set; }

        public virtual ICollection<OrderFilePosition> InvoicePositions { get; set; }

        public virtual OrderFilePosition SelectedOrderFilePosition { get; set; }

    }

    public enum CurencyEnum
    {
        RUB,
        USD,
        EUR
    }
}
