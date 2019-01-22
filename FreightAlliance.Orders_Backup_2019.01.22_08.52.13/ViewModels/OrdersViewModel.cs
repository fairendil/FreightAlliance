using System;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using FreightAlliance.Common.Attributes;
using FreightAlliance.Common.Enums;
using FreightAlliance.Common.Extensions;
using FreightAlliance.Orders.Properties;

namespace FreightAlliance.Orders.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Caliburn.Micro;
    using FreightAlliance.Base.Models;
    using FreightAlliance.Common.Interfaces;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows.Data;

    using FreightAlliance.Common.Helpers;

    using FreightAlliance.Common.Attributes;
    using Telerik.Windows.Controls;

    [Export("Orders", typeof(IScreen))]
    [Export(typeof(IScreen))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class OrdersViewModel : Screen, INavigationScreen, ICanRibbon
    {
        private readonly IDataProvider<Order> ordersProvider;
        private readonly IOrderPostionsProvider<OrderPosition> orderPositionProvider;
        private readonly IDataProvider<Code> codesProvider;
        private readonly User user;
        private readonly BindableCollection<OrderViewModel> orders;
        private IDataProvider<Invoice> invoiceProvider;
        private IDataProvider<OrderFilePosition> orderFilePostionsProvider;
        private IDataProvider<Supplier> supplierProvider;
        private OrderViewModel selectedOrder;
        private IDataProvider<StoragePlace> storagePlacesProvider;

        [ImportingConstructor]
        public OrdersViewModel([Import("OrdersProvider")] IDataProvider<Order> ordersProvider,
            [Import("OrderPositionsProvider")] IOrderPostionsProvider<OrderPosition> orderPositionProvider,
            [Import("InvoiceProvider")] IDataProvider<Invoice> invoiceProvider,
            [Import("OrderFilesProvider")]IOrderPostionsProvider<OrderFilePosition> orderFilePositionsProvider,

            [Import("SupplierProvider")] IDataProvider<Supplier> supplierProvider,
            [Import("CodesProvider")] IDataProvider<Code> codesProvider,
            [Import("StoragePlaceProvider")] IDataProvider<StoragePlace> storagePlacesProvider,
            [Import("User")] IUser user)
        {
            this.ordersProvider = ordersProvider;
            this.orderPositionProvider = orderPositionProvider;
            this.codesProvider = codesProvider;
            this.invoiceProvider = invoiceProvider;
            this.orderFilePostionsProvider = orderFilePositionsProvider;
            this.supplierProvider = supplierProvider;
            this.storagePlacesProvider = storagePlacesProvider;
            this.user = (User) user;
            
            this.orders = new BindableCollection<OrderViewModel>();
            this.Orders = CollectionViewSource.GetDefaultView(this.orders);
            this.Orders.Filter = item =>
            {
                OrderViewModel orderModel = item as OrderViewModel;
                return orderModel.Status.GetAttribute<ViewRoles>().UserRoles.Contains(this.user.Role);
            };
            this.SelectedOrder = this.orders.FirstOrDefault();
        }

        public OrderViewModel SelectedOrder
        {
            get { return this.selectedOrder; }
            set
            {
                if (SelectedOrder == value) return;
                this.selectedOrder = value;
                if (this.selectedOrder == null)
                {
                    this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(SelectedOrder)));
                    return;
                }
                this.selectedOrder.InvoicesUpdate();
                if (this.selectedOrder is SparePartsOrderViewModel)
                {
                    (this.selectedOrder as SparePartsOrderViewModel).LoadPositions();
                }
                else
                {
                    (this.selectedOrder as SupplyOrderViewModel).LoadPositions();
                }
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(SelectedOrder)));
                this.UpdateOrders();
                this.Refresh();
            }
        }

        private void OrdersOnCollectionChanged(object sender)
        {
            this.Orders.Refresh();
        }

        public ICollectionView Orders { get; set; }

        public string NavigationTitle => Resources.OrderText;
        

        public string NavigationGroup => Resources.MainText;

        public string NavigationItemName => "Orders";

        public List<RibbonTab> Tabs
        {
            get
            {



                var tab = new RibbonTab();
                var group = new RibbonGroup();
                
                
                var sparebutton = new RibbonButton
                {
                    Title = Properties.Resources.AddSparePartsOrderText,
                    Command = new DelegateCommand(this.AddSparePartsOrder),
                    ImgPath = "pack://application:,,,/Resources/Images/plus.png"
                };
                var supplyButton = new RibbonButton
                {
                    Title = Properties.Resources.AddSupplyOrderText,
                    Command = new DelegateCommand(this.AddSupplyOrder),
                    ImgPath = "pack://application:,,,/Resources/Images/plus.png"
                };
                var deleteButton = new RibbonButton
                {
                    Title = Properties.Resources.DeleteButtonText,
                    ImgPath = "pack://application:,,,/Resources/Images/fa-trash.png",
                    Command = new DelegateCommand(this.DeleteOrder)
                };

                group.Title = Properties.Resources.OrderText;
                if (this.user.Role == RoleEnum.Capitan)
                {
                    group.Buttons = new List<RibbonButton> {sparebutton, supplyButton, deleteButton};
                }
                if (this.user.Role == RoleEnum.Manager)
                {
                    group.Buttons = new List<RibbonButton> {deleteButton};
                }


                tab.Title = Properties.Resources.MainText;
                tab.Groups = new List<RibbonGroup> {group};
                
                return new List<RibbonTab> { tab };
            }
        }

        private bool CanDelete()
        {
            var canDelete = false;
            if(SelectedOrder.order != null)
            {
                if (this.user.Role == RoleEnum.Capitan)
                {
                    canDelete = this.SelectedOrder.order.Status == StatusEnum.New;
                }
                if (this.user.Role == RoleEnum.Manager)
                {

                    canDelete = (this.SelectedOrder.order.Status == StatusEnum.New ||
                                 this.SelectedOrder.order.Status == StatusEnum.ReadyToBeSent ||
                                 this.SelectedOrder.order.Status == StatusEnum.SentToTheOffice ||
                                 this.SelectedOrder.order.Status == StatusEnum.ReceivedAtOffice);
                }
            }
            return canDelete;
        }

        private void DeleteOrder(object obj)
        {
            if (!this.CanDelete())
            {
                System.Windows.MessageBox.Show(Resources.NotAppropriateStatusText);
                return;
            }
            var windowManager = IoC.Get<IWindowManager>();

            var closeOrder = new CloseOrderViewModel(this.SelectedOrder.order);
            windowManager.ShowDialog(closeOrder);
            if (closeOrder.Confirmed)
            {
                this.ordersProvider.SaveChanges(closeOrder.order);
                this.UpdateOrders();
            }
        }


        private void AddSupplyOrder(object obj)
        {
            this.ordersProvider.Add(new SupplyOrder(this.user) {Person = this.user.Name,PersonPost = this.user.Role.ToString(), Type = OrderType.Supply, Code = this.codesProvider.GetAll().FirstOrDefault() });
            this.UpdateOrders();
        }

        private void AddSparePartsOrder(object obj)
        {
            this.ordersProvider.Add(new SparePartsOrder(this.user) {Type = OrderType.SpareParts, PersonPost = this.user.Role.ToString(), Code = this.codesProvider.GetAll().FirstOrDefault()});
            this.UpdateOrders();
        }

        private void UpdateOrders()
        {
            
            var oldItems = this.orders.Select(o => o.order);
            var providerItems = this.ordersProvider.GetAll();

            if (this.user.Role == RoleEnum.Capitan)
            {
                providerItems = this.ordersProvider.GetAll(this.user.Vessel);
            }
            
            
            foreach (var newItem in providerItems.AsQueryable().Except(oldItems))
            {

                if (newItem is SparePartsOrder)
                {
                    this.orders.Add(new SparePartsOrderViewModel(newItem as SparePartsOrder, this.orderPositionProvider, this.codesProvider, this.user, this.ordersProvider, this.OrdersOnCollectionChanged, this.invoiceProvider,this.orderFilePostionsProvider, this.supplierProvider,this.storagePlacesProvider));
                }
                else if (newItem is SupplyOrder)
                {
                    this.orders.Add(new SupplyOrderViewModel(newItem as SupplyOrder, this.orderPositionProvider, this.codesProvider, this.user, this.ordersProvider, this.OrdersOnCollectionChanged, this.invoiceProvider, this.orderFilePostionsProvider,this.supplierProvider,this.storagePlacesProvider));
                }
            }

            foreach (var order in this.orders)
            {
                order.InvoicesUpdate();
                order.UpdateStoragePlaces();
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            this.UpdateOrders();
            this.SelectedOrder?.InvoicesUpdate();
            this.Orders.Refresh();
        }
    }
}