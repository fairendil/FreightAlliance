using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreightAlliance.Base.Models;
using FreightAlliance.Common.Common;
using FreightAlliance.Common.Enums;
using FreightAlliance.Orders.Properties;

namespace FreightAlliance.Orders.ViewModels
{

    public class OrderFilePositionViewModel : ViewModelBase
    {
        public readonly OrderFilePosition orderPosition;

        public OrderFilePositionViewModel(OrderFilePosition orderPosition)
        {
            this.orderPosition = orderPosition;
        }

        [Display(AutoGenerateField = false)]
        public int Id { get { return this.orderPosition.OrderFilePositionId; } }

        [Display(AutoGenerateField = false)]
        public int ParentOrderId { get; set; }

        [Display(AutoGenerateField = false)]
        public int OrderId { get; set; }

        [Display(Name = "NameText", Order = 2, ResourceType = typeof(Resources))]
        public string Name
        {
            get
            {
                return Path.GetFileNameWithoutExtension(this.orderPosition.Name);
            }

        }

        [Display(Name = "NumberText", Order = 1, ResourceType = typeof(Resources))]
        public int Number
        {
            get
            {
                return this.orderPosition.Number;
            }

        }

        [Display(Name = "SelectedText", Order = 3, ResourceType = typeof(Resources))]
        public bool Cheked
        {
            get
            {
                return this.orderPosition.Cheked;
            }

            set
            {
                if (this.orderPosition.Cheked.Equals(value))
                {
                    return;
                }

                this.orderPosition.Cheked = value;
                this.OnPropertyChanged();

            }

        }


    }
}
