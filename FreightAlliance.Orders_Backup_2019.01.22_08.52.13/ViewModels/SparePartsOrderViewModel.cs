using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using Caliburn.Micro;
using FreightAlliance.Common.Enums;
using FreightAlliance.Orders.Properties;
using Telerik.Windows.Controls;

namespace FreightAlliance.Orders.ViewModels
{
    using FreightAlliance.Base.Models;
    using FreightAlliance.Common.Interfaces;
    using Reports;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Forms;
    using System.Windows.Threading;
    using Telerik.Reporting.Processing;
    using System.Collections.Specialized;

    public class SparePartsOrderViewModel : OrderViewModel
    {
        private readonly SparePartsOrder order;
        private Action<object> ordersOnCollectionChanged;
        private SparePartsOrderPositionViewModel selectedOrderPosition;
        


        [Display(AutoGenerateField = false)]
        public ObservableCollection<SparePartsOrderPositionViewModel> orderPositions { get; set; }

        public SparePartsOrderViewModel(SparePartsOrder order, IOrderPostionsProvider<OrderPosition> orderPositionProvide, IDataProvider<Code> codesProvider, User user, IDataProvider<Order> ordersProvider, Action<object> ordersOnCollectionChanged, IDataProvider<Invoice> invoiceProvider, IDataProvider<OrderFilePosition> orderFilePositionsProvider, IDataProvider<Supplier> supplierProvider, IDataProvider<StoragePlace> storagePlacesProvider)
            : base(order, orderPositionProvide, codesProvider, user, ordersProvider,invoiceProvider, orderFilePositionsProvider, supplierProvider, storagePlacesProvider)
        {
            base.report = this.Report;
            this.order = order;
            this.orderPositions = new ObservableCollection<SparePartsOrderPositionViewModel>();
            this.OrderPositions = CollectionViewSource.GetDefaultView(this.orderPositions);
            this.LoadPositions();
            base.ordersOnCollectionChanged = ordersOnCollectionChanged;
            
            this.AddOrderPositionCommand = new DelegateCommand(this.AddOrderPosition, o => this.CanAddOrderPosition );
            this.DeleteOrderPositionCommand = new DelegateCommand(this.DeleteOrderPosition, o => this.CanDeleteOrderPosition);
            this.AddFileCommand = new DelegateCommand(this.AddFile, o => this.CanAddFile);
        }

        [Display(AutoGenerateField = false)]
        public DelegateCommand DeleteOrderPositionCommand { get; set; }

        [Display(AutoGenerateField = false)]
        public DelegateCommand AddOrderPositionCommand { get; set; }

        [Display(Name = "DrawingText", Order = 10,ResourceType = typeof(Resources))]
        public string Drawing
        {
            get
            {
                return this.order.Drawing;
                
            }
            set
            {
                if (this.order.Drawing.Equals(value))
                {
                    return;
                }
                this.order.Drawing = value;
                this.OnPropertyChanged(nameof(Drawing));
            }
        }
        [Display(Name = "PartSerialNumberText", Order = 10, ResourceType = typeof(Resources))]
        public string PartSerialNumber
        {
            get
            {
                return this.order.PartSerialNumber;
            }

            set
            {
                if (this.order.PartSerialNumber.Equals(value))
                {
                    return;
                }

                this.order.PartSerialNumber = value;
                this.OnPropertyChanged();
            }
        }

        [Display(Name = "ManufacturedText", Order = 10, ResourceType = typeof(Resources))]
        public string Manufactured
        {
            get
            {
                return this.order.Manufactured;

            }
            set
            {
                if (this.order.Manufactured.Equals(value))
                {
                    return;
                }
                this.order.Manufactured = value;
                this.OnPropertyChanged(nameof(Manufactured));
            }
        }

        [Display(Name = "ManufacturedYearText", Order = 10, ResourceType = typeof(Resources))]
        public DateTime? ManufacturedYear
        {
            get
            {
                return this.order.ManufacturedYear >= DateTime.Now.AddYears(-30) ? this.order.ManufacturedYear : (DateTime?)null;

            }
            set
            {
                if (this.order.ManufacturedYear.Equals(value) || value == null)
                {
                    return;
                }
                this.order.ManufacturedYear = (DateTime) value;
                this.OnPropertyChanged(nameof(ManufacturedYear));
            }
        }

        [Display(Name = "PlateText", Order = 10, ResourceType = typeof(Resources))]
        public string Plate
        {
            get
            {
                return this.order.Plate;

            }
            set
            {
                if (this.order.Plate.Equals(value))
                {
                    return;
                }
                this.order.Plate = value;
                this.OnPropertyChanged(nameof(Plate));
            }
        }


        [Display(Name = "SupplyForText", Order = 10, ResourceType = typeof(Resources))]
        public string SupplyFor
        {
            get
            {
                return this.order.SuplyFor;

            }
            set
            {
                if (this.order.SuplyFor.Equals(value))
                {
                    return;
                }
                this.order.SuplyFor = value;
                this.OnPropertyChanged(nameof(SupplyFor));
            }
        }


        [Display(AutoGenerateField = false)]
        public SparePartsOrderPositionViewModel SelectedOrderPosition
        {
            get { return this.selectedOrderPosition; }
            set
            {
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
            var oldItems = new List<Guid>();
            oldItems.AddRange(this.orderPositions?.Select(m => m.Id));

            var providerItems = base.orderPositionProvider.GetByOrderId(this.order.OrderGuid).Select(o => o.OrderPositionGuid);
            foreach (var item in providerItems.Except(oldItems))
            {
                var newItem = this.orderPositionProvider.GetByGuid(item);
                if (newItem is SparePartsOrderPosition)
                {
                        this.orderPositions.Add(new SparePartsOrderPositionViewModel(newItem as SparePartsOrderPosition, this.Type));                  
                }
            }
            foreach (var item in providerItems.Intersect(oldItems))
            {
                var oldItem = this.orderPositionProvider.GetByGuid(item) as SparePartsOrderPosition;
                var oldView = this.orderPositions.FirstOrDefault(p => p.Id == item);
                oldView.Received = oldItem.Received;
                oldView.StoragePlace = oldItem.StoragePlace;
            }

        }

        public  void Report()
        {
            var ord = this.order;
            if (ord is SparePartsOrder)
            {
                ReportProcessor reportProcessor = new ReportProcessor();
                Telerik.Reporting.InstanceReportSource instanceReportSource = new Telerik.Reporting.InstanceReportSource();
                instanceReportSource.ReportDocument = new OrderReport(this);
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
                        MessageBox.Show("Please close previous report.");
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
            var orderposition = new SparePartsOrderPosition() { OrderGuid = this.order.OrderGuid };
            if (this.order.OrderPositions == null)
            {
                this.order.OrderPositions = new List<OrderPosition>();
            }
            this.order.OrderPositions.Add(orderposition);
            this.orderPositions.Add(new SparePartsOrderPositionViewModel(orderposition, OrderType.SpareParts));
                
        }

        public void DeleteOrderPosition(object o)
        {
            this.orderPositions.Remove(this.SelectedOrderPosition);
        }

        [Display(AutoGenerateField = false)]
        public bool CanDeleteOrderPosition {
            get { return this.SelectedOrderPosition != null && this.CanAddOrderPosition; }
        }

    }
}
