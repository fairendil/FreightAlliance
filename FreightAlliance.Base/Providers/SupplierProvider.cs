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

    [Export(nameof(SupplierProvider), typeof(IDataProvider<Supplier>))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SupplierProvider : IDataProvider<Supplier>
    {
        [ImportingConstructor]
        public SupplierProvider([Import("DataProvider")] IBaseProvider provider)
        {
            this.dataProvider = (DataProvider)provider;
            Update();
        }

        private void Update()
        {
            this.oldOrders = new List<Supplier>();
            this.oldOrders.AddRange(this.dataProvider.Suppliers.ToList());
            this.loadedSuppliers = new List<Supplier>(this.oldOrders.ToList());
        }


        private List<Supplier> loadedSuppliers;

        private DataProvider dataProvider;
        private List<Supplier> oldOrders;

        public IEnumerable<Supplier> GetAll()
        {
            return this.loadedSuppliers;
        }

        public IEnumerable<Supplier> Get(int count)
        {
            if (this.loadedSuppliers.Count <= count)
            {
                return this.loadedSuppliers;
            }

            return this.loadedSuppliers.OrderByDescending(i => i.Name).Take(count);
        }

        public Supplier GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Supplier GetByGuid(Guid id)
        {
            throw new NotImplementedException();
        }

        public Supplier GetById(int id)
        {
            return this.loadedSuppliers.FirstOrDefault(i => i.SupplierId == id);
        }

        public void Add(Supplier item)
        {
            
            this.loadedSuppliers.Add(item);

            this.dataProvider.Suppliers.AddOrUpdate(item);

            this.dataProvider.SaveChanges();
        }

        public void AddOrUdate(Supplier item)
        {
            var oldSupplier = this.loadedSuppliers.FirstOrDefault(i => i.SupplierId == item.SupplierId);
            if (oldSupplier != null)
            {
                this.loadedSuppliers.Remove(oldSupplier);
                this.loadedSuppliers.Add(item);
            }
            else
            {
                this.loadedSuppliers.Add(item);
            }

            this.dataProvider.Suppliers.AddOrUpdate(item);
            this.dataProvider.SaveChanges();
        }


        public void AddRange(IEnumerable<Supplier> items)
        {
            this.loadedSuppliers.AddRange(items);
            this.dataProvider.Suppliers.AddRange(items);
            this.dataProvider.SaveChangesAsync();
        }

        public void Remove(Supplier item)
        {
            this.loadedSuppliers.Remove(item);
            this.dataProvider.Suppliers.Remove(item);
            this.dataProvider.SaveChangesAsync();
        }

        public void RemoveById(int id)
        {
            this.loadedSuppliers.Remove(this.loadedSuppliers.FirstOrDefault(order => order.SupplierId == id));
        }

        public void SaveChanges(Supplier item)
        {
            var oldSupplier = this.loadedSuppliers.FirstOrDefault(i => i.SupplierId == item.SupplierId);
            if (oldSupplier != null)
            {
                this.loadedSuppliers.Remove(oldSupplier);
                this.loadedSuppliers.Add(item);
            }
            else
            {
                this.loadedSuppliers.Add(item);
            }

            this.dataProvider.Suppliers.AddOrUpdate(item);
            this.dataProvider.SaveChanges();
        }

        public IEnumerable<Supplier> GetAll(string vessel)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Supplier> GetByOrderId(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}

