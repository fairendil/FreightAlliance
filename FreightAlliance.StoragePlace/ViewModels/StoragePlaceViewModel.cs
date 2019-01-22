using System.ComponentModel.DataAnnotations;
using FreightAlliance.Base.Models;
using FreightAlliance.Common.Common;
using FreightAlliance.Common.Interfaces;
using FreightAlliance.StoragePlaces.Properties;

namespace FreightAlliance.StoragePlaces.ViewModels
{
    public class StoragePlaceViewModel : ViewModelBase
    {
        public readonly StoragePlace StoragePlace;
        private readonly IDataProvider<Base.Models.StoragePlace> StoragePlacesProvider;

        public StoragePlaceViewModel(Base.Models.StoragePlace StoragePlace, IDataProvider<Base.Models.StoragePlace> StoragePlacesProvider)
        {
            this.StoragePlace = StoragePlace;
            this.StoragePlacesProvider = StoragePlacesProvider;
        }



        [Display(AutoGenerateField = false)]
        public int Id
        {
            get
            {
                return this.StoragePlace.StoragePlacePK;
            }
        }

        [Display(Name = "NameText", Order = 0, ResourceType = typeof(Resources))]
        public string Place
        {
            get
            {
                return this.StoragePlace.Place;
            }

            set
            {
                if (Equals(this.StoragePlace.Place, value))
                {

                    return;

                }

                this.StoragePlace.Place = value;
                this.OnPropertyChanged(nameof(this.Place));
            }
        }

        public void Save()
        {
            this.StoragePlacesProvider.SaveChanges(this.StoragePlace);
        }
    }
}
