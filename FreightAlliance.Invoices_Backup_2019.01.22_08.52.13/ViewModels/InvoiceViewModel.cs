using System.Windows;
using FreightAlliance.Common.Interfaces;

namespace FreightAlliance.Invoices.ViewModels
{
    using Base.Models;
    using Caliburn.Micro;
    using Common.Common;
    using Microsoft.Win32;
    using System;
    using FreightAlliance.Invoices.Properties;
    using System.ComponentModel.DataAnnotations;
    using System.IO;

    public class InvoiceViewModel : ViewModelBase
    {

        public readonly Invoice invoice;

        public readonly User User;


        public InvoiceViewModel(Invoice Invoice, User user, IDataProvider<Invoice> invoiceProvider)
        {
            this.invoice = Invoice;
            this.User = user;
            this.invoiceProvider = invoiceProvider;
        }

        public IDataProvider<Invoice> invoiceProvider;

        public InvoiceViewModel()
        {
        }

        [Display(AutoGenerateField = false)]
        public int Id
        {
            get
            {
                return this.invoice.InvoiceId;
            }
        }

        [Display(Name = "AmountText", Order = 0, ResourceType = typeof(Resources))]
        public float Amount
        {
            get
            {
                return this.invoice.Amount;
            }

            set
            {
                if (Equals(this.invoice.Amount, value))
                {

                    return;

                }

                this.invoice.Amount = value;
                this.OnPropertyChanged(nameof(this.Amount));
            }
        }



        [Display(Name = "InvoiceDateText", Order = 1, ResourceType = typeof(Resources))]
        public DateTime? InvoiceDate
        {
            get
            {
                return this.invoice.Date >= DateTime.Now.AddYears(-30) ? this.invoice.Date : (DateTime?)null; 
            }

            set
            {
                if (this.invoice.Date == value)
                {
                    return;
                }
                this.invoice.Date = (DateTime) value;
                this.OnPropertyChanged(nameof(InvoiceDate));
            }

        }

        [Display(Name = "DeliveryAmountText", Order = 2, ResourceType = typeof(Resources))]
        public float DeliveryAmount
        {
            get
            {
                return this.invoice.DeliveryAmount;
            }

            set
            {
                if (this.invoice.DeliveryAmount == value)
                {
                    return;
                }
                this.invoice.DeliveryAmount = value;
                this.OnPropertyChanged();
            }
        }

        [Display(Name = "PaidAmountText", Order = 4, ResourceType = typeof(Resources))]
        public float PaidAmount
        {
            get
            {
                return this.invoice.PaidAmount;
            }
            set
            {
                if (this.invoice.PaidAmount == value)
                {
                    return;
                }
                this.invoice.PaidAmount = value;
                this.OnPropertyChanged();
            }
        }

        [Display(Name = "CurrencyText", Order = 5, ResourceType = typeof(Resources))]
        public CurencyEnum Currency
        {
            get
            {
                return this.invoice.CurrencyTemp;
            }

            set
            {
                    if (this.invoice.CurrencyTemp == value)
                    {
                        return;
                    }
                    this.invoice.CurrencyTemp = value;
                    this.OnPropertyChanged();                
            }
        }

        [Display(Name = "FullyPaidText", Order = 6, ResourceType = typeof(Resources))]
        public bool FullyPaid
        {
            get
            {
                return this.invoice.FullyPaid;
            }
            set
            {
                if (this.invoice.FullyPaid == value)
                {
                    return;
                }
                this.invoice.FullyPaid = value;
                this.OnPropertyChanged();
            }
        }

        public void Save()
        {
            this.invoiceProvider.SaveChanges(this.invoice);
        }
    }


}




