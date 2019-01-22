using System.ComponentModel.DataAnnotations.Schema;


namespace FreightAlliance.Base.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using FreightAlliance.Common.Enums;

    public abstract class Order
    {
        protected Order()
        {
            //this.CreationDate = DateTime.Now;
            //this.DeleteDate = DateTime.Now.AddYears(-100);
            //this.ReceivedAtOfficeDate = DateTime.Now.AddYears(-100);
            //this.ReceivedAtVesselDate = DateTime.Now.AddYears(-100);
            //this.SentToOfficeDate = DateTime.Now.AddYears(-100);
            //this.Code = new Code();
            //this.Number = new Number();
            //this.OrderPositions = new List<OrderPosition>();
            //this.Supplier = new Supplier();
            //this.Vessel = string.Empty;
            //this.Status = StatusEnum.New;
            //this.DeleteReason = string.Empty;
            //this.Comment = string.Empty;
            //this.FilePositions = new List<OrderFilePosition>();
            //this.OrderGuid = Guid.NewGuid();
        }

        protected Order(User user)
        {
            this.CreationDate = DateTime.Now;
            this.DeleteDate = DateTime.Now.AddYears(-100);
            this.ReceivedAtOfficeDate = DateTime.Now.AddYears(-100);
            this.ReceivedAtVesselDate = DateTime.Now.AddYears(-100);
            this.SentToOfficeDate = DateTime.Now.AddYears(-100);
            this.Code = new Code();
            this.Number = new Number();
            this.OrderPositions = new List<OrderPosition>();
            this.Supplier = new Supplier();
            this.Vessel = user.Vessel;
            this.PersonPost = user.Position;
            this.Status = StatusEnum.New;
            this.DeleteReason = string.Empty;
            this.Comment = string.Empty;
            this.FilePositions = new List<OrderFilePosition>();
            this.OrderGuid = Guid.NewGuid();
        }

        public virtual int OrderId { get; set; }

        public virtual Code Code { get; set; }

        public string Comment { get; set; }

        public DateTime CreationDate { get; set; }

        public virtual Invoice Invoice { get; set; }

        public virtual Number Number { get; set; }

        public virtual ICollection<OrderPosition> OrderPositions { get; set; }

        public DateTime ReceivedAtVesselDate { get; set; }

        public DateTime ReceivedAtOfficeDate { get; set; }

        public DateTime SentToOfficeDate { get; set; }

        public virtual Supplier Supplier { get; set; }

        
       
        public string Vessel { get; set; }

        public StatusEnum Status { get; set; }

        public OrderType Type { get; set; }

        public string DeleteReason { get; set; }

        public DateTime DeleteDate { get; set; }

        public string PersonPost { get; set; }

        public Guid OrderGuid { get; set; }
        public virtual ICollection<OrderFilePosition> FilePositions {get; set;}

    }
}