using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using Caliburn.Micro;
using FreightAlliance.Common.Enums;
using FreightAlliance.Common.Extensions;
using Telerik.Windows.Controls;

namespace FreightAlliance.Orders.ViewModels
{
    using FreightAlliance.Base.Models;
    using Base.Providers;
    using FreightAlliance.Common.Interfaces;
    using Reports;
    using System.IO;
    using Telerik.Reporting.Processing;
    using System.Collections.Generic;
    using FreightAlliance.Orders.Properties;
    using System.Windows;
    using System;

    public class SupplyOrderViewModel : OrderViewModel
    {
        private readonly SupplyOrder order;
        private SupplyOrderPositionViewModel selectedOrderPosition;
        

        [Display(AutoGenerateField = false)]
        public  BindableCollection<SupplyOrderPositionViewModel> orderPositions { get; set; }

        public SupplyOrderViewModel()
        {
            
        }
        public SupplyOrderViewModel(SupplyOrder order, IOrderPostionsProvider<OrderPosition> orderPositionProvide, IDataProvider<Code> codesProvider, User user, IDataProvider<Order> ordersProvider, Action<object> ordersOnCollectionChanged, IDataProvider<Invoice> invoiceProvider, IDataProvider<OrderFilePosition> orderFilePositionsProvider, IDataProvider<Supplier> supplierProvider, IDataProvider<StoragePlace> storagePlacesProvider)
            : base(order, orderPositionProvide, codesProvider, user, ordersProvider, invoiceProvider, orderFilePositionsProvider, supplierProvider, storagePlacesProvider)
        {
            this.order = order;
            this.orderPositions = new BindableCollection<SupplyOrderPositionViewModel>();
            this.OrderPositions = CollectionViewSource.GetDefaultView(this.orderPositions);
            this.LoadPositions();
            base.report = this.Report;
            base.ordersOnCollectionChanged = ordersOnCollectionChanged;
            this.AddOrderPositionCommand = new DelegateCommand(this.AddOrderPosition,o => CanAddOrderPosition);
            this.DeleteOrderPositionCommand = new DelegateCommand(this.DeleteOrderPosition, o => this.CanDeleteOrderPosition);
            //this.order.Code = this.codes.FirstOrDefault();
            this.AddFileCommand = new DelegateCommand(this.AddFile, o => this.CanAddFile);


        }

        [Display(AutoGenerateField = false)]
        public DelegateCommand DeleteOrderPositionCommand { get; set; }

        [Display(AutoGenerateField = false)]
        public DelegateCommand AddOrderPositionCommand { get; set; }

        [Display(Name = @"PersonNameText", Order = 7, ResourceType = typeof(Resources))]
        public string Person {
            get
            {
                return this.order.Person.ToString();
  
            }

            set
            {
                if(this.Person.Equals(value))
                {
                    return;
                }

                this.order.Person = value;
                this.OnPropertyChanged(nameof(Person));
            }
                
            }

        [Display(AutoGenerateField = false)]
        public SupplyOrderPositionViewModel SelectedOrderPosition
        {
            get { return this.selectedOrderPosition; }
            set {
                if (this.selectedOrderPosition == value)
                {
                    return;
                }
                this.selectedOrderPosition = value;
                this.OnPropertyChanged(nameof(SelectedOrderPosition));
                this.DeleteOrderPositionCommand.InvalidateCanExecute();
                this.OnPropertyChanged(nameof(CanDeleteOrderPosition));
            }
        }

        public void LoadPositions()
        {
            var oldItems = this.orderPositions?.Select(m => m.Id) ?? new List<Guid>();
            
            var providerItems = (base.orderPositionProvider as OrderPositionsProvider).GetByOrderId(this.order.OrderGuid).Select(o => o.OrderPositionGuid);

            foreach (var item in providerItems.Except(oldItems))
            {
                var newItem = this.orderPositionProvider.GetByGuid(item);
                if (newItem is SupplyOrderPosition)
                {
                    this.orderPositions.Add(new SupplyOrderPositionViewModel(newItem as SupplyOrderPosition, this.Type));
                }
            }

        }

        public void Report()
        {
            var ord = this.order;
            if (ord is SupplyOrder)
            {
                ReportProcessor reportProcessor = new ReportProcessor();
                Telerik.Reporting.InstanceReportSource instanceReportSource = new Telerik.Reporting.InstanceReportSource();
                instanceReportSource.ReportDocument = new OrderReportCopy(this as SupplyOrderViewModel);
                RenderingResult result = reportProcessor.RenderReport("XLS", instanceReportSource, null);
                var fileName = Path.GetTempFileName() + ".XLS";
                var file = new FileInfo(fileName);
                if (file.Exists)
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (IOException ex)
                    {
                        System.Windows.Forms.MessageBox.Show("Please close previous report.");
                    }

                }

                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
                    fs.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);
                }

                System.Diagnostics.Process.Start(fileName);


            }
        }

        public void AddOrderPosition(object o)
        {
            var orderposition = new SupplyOrderPosition() {OrderGuid = this.order.OrderGuid};
            if (this.order.OrderPositions == null)
            {
                this.order.OrderPositions = new List<OrderPosition>();
            }
            this.order.OrderPositions.Add(orderposition);
            this.orderPositions.Add(new SupplyOrderPositionViewModel(orderposition, OrderType.SpareParts));
        }

        public void DeleteOrderPosition(object o)
        {
            this.orderPositions.Remove(this.SelectedOrderPosition);
        }
        [Display(AutoGenerateField = false)]
        public bool CanDeleteOrderPosition
        {
            get { return (this.SelectedOrderPosition != null && this.CanAddOrderPosition); }
        }
    }
}
