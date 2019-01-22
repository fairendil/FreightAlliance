using System;
using System.ComponentModel.DataAnnotations;
using FreightAlliance.Common.Enums;
using FreightAlliance.Orders.Properties;

namespace FreightAlliance.Orders.ViewModels
{
    using FreightAlliance.Base.Models;
    using FreightAlliance.Common.Common;

    public class OrderPositionViewModel : ViewModelBase
    {
        protected OrderPosition orderPosition;

        public OrderPositionViewModel(OrderPosition orderPosition, OrderType type)
        {
            this.orderPosition = orderPosition;
            this.Type = type;
        }

        [Display(AutoGenerateField = false)]
        public Guid Id => this.orderPosition.OrderPositionGuid;

        [Display(Name = "QuantityText", Order = 1, ResourceType = typeof(Resources))]
        public float Quantity
        {
            get
            {
                return this.orderPosition.Quantity;
            }

            set
            {
                if (this.orderPosition.Quantity.Equals(value))
                {
                    return;
                }

                this.orderPosition.Quantity = value;
                this.OnPropertyChanged();
                
            }
        }

        [Display(Name = "DescriptionText", Order = 3, ResourceType = typeof(Resources))]
        public string Description
        {
            get
            {
                return this.orderPosition.Description;
            }

            set
            {
                if (this.orderPosition.Description.Equals(value))
                {
                    return;
                }

                this.orderPosition.Description = value;
                this.OnPropertyChanged();
            }
        }

        [Display(Name = "PartNumberTypeText", Order = 2, ResourceType = typeof(Resources))]
        public string PartNumberType
        {
            get
            {
                return this.orderPosition.PartNumberType;
            }

            set
            {
                if (this.orderPosition.PartNumberType == value)
                {
                    return;
                }

                this.orderPosition.PartNumberType = value;
                this.OnPropertyChanged();
            }
        }

        [Display(AutoGenerateField = false)]
        public OrderType Type { get; private set; }

        [Display(Name = "PriceText", Order = 5, ResourceType = typeof(Resources))]
        public float Price
        {
            get
            {
                return this.orderPosition.Price;
            }

            set
            {
                if (this.orderPosition.Price.Equals(value))
                {
                    return;
                }

                this.orderPosition.Price = value;

                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.Amount));
            }
        }

        [Display(Name = "AmountText", Order = 6, ResourceType = typeof(Resources))]
        public float Amount
        {
            get
            {
                return this.Quantity * this.Price;
            }
        }

        [Display(Name = "CurencyText", Order = 7, ResourceType = typeof(Resources))]
        public CurencyEnum Curency
        {
            get { return this.orderPosition.Curency; }
            set
            {
                if (this.orderPosition.Curency.Equals(value))
                {
                    return;
                }

                this.orderPosition.Curency = value;

                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.Curency));
            }
        }


    }
}