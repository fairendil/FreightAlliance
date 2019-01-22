using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreightAlliance.Base.Models
{
    public class Number : IComparable
    {
        

        public int NumberId { get; set; }

        public int OrderId { get; set; }
        public Guid OrderGuid { get; set; }
        public int No { get; set; }

        public int ParentOrderId { get; set; }
        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        public DateTime Year { get; set; }

        /// <summary>
        /// Gets or sets the nav type.
        /// </summary>
        public NavType NavType { get; set; }

        public Number()
        {
            this.Year = DateTime.Today;
            this.No =  0;
            this.NavType = NavType.D;

        }

        public Number(int maxNo, NavType navType, int orderId)
        {
            this.Year = DateTime.Now;
            this.No = maxNo + 1;
            this.NavType = navType;
            this.OrderId = orderId;
        }

        public override string ToString()
        {
            return string.Format("{1}/{0}", this.Year.ToString("yy"), this.No);
        }

        public int CompareTo(object obj)
        {
            if ((Number)obj == null) return 1;
            return String.Compare(ToString(), ((Number) obj).ToString(), StringComparison.Ordinal);
        }
    }

    public enum NavType
    {
                E, 

        D, 

        S, 

        P

    }
}

