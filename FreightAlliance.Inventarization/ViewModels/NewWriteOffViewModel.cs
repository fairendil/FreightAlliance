using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace FreightAlliance.Inventarization.ViewModels
{
    public class NewWriteOffViewModel: Screen
    {
        private DateTime date;
        private int quantity;

        public NewWriteOffViewModel(DateTime date)
        {
            this.date = date;
            this.quantity = 0;
        }


        public DateTime Date
        {
            get
            {
                return this.date;
            }

            set
            {
                if (this.date == value)
                {
                    return;
                }
                this.date = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(Date)));
                
            }

        }

        public int Quantity
        {
            get
            {
                return this.quantity;
            }

            set
            {
                if (this.quantity == value)
                {
                    return;
                }
                this.quantity = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(Quantity)));
                
            }

        }

        public void Select()
        {
            this.Confirmed = true;
            ((IDeactivate)this).Deactivate(true);
        }

        public bool Confirmed { get; set; }

        public void Cancel()
        {
            this.Confirmed = false;
            this.TryClose(false);
        }

    }
}
