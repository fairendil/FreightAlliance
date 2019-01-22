using System;
using System.ComponentModel.DataAnnotations;
using FreightAlliance.Base.Models;
using FreightAlliance.Common.Enums;
using FreightAlliance.Orders.Properties;

namespace FreightAlliance.Orders.ViewModels
{
    public class SparePartsOrderPositionViewModel : OrderPositionViewModel
    {

        private readonly SparePartsOrderPosition orderPosition;

        
        public SparePartsOrderPositionViewModel(SparePartsOrderPosition orderPosition, OrderType type) : base(orderPosition, type)
        {
            this.orderPosition = orderPosition;
        }



        [Display(Name = "RemarksText", Order = 4, ResourceType = typeof(Resources))]
        public string Remarks
        {
            get
            {
                return this.orderPosition.Remarks;
            }

            set
            {
                if (this.orderPosition.Remarks.Equals(value))
                {
                    return;
                }

                this.orderPosition.Remarks = value;
                this.OnPropertyChanged();
            }
        }


        [Display(Name = "ItemCodeText", Order = 1, ResourceType = typeof(Resources))]
        public string ItemCode
        {
            get
            {
                return this.orderPosition.ItemCode;
            }

            set
            {
                if (this.orderPosition.ItemCode.Equals(value))
                {
                    return;
                }

                this.orderPosition.ItemCode = value;
                this.OnPropertyChanged(nameof(ItemCode));
            }
        }

        [Display(Name = "PosInDrawningText", Order = 5, ResourceType = typeof(Resources))]
        public string PosInDrawning
        {
            get
            {
                return this.orderPosition.PosInDrawning;
            }

            set
            {
                if (this.orderPosition.PosInDrawning.Equals(value))
                {
                    return;
                }

                this.orderPosition.PosInDrawning = value;
                this.OnPropertyChanged(nameof(PosInDrawning));
            }
        }

        [Display(Name = "UnitText", Order = 6, ResourceType = typeof(Resources))]
        public string Unit
        {
            get
            {
                return this.orderPosition.Unit;
            }

            set
            {
                if (this.orderPosition.Unit.Equals(value))
                {
                    return;
                }

                this.orderPosition.Unit = value;
                this.OnPropertyChanged();
            }
        }


        [Display(Name = "ReceivedColumnText", Order = 7, ResourceType = typeof(Resources))]
        public bool Received
        {
            get
            {
                return this.orderPosition.Received;
            }
            set
            {
                if (this.orderPosition.Received == value) return;
                this.orderPosition.Received = value;
                this.OnPropertyChanged(nameof(Received));
                this.OnPropertyChanged(nameof(StoragePlace));
            }
        }

        [Display(Name = "StoragePlaceColumnText", Order = 8, ResourceType = typeof(Resources))]
        public string StoragePlace
        {
            get
            {
                return this.orderPosition.StoragePlace;
            }

            set
            {
                if (this.orderPosition.StoragePlace == value)
                {
                    return;
                }

                this.orderPosition.StoragePlace = value;
                if (!string.IsNullOrEmpty(this.orderPosition.StoragePlace))
                {
                    this.orderPosition.Received = true;
                }

                this.OnPropertyChanged(nameof(StoragePlace));
                this.OnPropertyChanged(nameof(Received));
            }
        }
    }
}