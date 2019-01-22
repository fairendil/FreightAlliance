using System.ComponentModel.DataAnnotations;
using FreightAlliance.Base.Models;
using FreightAlliance.Common.Common;
using FreightAlliance.Common.Interfaces;

namespace FreightAlliance.Suppliers.ViewModels
{
    using FreightAlliance.Suppliers.Properties;
    public class SupplierViewModel : ViewModelBase
    {
        public readonly Supplier supplier;
        private IDataProvider<Supplier> suppliersProvider;
        private User user;

        public SupplierViewModel(Supplier supplier, User user, IDataProvider<Supplier> suppliersProvider)
        {
            this.supplier = supplier;
            this.user = user;
            this.suppliersProvider = suppliersProvider;

        }



        [Display(AutoGenerateField = false)]
        public int Id
        {
            get
            {
                return this.supplier.SupplierId;
            }
        }

        [Display(Name = "NameText", Order = 0, ResourceType = typeof(Resources))]
        public string Amount
        {
            get
            {
                return this.supplier.Name;
            }

            set
            {
                if (Equals(this.supplier.Name, value))
                {

                    return;

                }

                this.supplier.Name = value;
                this.OnPropertyChanged(nameof(this.Amount));
            }
        }

        public void Save()
        {
            this.suppliersProvider.SaveChanges(this.supplier);
        }

    }
}
