using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using FreightAlliance.Base.Models;
using FreightAlliance.Common.Attributes;
using FreightAlliance.StoragePlaces.ViewModels;

namespace FreightAlliance.StoragePlaces.ViewModels
{
    using Caliburn.Micro;

    using FreightAlliance.Common.Helpers;
    using FreightAlliance.Common.Interfaces;

    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System;

    using Telerik.Windows.Controls;

    [Export("StoragePlaces", typeof(IScreen))]
    [Export(typeof(IScreen))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class StoragePlacesViewModel : Screen, INavigationScreen, ICanRibbon
    {
        private User user;
        private IDataProvider<Base.Models.StoragePlace> StoragePlacesProvider;
        private BindableCollection<StoragePlaceViewModel> storagePlaces;
        public string NavigationTitle => Properties.Resources.StoragePlaceText;

        public string NavigationItemName => "StoragePlaces";



        public string NavigationGroup
        {
            get
            {
                return Properties.Resources.MainText;
            }
        }

        public List<RibbonTab> Tabs
        {
            get
            {

                var tab = new RibbonTab();
                var group = new RibbonGroup();

                var sparebutton = new RibbonButton
                {
                    Title = Properties.Resources.AddSupplierText,
                    ImgPath = "pack://application:,,,/Resources/Images/plus.png",
                    Command = new DelegateCommand(this.AddStoragePlace)
                };

                var deleteButton = new RibbonButton
                {
                    Title = Properties.Resources.DeleteSupplierText,
                    ImgPath = "pack://application:,,,/Resources/Images/fa-trash.png",
                    Command = new DelegateCommand(this.DeleteStoragePlace)
                };
                group.Title = Properties.Resources.StoragePlaceText;
                group.Buttons = new List<RibbonButton> { sparebutton, deleteButton };
                tab.Title = Properties.Resources.MainText;
                    
                tab.Groups = new List<RibbonGroup> { group };
               
                return new List<RibbonTab> { tab };
            }
        }

        private void DeleteStoragePlace(object obj)
        {
            if (this.SelectedStoragePlace != null)
            {
                this.StoragePlacesProvider.Remove(this.SelectedStoragePlace.StoragePlace);
                this.storagePlaces.Remove(this.SelectedStoragePlace);
            }
        }

        [ImportingConstructor]
        public StoragePlacesViewModel([Import("StoragePlaceProvider")] IDataProvider<Base.Models.StoragePlace> StoragePlacesProvider)
        {
            this.StoragePlacesProvider = StoragePlacesProvider;
            this.SelectedStoragePlace = new StoragePlaceViewModel(new Base.Models.StoragePlace(),this.StoragePlacesProvider);
            this.storagePlaces = new BindableCollection<StoragePlaceViewModel>();
            this.StoragePlaces = CollectionViewSource.GetDefaultView(this.storagePlaces);
        }

        public StoragePlaceViewModel SelectedStoragePlace { get; set; }

        public ICollectionView StoragePlaces
        {
            get; set;
        }

        private void AddStoragePlace(object obj)
        {
            var sup = new Base.Models.StoragePlace();
            this.StoragePlacesProvider.Add(sup);
            this.storagePlaces.Add(new StoragePlaceViewModel(sup, this.StoragePlacesProvider));
        }

        private void Update()
        {
            var oldInv = this.storagePlaces.Select(i => i.Id);
            var providerInv = this.StoragePlacesProvider.GetAll().Select(inv => inv.StoragePlacePK);

            foreach (var item in providerInv.Except(oldInv))
            {
                this.storagePlaces.Add(new StoragePlaceViewModel(this.StoragePlacesProvider.GetById(item), this.StoragePlacesProvider));
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            this.Update();
            this.StoragePlaces?.Refresh();
        }
    }
}
