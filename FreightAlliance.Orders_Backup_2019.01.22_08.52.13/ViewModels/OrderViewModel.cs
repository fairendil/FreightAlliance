using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;
using FreightAlliance.Common.Attributes;
using FreightAlliance.Common.Extensions;
using FreightAlliance.Common.Helpers;
using FreightAlliance.Orders.Properties;
using Microsoft.Win32;
using Telerik.Windows.Controls;

namespace FreightAlliance.Orders.ViewModels
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Data;

    using Caliburn.Micro;

    using FreightAlliance.Base.Models;
    using FreightAlliance.Common.Common;
    using FreightAlliance.Common.Enums;
    using FreightAlliance.Common.Interfaces;
    using Telerik.Reporting.Processing;
    using Reports;

    public class OrderViewModel : ViewModelBase
    {
        public readonly Order order;

        protected readonly IOrderPostionsProvider<OrderPosition> orderPositionProvider;

        private readonly IDataProvider<Code> codesProvider;
        public readonly User User;

        private readonly IEnumerable<Code> codes;
        private IEnumerable<Invoice> invoices;
        private IDataProvider<Order> orderProvider;
        public System.Action report;
        private OrderFilePositionViewModel selectedOrderFile;
        protected Action<object> ordersOnCollectionChanged;
        private IDataProvider<Invoice> invoiceProvider;
        private IDataProvider<OrderFilePosition> orderFilePositionsProvider;
        private ObservableCollection<OrderFilePositionViewModel> orderFiles;
        private IDataProvider<Supplier> supplierProvider;
        private IEnumerable<Supplier> suppliers;
        private List<OrderFilePositionViewModel> orderFilePositionsViews;

        public OrderViewModel()
        {
            
        }

        public OrderViewModel(Order order, IOrderPostionsProvider<OrderPosition> orderPositionProvider, IDataProvider<Code> codesProvider, User user, IDataProvider<Order> ordersProvider, IDataProvider<Invoice> invoiceProvider, IDataProvider<OrderFilePosition> orderFilePositionsProvider, IDataProvider<Supplier> supplierProvider, IDataProvider<StoragePlace> storagePlacesProvider)
        {
            this.order = order;
            this.orderPositionProvider = orderPositionProvider;
            this.codesProvider = codesProvider;
            this.invoiceProvider = invoiceProvider;
            this.orderFilePositionsProvider = orderFilePositionsProvider;
            this.supplierProvider = supplierProvider;
            this.User = user;

            this.codes = this.codesProvider.GetAll();
            this.invoices = this.invoiceProvider.GetAll().Where(i => i.OrderGuid == Guid.Empty);
            this.suppliers = this.supplierProvider.GetAll();
            this.storagePlacesProvider = storagePlacesProvider;
            this.orderProvider = ordersProvider;
            //this.orderFilePositionsViews =
            //    this.order.FilePositions.Select(o => new OrderFilePositionViewModel(o)).ToList();
            this.orderFiles = new ObservableCollection<OrderFilePositionViewModel>();
            this.OrderFiles = CollectionViewSource.GetDefaultView(this.orderFiles);
            this.FileSelected = true;
            this.LoadFilePositions();

            this.DeleteFileCommand = new DelegateCommand(this.DeleteFile, o => this.CanDeleteFile);
            
            this.SelectFileCommand = new DelegateCommand(this.SelectFile,o => this.CanSelectFile);
            this.PromoteCommand = new DelegateCommand(this.Promote, o => this.CanPromote);
            this.PrintReportCommand = new DelegateCommand(this.PrintReport);

            this.StoragePlaces = new ObservableCollection<StoragePlace>();
            this.Positions = new BindableCollection<string>
            {
                Resources.Master,
                Resources.Chief_engineer,
                Resources._2_engineer,
                Resources._3_engineer,
                Resources._4_engineer,
                Resources.Chief_officer,
                Resources._2Mate,
                Resources._3Mate,
                Resources.Cook,
                Resources.Superintendant
            };
            this.UpdateStoragePlaces();
        }

        public BindableCollection<string> Positions { get; set; }


        [Display(AutoGenerateField = false)]
        public DelegateCommand PromoteCommand { get; set; }

        [Display(AutoGenerateField = false)]
        public DelegateCommand SelectFileCommand { get; set; }

        [Display(AutoGenerateField = false)]
        public DelegateCommand AddFileCommand { get; set; }

        [Display(AutoGenerateField = false)]
        public DelegateCommand DeleteFileCommand { get; set; }



        [Display(AutoGenerateField = false)]
        public int Id
        {
            get
            {
                return this.order.OrderId;
            }
        }

        [Display(Name = "CommentsColumnText", Order = 11, ResourceType = typeof(Resources))]
        public string Comment
        {
            get
            {
                return this.order.Comment;
            }

            set
            {
                if (this.order.Comment == value)
                {
                    return;
                }
                if (value.Length > 255)
                {
                    throw new ValidationException("The Comment value cannot exceed 255 characters.");
                }
                this.order.Comment = value;
                this.OnPropertyChanged(nameof(Comment));
            }
        }

        [Display(AutoGenerateField = false)]
        public IEnumerable<Code> Codes
        { 
            get
            {
                return this.codes;
            }
        }
        [Display(AutoGenerateField = false)]
        public IEnumerable<Invoice> Invoices
        { 
            get
            {
                return this.invoices;
            }
        }
        [Display(AutoGenerateField = false)]
        public IEnumerable<Supplier> Suppliers
        {
            get
            {
                return this.suppliers;
            }
        }


        [Display(Name = "CodeText", Order = 1, ResourceType = typeof(Resources))]
        public int Code
        {
            get
            {
                return this.order.Code.Number;
            }

            set
            {
                if (this.order.Code.Number == value) return;
                var code = this.codes.FirstOrDefault(c => c.Number.ToString().Contains(value.ToString()));
                if (code == null) return;
                this.order.Code = code;
                this.OnPropertyChanged(nameof(Code));
                this.OnPropertyChanged(nameof(CodeType));
            }
        }

        [Display(Name = "CodeTypeText", Order = 1, ResourceType = typeof(Resources))]
        public string CodeType
        {
            get
            {
                switch (this.order.Code.Number / 100)
                {
                    case 71:
                        return "Crew needs";
                    case 72:
                        return "Store/Consumable";
                    case 73:
                        return "Spare Parts";
                    case 74:
                        return "Repair and Maintenance";
                    default:
                        return ("");
                }
            }

        }


        [Display(Name = "CreationDateText", Order = 4, ResourceType = typeof(Resources))]
        public DateTime? CreationDate
        {
            get
            {
                return this.order.CreationDate >= DateTime.Now.AddYears(-30) ?  this.order.CreationDate : (DateTime?) null;
            }
        }

        [Display(Name = "DeleteDateColumnText", Order = 13, ResourceType = typeof(Resources))]
        public DateTime? DeleteDate
        {
            get
            {
                return this.order.DeleteDate >= DateTime.Now.AddYears(-30) ? this.order.DeleteDate : (DateTime?)null;
            }
        }

        [Display(Name = "DeleteReasonColumnText", Order = 12, ResourceType = typeof(Resources))]
        [StringLength(255)]
        public string DeleteReason
        {
            get
            {
                return this.order.DeleteReason;
            }

            set
            {
                if (this.order.DeleteReason != value)
                {
                    if (value.Length > 255)
                    {
                        throw new ValidationException("The Comment value cannot exceed 255 characters.");
                    }

                    this.order.DeleteReason = value;
                    this.OnPropertyChanged(nameof(DeleteReason));
                }
            }
        }

        [Display(Name = "InvoiceText", Order = 7, ResourceType = typeof(Resources))]
        public Invoice Invoice
        {
            get
            {
                return this.order.Invoice;
            }

            set
            {
                if (this.order.Invoice == value)
                {
                    return;
                }

                if (this.order.Invoice != null)
                {
                    this.order.Invoice.OrderGuid = Guid.Empty;
                }

                this.order.Invoice = value;
                this.OnPropertyChanged();
            }
        }

        [Display(Name = "OrderNumberColumnText", Order = 0, ResourceType = typeof(Resources))]
        public Number Number
        {
            get
            {
                return this.order.Number;
            }
        }

        [Display(AutoGenerateField = false)]
        public ICollectionView OrderPositions { get; set; }

        [Display(Name = "ReceivedAtOfficeDateText", Order = 11, ResourceType = typeof(Resources))]
        public DateTime? ReceivedAtOfficeDate
        {
            get
            {
                return this.order.ReceivedAtOfficeDate >= DateTime.Now.AddYears(-30) ? this.order.ReceivedAtOfficeDate : (DateTime?)null; 
            }
            internal set { this.order.ReceivedAtOfficeDate = (DateTime) value; }
        }

        [Display(Name = "ReceivedAtVesselDateText", Order = 5, ResourceType = typeof(Resources))]
        public DateTime? ReceivedAtVesselDate
        {
            get
            {
                return this.order.ReceivedAtVesselDate >= DateTime.Now.AddYears(-30) ? this.order.ReceivedAtVesselDate : (DateTime?)null; 
            }
            internal set { this.order.ReceivedAtVesselDate = (DateTime) value; }
        }

        [Display(Name = "SentToOfficeDateText", Order = 3, ResourceType = typeof(Resources))]
        public DateTime? SentToOfficeDate
        {
            get
            {
                return this.order.SentToOfficeDate >= DateTime.Now.AddYears(-30) ? this.order.SentToOfficeDate : (DateTime?)null; ;
            }
            internal set { this.order.SentToOfficeDate = (DateTime) value; }
        }


        
        [Display(Name = "OrderStatus", Order = 2, ResourceType = typeof(Resources))]
        public StatusEnum Status
        {
            get
            {
                return this.order.Status;
            }

            internal set
            {
                if (this.order.Status == value)
                {
                    return;
                }

                this.order.Status = value;
                this.OnPropertyChanged(nameof(Status));
                this.OnPropertyChanged("CanAddOrderPosition");
                this.OnPropertyChanged("CanDeleteOrderPosition");
            }
        }

        [Display(Name = "SupplierColumnText", Order = 6, ResourceType = typeof(Resources))]
        public Supplier Supplier
        {
            get
            {
                return this.order.Supplier;
            }

            set
            {
                if (this.order.Supplier == value)
                {
                    return;
                }
                if (this.order.Supplier != null)
                {
                    if (this.order.Supplier.OrderGuids.Contains(this.order.OrderGuid))
                    {
                        this.order.Supplier.OrderGuids.Remove(this.order.OrderGuid);
                        this.supplierProvider.SaveChanges(this.order.Supplier);
                    }
                }
                this.order.Supplier = value;
                this.OnPropertyChanged();
            }

        }

        
        [Display(Name = "OrderTypeColumnText", Order = 2, ResourceType = typeof(Resources))]
        public OrderType Type
        {
            get
            {
                return this.order.Type;
            }
        }

        [Display(Name = "VesselText", Order = 2, ResourceType = typeof(Resources))]
        
        public string Vessel
        {
            get
            {
                return this.order.Vessel;
            }

            internal set
            {
                if (this.order.Vessel == value)
                {
                    return;
                }

                this.order.Vessel = value;
                this.OnPropertyChanged(nameof(Vessel));
            }
        }

        [Display(Name = "PersonPostText", Order = 2, ResourceType = typeof(Resources))]
        public string PesonPost
        {
            get
            {
                return this.order.PersonPost;
            }

            set
            {
                if (this.order.PersonPost == value)
                {
                    return;
                }

                this.order.PersonPost = value;
                this.OnPropertyChanged(nameof(PesonPost));
            }
        }

        public void PrintReport(object o)
        {
            Task.Factory.StartNew(this.report);
        }

        public void SaveChanges()
        {
            if (this.Invoice != null)
            {
                this.order.Invoice.OrderGuid = this.order.OrderGuid;
                this.invoiceProvider.SaveChanges(this.order.Invoice);
            }
            if (this.Supplier != null && !string.IsNullOrEmpty(this.Supplier.Name))
            {
                this.order.Supplier.OrderGuids.Add(this.order.OrderGuid);
                this.supplierProvider.SaveChanges(this.order.Supplier);
            }
            this.orderProvider.SaveChanges(this.order);
        }

        public void Promote(object o)
        {
            if (this.Status == StatusEnum.Confirmed)
            {
                this.ReceivedAtVesselDate = DateTime.Now;
            }
            if (this.Status == StatusEnum.SentToTheOffice)
            {
                
                this.ReceivedAtOfficeDate = DateTime.Now;
            }
            if (this.Status == StatusEnum.ReadyToBeSent)
            {

                if (this.OrderPositions.IsEmpty)
                {
                    System.Windows.MessageBox.Show(Resources.NeedOrderPositionText);
                    return;
                }
                else
                {
                    this.SentToOfficeDate = DateTime.Now;

                }
                
            }
            this.Status += 1;
            this.OnPropertyChanged(nameof(Status));
            this.OnPropertyChanged(nameof(PromoteName));
            this.PromoteCommand.InvalidateCanExecute();
            this.OnPropertyChanged(nameof(CanPromote));
            this.OnPropertyChanged(nameof(UserCanAddFiles));
            this.SaveChanges();
            this.ordersOnCollectionChanged.Invoke(this);
        }

        [Display(AutoGenerateField = false)]
        public string PromoteName
        {
            get
            {
                if (this.Status == StatusEnum.New && this.CanPromote)
                {
                    return FreightAlliance.Orders.Properties.Resources.ConfirmText;
                }
                if (this.Status == StatusEnum.ReadyToBeSent && this.CanPromote)
                {
                    return FreightAlliance.Orders.Properties.Resources.SendText;
                }
                if (this.Status == StatusEnum.SentToTheOffice && this.CanPromote)
                {
                    return Properties.Resources.ConfirmReceiptText;
                }

                if (this.Status == StatusEnum.ReceivedAtOffice && this.CanPromote)
                {
                    return Properties.Resources.SendForQuotationText;
                }

                if (this.Status == StatusEnum.SentForQuotation)
                {
                    return Properties.Resources.SelectInvoiceText;
                }

                if (this.Status == StatusEnum.Confirmed && this.CanPromote)
                {
                    return Properties.Resources.ConfirmReceiptText;
                }
                if (this.Status == StatusEnum.Received)
                {
                    return Properties.Resources.NextStatusText;
                }
                return string.Empty;
            }
        }


        [Display(AutoGenerateField = false)]
        public bool CanPromote
        {
            get
            {
                if (this.Status == StatusEnum.Received)
                {
                    if (this.order is SparePartsOrder)
                    {
                        return (this as SparePartsOrderViewModel).orderPositions.Any(p => p.Received);
                    }
                    return this.Status.GetAttribute<PromoteRoles>().UserRoles.Contains(this.User.Role);
                }
                return this.Status.GetAttribute<PromoteRoles>().UserRoles.Contains(this.User.Role);
            }
        }

        [Display(AutoGenerateField = false)]
        public bool FileSelected { get; set; }

        public void OpenFile()
        {
            if (SelectedOrderFile == null) return;
            try
            {
                System.Diagnostics.Process.Start(this.SelectedOrderFile.orderPosition.Name);
            }
            catch (Exception)
            {

            }
        }

        [Display(AutoGenerateField = false)]
        public bool CanAddFile => this.FileSelected && this.User.Role == RoleEnum.Manager && this.Status == StatusEnum.SentForQuotation;

        public void AddFile(object o)
        {
            var filedialog = new OpenFileDialog();
            var isOk = filedialog.ShowDialog();
            if (isOk != true) return;
            var fileName = filedialog.FileName;
            string fileNewName;
            try
            {
                System.IO.File.Copy(fileName,
                    Path.Combine(System.IO.Directory.GetCurrentDirectory(), this.order.ToString(),
                        Path.GetFileNameWithoutExtension(fileName) + Path.GetExtension(fileName)));
                fileNewName = Path.Combine(System.IO.Directory.GetCurrentDirectory(),
                    this.order.ToString(),
                    Path.GetFileNameWithoutExtension(fileName)+ Guid.NewGuid() + Path.GetExtension(fileName));
            }
            catch (DirectoryNotFoundException ex)
            {
                System.IO.Directory.CreateDirectory(Path.Combine(System.IO.Directory.GetCurrentDirectory(),
                    this.order.ToString()));
                System.IO.File.Copy(fileName,
                    Path.Combine(System.IO.Directory.GetCurrentDirectory(), this.order.ToString(),
                        Path.GetFileNameWithoutExtension(fileName) + Path.GetExtension(fileName)));
                fileNewName = Path.Combine(System.IO.Directory.GetCurrentDirectory(),
                    this.order.ToString(),
                    Path.GetFileNameWithoutExtension(fileName) + Path.GetExtension(fileName));
            }
            catch (IOException ex)
            {
                var nw = Guid.NewGuid().ToString();
                System.IO.File.Copy(fileName,
                    Path.Combine(System.IO.Directory.GetCurrentDirectory(), this.order.ToString(),
                        Path.GetFileNameWithoutExtension(fileName) + nw  + Path.GetExtension(fileName)));
                fileNewName = Path.Combine(System.IO.Directory.GetCurrentDirectory(),
                    this.order.ToString(),
                    Path.GetFileNameWithoutExtension(fileName) + nw + Path.GetExtension(fileName));
            }
            var max = this.orderFiles?.ToList().Count + 1 ?? 1;
            var newFilePosition = new OrderFilePosition {Name = fileNewName, Number = max, OrderGuid = this.order.OrderGuid, OrderId = this.order.OrderId};
            if (this.order.FilePositions != null)
            {
                this.order.FilePositions.Add(newFilePosition);
                var orderFilePositionViewModels = this.orderFiles;
                if (orderFilePositionViewModels != null) orderFilePositionViewModels.Add(new OrderFilePositionViewModel(newFilePosition));
            }
            else
            {
                this.order.FilePositions = new List<OrderFilePosition>();
                this.order.FilePositions.Add(newFilePosition);
                var orderFilePositionViewModels = this.orderFiles;
                if (orderFilePositionViewModels != null) orderFilePositionViewModels.Add(new OrderFilePositionViewModel(newFilePosition));
            }
            //this.orderFilePositionsViews.Add(new OrderFilePositionViewModel(newFilePosition));
            this.OnPropertyChanged(nameof(orderFiles));
            
            this.SaveChanges();

            this.OnPropertyChanged(nameof(OrderFiles));
            this.OnPropertyChanged(nameof(orderFiles));
        }

        [Display(AutoGenerateField = false, AutoGenerateFilter = false)]
        public ICollectionView OrderFiles { get; set; }

        [Display(AutoGenerateField = false, AutoGenerateFilter = false)]
        public OrderFilePositionViewModel SelectedOrderFile {
            get { return this.selectedOrderFile; }
            set
            {
                if (this.selectedOrderFile == value)
                {
                    return;
                }
                this.selectedOrderFile = value;
                this.DeleteFileCommand.InvalidateCanExecute();
                this.SelectFileCommand.InvalidateCanExecute();
                this.OnPropertyChanged(nameof(SelectedOrderFile));
                this.OnPropertyChanged(nameof(CanSelectFile));
                this.OnPropertyChanged(nameof(CanDeleteFile));
            } }

        [Display(AutoGenerateField = false, AutoGenerateFilter = false)]
        public bool CanSelectFile => (this.SelectedOrderFile != null) && this.FileSelected;

        [Display(AutoGenerateField = false)]
        public bool UserCanAddFiles
        {
            get { return this.Status.GetAttribute<ViewFilesRole>().UserRoles.Contains(this.User.Role); }
        }

        [Display(AutoGenerateField = false)]
        public bool CanDeleteFile
        {
            get { return this.CanSelectFile; }
        }

        [Display(AutoGenerateField = false)]
        public DelegateCommand PrintReportCommand { get; set; }

        [Display(AutoGenerateField = false)]
        public bool IsEditable
        {
            get { return this.Status.GetAttribute<EditRoles>().UserRoles.Contains(this.User.Role); }
        }

        [Display(AutoGenerateField = false)]
        public bool CanAddOrderPosition
        {
            get
            {
                return this.Status != StatusEnum.Confirmed && this.Status != StatusEnum.Capitalized &&
                       this.Status != StatusEnum.Closed && this.order.Status != StatusEnum.Received;
            }
        }
        public void SelectFile(object o)
        {
            this.FileSelected = false;
            this.SelectedOrderFile.Cheked = true;

            this.DeleteFileCommand.InvalidateCanExecute();
            this.SelectFileCommand.InvalidateCanExecute();
            this.AddFileCommand.InvalidateCanExecute();
            this.OnPropertyChanged(nameof(CanAddFile));
            this.OnPropertyChanged(nameof(CanSelectFile));
            this.OnPropertyChanged(nameof(CanDeleteFile));
            this.OnPropertyChanged(nameof(OrderFiles));
            this.Promote(o);
            this.SaveChanges();
        }

        public  void DeleteFile(object o)
        {
            this.orderFiles.Remove(this.SelectedOrderFile);
            this.DeleteFileCommand.InvalidateCanExecute();
            this.SelectFileCommand.InvalidateCanExecute();
            this.OnPropertyChanged(nameof(CanSelectFile));
            this.OnPropertyChanged(nameof(CanDeleteFile));
            this.OnPropertyChanged(nameof(OrderFiles));
            this.SaveChanges();
        }

        public void InvoicesUpdate()
        {
            if (order != null)
            {
                this.invoices =
                    this.invoiceProvider.GetAll()
                        .Where(i => i.OrderGuid == order.OrderGuid || i.OrderGuid == Guid.Empty);
                this.suppliers = this.supplierProvider.GetAll();
                this.OnPropertyChanged(nameof(Invoices));
                this.OnPropertyChanged(nameof(Suppliers));
            }
        }

        private void LoadFilePositions()
        {
            var oldItems = new List<int>();
            oldItems.AddRange(this.orderFiles?.Select(m => m.Id));

            var providerItems = this.orderFilePositionsProvider.GetByOrderId(this.order.OrderGuid).Select(o => o.OrderFilePositionId);
            foreach (var item in providerItems.Except(oldItems))
            {
                var newItem = this.orderFilePositionsProvider.GetById(item);

                    this.orderFiles.Add(new OrderFilePositionViewModel(newItem));
            }
            orderFiles?.ToList().ForEach(f => this.FileSelected = !(FileSelected && f.Cheked));
        }

        public void UpdateStoragePlaces()
        {
            var oldInv = this.StoragePlaces.Select(i => i.StoragePlacePK);
            var providerInv = this.storagePlacesProvider.GetAll().Select(inv => inv.StoragePlacePK);

            foreach (var item in providerInv.Except(oldInv))
            {
                this. StoragePlaces.Add(this.storagePlacesProvider.GetById(item));
            }
            this.OnPropertyChanged(nameof(StoragePlaces));
        }

        [Display(AutoGenerateField = false)]
        public ObservableCollection<StoragePlace> StoragePlaces { get; set; }
        

        private readonly IDataProvider<StoragePlace> storagePlacesProvider;
    }
}