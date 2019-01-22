using System;
using Caliburn.Micro;
using FreightAlliance.Base.Models;
using FreightAlliance.Common.Enums;
using Telerik.Windows.Controls;

namespace FreightAlliance.Orders.ViewModels
{
    public class CloseOrderViewModel : Screen
    {
        public Order order;

        public CloseOrderViewModel(Order order)
        {
            this.order = order;

            this.CancelDeleteCommand = new DelegateCommand(this.CancelDelete);
            this.OkDeleteCommand = new DelegateCommand(this.OkDelete,o => this.CanAccept);

        }

        private bool CanAccept
        {
            get { return !string.IsNullOrEmpty(this.order.DeleteReason); }
        }

        private void CancelDelete(object obj)
        {
            this.DeleteReason = string.Empty;
            this.Confirmed = false;
            ((IDeactivate)this).Deactivate(true);
        }

        private void OkDelete(object obj)
        {
            this.order.DeleteDate = DateTime.Now;
            this.order.Status = StatusEnum.Canceled;
            this.Confirmed = true;
            ((IDeactivate)this).Deactivate(true);
        }

        public bool Confirmed { get; set; }

        public DelegateCommand OkDeleteCommand { get; set; }

        public DelegateCommand CancelDeleteCommand { get; set; }

        public string DeleteReason
        {
            get { return this.order.DeleteReason; }
            set
            {
                if (this.order.DeleteReason == value)
                {
                    return;
                }

                this.order.DeleteReason = value;
                this.NotifyOfPropertyChange(nameof(DeleteReason));
                this.NotifyOfPropertyChange(nameof(CanAccept));
                this.OkDeleteCommand.InvalidateCanExecute();
            }
        }
    }
}