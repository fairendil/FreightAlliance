using System.ComponentModel.DataAnnotations;
using FreightAlliance.Common.Enums;

namespace FreightAlliance.Inventarization.ViewModels
{
    using FreightAlliance.Base.Models;
    using FreightAlliance.Inventarization.Properties;
    using Base.Providers;
    using FreightAlliance.Common.Common;

    public class OrderPositionViewModel : ViewModelBase
    {
        protected SparePartsOrderPosition orderPosition;
        private int writenOff;
        private DataProvider dataProvider;

        public OrderPositionViewModel(SparePartsOrderPosition orderPosition)
        {
            this.orderPosition = orderPosition;
        }

        public OrderPositionViewModel(SparePartsOrderPosition orderPosition, int writenOff, DataProvider dataProvider) : this(orderPosition)
        {
            this.writenOff = writenOff;
            this.dataProvider = dataProvider;
        }

        [Display(AutoGenerateField = false)]
        public int Id => this.orderPosition.OrderPositionId;

        [Display(Name = "QuantityText", Order = 3, ResourceType = typeof(Resources))]
        public float Quantity
        {
            get
            {
                return this.orderPosition.Quantity;
            }

           }

        [Display(Name = "DescriptionText", Order = 2, ResourceType = typeof(Resources))]
        public string Description
        {
            get
            {
                return this.orderPosition.Description;
            }

         
        }

        [Display(Name = "ItemCodeText", Order = 1, ResourceType = typeof(Resources))]
        public string PartNumberType
        {
            get
            {
                return this.orderPosition.ItemCode;
            }

          
        }

        [Display(Name = "StoragePlaceColumnText", Order = 6, ResourceType = typeof(Resources))]
        public string StoragePlace
        {
            get
            {
                return this.orderPosition.StoragePlace;
            }
        }

        [Display(Name = "AmountText", Order = 5, ResourceType = typeof(Resources))]
        public float Amount
        {
            get
            {
                return this.Quantity - this.writenOff;
            }
        }

        [Display(Name = "WritenOffText", Order = 4, ResourceType = typeof(Resources))]
        public int WritenOff
        {
            get
            {
                return this.writenOff;
            }
        }


    }
}