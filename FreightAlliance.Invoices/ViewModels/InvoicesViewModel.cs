using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using FreightAlliance.Base.Models;
using FreightAlliance.Common.Attributes;

namespace FreightAlliance.Invoices.ViewModels
{
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using FreightAlliance.Common.Helpers;
    using FreightAlliance.Common.Interfaces;

    using Telerik.Windows.Controls;

    [Export("Invoices", typeof(IScreen))]
    [Export(typeof(IScreen))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class InvoicesViewModel : Screen, INavigationScreen, ICanRibbon
    {
        private Invoice invoice;

        public string NavigationTitle => Properties.Resources.InvoiceText;

        public string NavigationItemName => "Invoices";

        public string NavigationGroup {
            get
            {
                var title = this.user.Role == RoleEnum.Manager? Properties.Resources.MainText : string.Empty;
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
                        Title = Properties.Resources.AddInvoiceText,
                        ImgPath = "pack://application:,,,/Resources/Images/plus.png",
                        Command = new DelegateCommand(this.AddInvoice)
                    };

                    var deleteButton = new RibbonButton
                    {
                        Title = Properties.Resources.DeleteInvoiceText,
                        ImgPath = "pack://application:,,,/Resources/Images/fa-trash.png",
                        Command = new DelegateCommand(this.DeleteInvoice)
                    };
                    group.Title = Properties.Resources.InvoiceText;
                    group.Buttons = new List<RibbonButton> {sparebutton, deleteButton };
                    tab.Title = Properties.Resources.MainText;
                    tab.Groups = new List<RibbonGroup> {group};
                }
                return new List<RibbonTab> {tab};
            }
        }

        private void DeleteInvoice(object obj)
        {
            if (this.SelectedInvoice != null && this.SelectedInvoice.invoice.OrderGuid == Guid.Empty)
            {
                this.invoicesProvider.Remove(this.SelectedInvoice.invoice);
                this.invoices.Remove(this.SelectedInvoice);
            }
        }

        private void AddInvoice(object obj)
        {
            var inv = new Invoice();
            this.invoicesProvider.Add(inv);
            this.invoices.Add(new InvoiceViewModel(inv, this.user,this.invoicesProvider));
        }


        private readonly BindableCollection<InvoiceViewModel> invoices;
        private User user;
        private IDataProvider<Order> ordersProvider;
        private IDataProvider<Invoice> invoicesProvider;

        [ImportingConstructor]
        public InvoicesViewModel([Import("OrdersProvider")] IDataProvider<Order> ordersProvider,
                                 [Import("InvoiceProvider")] IDataProvider<Invoice> invoiceProvider,
                                 [Import("User")] IUser user)
        {
            this.invoicesProvider = invoiceProvider;
            this.ordersProvider = ordersProvider;
            this.SelectedInvoice = new InvoiceViewModel();
            this.user = (User)user;
            this.invoices = new BindableCollection<InvoiceViewModel>();
            this.Invoices = CollectionViewSource.GetDefaultView(this.invoices);

        }

        public InvoiceViewModel SelectedInvoice { get; set; }

        public ICollectionView Invoices { get; set; }

        private void Update()
        {
            var oldInv = this.invoices.Select(i => i.Id);
            var providerInv = this.invoicesProvider.GetAll().Select(inv => inv.InvoiceId);

            foreach (var item in providerInv.Except(oldInv))
            {
                this.invoices.Add(new InvoiceViewModel(this.invoicesProvider.GetById(item),this.user,this.invoicesProvider));
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            this.Update();
            this.Invoices.Refresh();
        }
    }
}
