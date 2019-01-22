using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using FreightAlliance.Base.Models;
using FreightAlliance.Common.Attributes;

namespace FreightAlliance.Suppliers.ViewModels
{
    using Caliburn.Micro;

    using FreightAlliance.Common.Helpers;
    using FreightAlliance.Common.Interfaces;

    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System;

    using Telerik.Windows.Controls;

    [Export("Suppliers", typeof(IScreen))]
    [Export(typeof(IScreen))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SuppliersViewModel : Screen, INavigationScreen, ICanRibbon
    {
        private User user;
        private IDataProvider<Supplier> suppliersProvider;
        private BindableCollection<SupplierViewModel> suppliers;
        public string NavigationTitle => Properties.Resources.SupplierText;

        public string NavigationItemName => "Suppliers";



        public string NavigationGroup
        {
            get
            {
                var title = this.user.Role == RoleEnum.Manager ? Properties.Resources.MainText : string.Empty;
                return title;
            }
        }

        public List<RibbonTab> Tabs
        {
            get
            {

                var tab = new RibbonTab();
                var group = new RibbonGroup();
                if (this.user.Role == RoleEnum.Manager)
                {
                    var sparebutton = new RibbonButton
                    {
                        Title = Properties.Resources.AddSupplierText,
                        ImgPath = "pack://application:,,,/Resources/Images/plus.png",
                        Command = new DelegateCommand(this.AddSupplier)
                    };

                    var deleteButton = new RibbonButton
                    {
                        Title = Properties.Resources.DeleteSupplierText,
                        ImgPath = "pack://application:,,,/Resources/Images/fa-trash.png",
                        Command = new DelegateCommand(this.DeleteSupplier)
                    };
                    group.Title = Properties.Resources.SupplierText;
                    group.Buttons = new List<RibbonButton> { sparebutton, deleteButton };
                    tab.Title = Properties.Resources.MainText;
                    
                    tab.Groups = new List<RibbonGroup> { group };
                }
                return new List<RibbonTab> { tab };
            }
        }

        private void DeleteSupplier(object obj)
        {
            if (this.SelectedSupplier != null && this.SelectedSupplier.supplier.OrderGuid == Guid.Empty)
            {
                this.suppliersProvider.Remove(this.SelectedSupplier.supplier);
                this.suppliers.Remove(this.SelectedSupplier);
            }
        }

        [ImportingConstructor]
        public SuppliersViewModel([Import("SupplierProvider")] IDataProvider<Supplier> suppliersProvider,
                                 [Import("User")] IUser user)
        {
            this.suppliersProvider = suppliersProvider;
            this.SelectedSupplier = new SupplierViewModel(new Supplier(), this.user,this.suppliersProvider);
            this.user = (User)user;
            this.suppliers = new BindableCollection<SupplierViewModel>();
            this.Suppliers = CollectionViewSource.GetDefaultView(this.suppliers);
        }

        public SupplierViewModel SelectedSupplier { get; set; }

        public ICollectionView Suppliers
        {
            get; set;
        }

        private void AddSupplier(object obj)
        {
            var sup = new Supplier();
            this.suppliersProvider.Add(sup);
            this.suppliers.Add(new SupplierViewModel(sup, this.user, this.suppliersProvider));
        }

        private void Update()
        {
            var oldInv = this.suppliers.Select(i => i.Id);
            var providerInv = this.suppliersProvider.GetAll().Select(inv => inv.SupplierId);

            foreach (var item in providerInv.Except(oldInv))
            {
                this.suppliers.Add(new SupplierViewModel(this.suppliersProvider.GetById(item), this.user, this.suppliersProvider));
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            this.Update();
            this.Suppliers.Refresh();
        }
    }
}
