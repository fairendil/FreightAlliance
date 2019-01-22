using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreightAlliance.Base.Providers
{

    using FreightAlliance.Base.Models;
    using Common.Interfaces;
    using System.ComponentModel.Composition;
    using System.Data.Entity.Migrations;

    [Export(nameof(StoragePlaceProvider), typeof(IDataProvider<StoragePlace>))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class StoragePlaceProvider : IDataProvider<StoragePlace>
    {
        [ImportingConstructor]
        public StoragePlaceProvider([Import("DataProvider")] IBaseProvider provider)
        {
            this.dataProvider = (DataProvider)provider;
            Update();
        }

        private void Update()
        {
            this.oldOrders = new List<StoragePlace>();
            this.oldOrders.AddRange(this.dataProvider.StoragePlaces.ToList());
            this.loadedStoragePlaces = new List<StoragePlace>(this.oldOrders.ToList());
        }


        private List<StoragePlace> loadedStoragePlaces;

        private DataProvider dataProvider;
        private List<StoragePlace> oldOrders;

        public IEnumerable<StoragePlace> GetAll()
        {
            return this.loadedStoragePlaces;
        }

        public IEnumerable<StoragePlace> Get(int count)
        {
            if (this.loadedStoragePlaces.Count <= count)
            {
                return this.loadedStoragePlaces;
            }

            return this.loadedStoragePlaces.Take(count);
        }

        public StoragePlace GetById(int id)
        {
            return this.loadedStoragePlaces.FirstOrDefault(i => i.StoragePlacePK == id);
        }

        public StoragePlace GetByGuid(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Add(StoragePlace item)
        {
            
            this.loadedStoragePlaces.Add(item);

            this.dataProvider.StoragePlaces.AddOrUpdate(item);

            this.dataProvider.SaveChanges();
        }

        public void AddOrUdate(StoragePlace item)
        {
            var oldStoragePlace = this.loadedStoragePlaces.FirstOrDefault(i => i.StoragePlacePK == item.StoragePlacePK);
            if (oldStoragePlace != null)
            {
                this.loadedStoragePlaces.Remove(oldStoragePlace);
                this.loadedStoragePlaces.Add(item);
            }
            else
            {
                this.loadedStoragePlaces.Add(item);
            }

            this.dataProvider.StoragePlaces.AddOrUpdate(item);
            this.dataProvider.SaveChanges();
        }


        public void AddRange(IEnumerable<StoragePlace> items)
        {
            this.loadedStoragePlaces.AddRange(items);
            this.dataProvider.StoragePlaces.AddRange(items);
            this.dataProvider.SaveChangesAsync();
        }

        public void Remove(StoragePlace item)
        {
            this.loadedStoragePlaces.Remove(item);
            this.dataProvider.StoragePlaces.Remove(item);
            this.dataProvider.SaveChangesAsync();
        }

        public void RemoveById(int id)
        {
            this.loadedStoragePlaces.Remove(this.loadedStoragePlaces.FirstOrDefault(order => order.StoragePlacePK == id));
        }

        public void SaveChanges(StoragePlace item)
        {
            var oldStoragePlace = this.loadedStoragePlaces.FirstOrDefault(i => i.StoragePlacePK == item.StoragePlacePK);
            if (oldStoragePlace != null)
            {
                this.loadedStoragePlaces.Remove(oldStoragePlace);
                this.loadedStoragePlaces.Add(item);
            }
            else
            {
                this.loadedStoragePlaces.Add(item);
            }

            this.dataProvider.StoragePlaces.AddOrUpdate(item);
            this.dataProvider.SaveChanges();
        }

        public IEnumerable<StoragePlace> GetAll(string vessel)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<StoragePlace> GetByOrderId(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}

