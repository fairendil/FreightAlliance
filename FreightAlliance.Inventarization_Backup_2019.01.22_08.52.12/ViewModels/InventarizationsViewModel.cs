using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FreightAlliance.Base.Models;
using FreightAlliance.Base.Providers;
using FreightAlliance.Common.Attributes;
using FreightAlliance.Common.Helpers;
using FreightAlliance.Common.Interfaces;
using Telerik.Windows.Controls;

namespace FreightAlliance.Inventarization.ViewModels
{
    using Base.Models;
    using Caliburn.Micro;
    using Common.Common;
    using Microsoft.Win32;
    using System;
    using System.Windows.Data;
    using System.ComponentModel.DataAnnotations;
    using System.IO;

    [Export("Inventarization", typeof(IScreen))]
    [Export(typeof(IScreen))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class InventarizationsViewModel : Screen, INavigationScreen, ICanRibbon
    {


        public string NavigationTitle => Properties.Resources.InventarizationText;
        public string NavigationItemName => "Inventarization";

        public string NavigationGroup => Properties.Resources.MainText;


        public readonly User User;

        [ImportingConstructor]
        public InventarizationsViewModel([Import("DataProvider")] IBaseProvider dataProvider,
                                                [Import("User")] IUser user)
        {
            this.User = (User) user;
            this.dataProvider = (DataProvider) dataProvider;
            this.inventarizationDate = DateTime.Today;
            this.inventarizations = new BindableCollection<OrderPositionViewModel>();
            this.Inventarizations = CollectionViewSource.GetDefaultView(this.inventarizations);
            this.AddWritenOffCommand = new DelegateCommand(this.AddWritenOff, o => this.CanAdd);
        }

        private bool CanAdd => this.selectedOrderPositionViewModel != null;

        private void AddWritenOff(object obj)
        {
            var windowManager = IoC.Get<IWindowManager>();

            var newWriteOffVeiwModel = new NewWriteOffViewModel(this.InventarizationDate);
            windowManager.ShowDialog(newWriteOffVeiwModel);
            if (newWriteOffVeiwModel.Confirmed)
            {
                this.dataProvider.WritenOffs.Add(new WritenOff()
                {
                    Date = newWriteOffVeiwModel.Date,
                    Quantity = newWriteOffVeiwModel.Quantity,
                    OrderPositionId = this.SelectedOrderPositionViewModel.Id
                });
                this.dataProvider.SaveChanges();
                this.Update();
            }
        }

        public OrderPositionViewModel SelectedOrderPositionViewModel {
            get { return this.selectedOrderPositionViewModel; }

            set
            {
                if (this.selectedOrderPositionViewModel == value)
                {
                    return;
                }
                this.selectedOrderPositionViewModel = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(SelectedOrderPositionViewModel)));
                this.AddWritenOffCommand.InvalidateCanExecute();
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanAdd)));
             
            } }

        public ICollectionView Inventarizations { get; set; }

        private readonly DataProvider dataProvider;
        private DateTime inventarizationDate;
        private readonly BindableCollection<OrderPositionViewModel> inventarizations;
        private OrderPositionViewModel selectedOrderPositionViewModel;


        public InventarizationsViewModel()
        {
        }


        public DateTime InventarizationDate
        {
            get
            {
                return this.inventarizationDate;
            }

            set
            {
                if (this.inventarizationDate == value)
                {
                    return;
                }
                this.inventarizationDate = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(InventarizationDate)));
                this.Update();
            }

        }

        private void Update()
        {
            
            this.inventarizations.Clear();
            var writeOffs = this.dataProvider.WritenOffs.Where(w => w.Date <= this.InventarizationDate).ToList().OrderBy(w => w.Date);
            var sparePartsOrderPositions = this.dataProvider.SparePartsOrderPosition.Where(o => o.Received).ToList();
            if (this.User.Role == RoleEnum.Capitan)
            {
                var orders = this.dataProvider.SparePartsOrder.Where(o => o.Vessel == this.User.Vessel).Select(o => o.OrderGuid).ToList();
                sparePartsOrderPositions = sparePartsOrderPositions.Where(o => orders.Contains(o.OrderGuid)).ToList();
            }
            foreach (var sparePartsOrderPosition in sparePartsOrderPositions)
            {
                var writeOff = writeOffs.Where(w => w.OrderPositionId == sparePartsOrderPosition.OrderPositionId).Sum(w => w.Quantity);
                if ((sparePartsOrderPosition.Quantity - writeOff) > 0)
                {
                    this.inventarizations.Add(new OrderPositionViewModel(sparePartsOrderPosition,
                        writeOff,
                        this.dataProvider));
                }
            }
            this.Inventarizations.Refresh();
        }

        [Display(AutoGenerateField = false)]
        public DelegateCommand AddWritenOffCommand { get; set; }

        protected override void OnActivate()
        {
            base.OnActivate();
            this.Update();
            
        }

        public List<RibbonTab> Tabs { get; }
    }


}




