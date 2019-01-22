using System.ComponentModel.DataAnnotations;
using FreightAlliance.Base.Models;
using FreightAlliance.Common.Enums;

namespace FreightAlliance.Orders.ViewModels
{
    public class SupplyOrderPositionViewModel : OrderPositionViewModel
    {
        private readonly SupplyOrderPosition orderPosition;

        public SupplyOrderPositionViewModel(SupplyOrderPosition orderPosition, OrderType type) : base(orderPosition, type)
        {
            this.orderPosition = orderPosition;
        }

    }
}