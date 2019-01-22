using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Data.Entity.Migrations;
using System.Runtime.CompilerServices;
using FreightAlliance.Base.Helpers;

namespace FreightAlliance.Base.Providers
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using FreightAlliance.Base.Models;
    using FreightAlliance.Common.Interfaces;

    [Export(nameof(OrdersProvider), typeof(IDataProvider<Order>))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class OrdersProvider : IDataProvider<Order>
    {

        [ImportingConstructor]
        public OrdersProvider(
            [Import("DataProvider")] IBaseProvider provider)
        {
            this.dataProvider = (DataProvider) provider;
            
            this.loadedOrders = new List<Order>();
            Update();
        }

        private void Update()
        {
            this.dataProvider.Configuration.ProxyCreationEnabled = false;
            this.dataProvider.Configuration.LazyLoadingEnabled = false;
            this.dataProvider.Configuration.ProxyCreationEnabled = false;
            this.oldOrders = new List<Order>();
            this.oldOrders.AddRange(this.dataProvider.SparePartsOrder.ToList());
            this.oldOrders.AddRange(this.dataProvider.SupplyOrder.ToList());
            foreach (var order in oldOrders)
            {
                this.dataProvider.Entry(order).Reload();
                order.Number = this.dataProvider.Numbers.FirstOrDefault(n => n.OrderGuid == order.OrderGuid);
                order.Invoice = this.dataProvider.Invoices.FirstOrDefault(n => n.OrderGuid == order.OrderGuid);
                order.Supplier = this.dataProvider.Suppliers.FirstOrDefault(n => n.OrderGuid == order.OrderGuid);
                if (order is SupplyOrder)
                {

                }
                else
                {

                }
            }
            this.loadedOrders = this.oldOrders;
        }


        private List<Order> loadedOrders;

        private DataProvider dataProvider;
        private List<Order> oldOrders;
        

        public IEnumerable<Order> GetAll()
        {
            this.Update();
            return this.loadedOrders;
        }

        public IEnumerable<Order> GetAll(Vessel vessel)
        {
            this.Update();
            return this.loadedOrders.Where(o => o.Vessel.Equals(vessel));
        }

        public IEnumerable<Order> Get(int count)
        {
            if (this.loadedOrders.Count <= count)
            {
                return this.loadedOrders;
            }

            return this.loadedOrders.OrderByDescending(order => order.CreationDate).Take(count);
        }

        public Order GetById(int id)
        {
            return this.loadedOrders.FirstOrDefault(order => order.OrderId == id);
        }

        public Order GetByGuid(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Add(Order item)
        {
            if (item.OrderId == 0)
            {

                if (this.loadedOrders.Any(o => o.Vessel.Equals(item.Vessel)))
                {
                    var max = this.loadedOrders.Where(o => o.Vessel.Equals(item.Vessel))
                            .Max(order => order.Number.No) + 1;
                    item.Number.No = max;
                    
                }
                else
                {
                    item.Number.No = 1;
                   
                }
                item.Number.Year = item.CreationDate;                
            }
            this.loadedOrders.Add(item);
            if (item is SparePartsOrder)
            {
                this.dataProvider.SparePartsOrder.AddOrUpdate(item as SparePartsOrder);
            }
            else
            {
                this.dataProvider.SupplyOrder.AddOrUpdate(item as SupplyOrder);
            }


            this.dataProvider.SaveChanges();
            item.Number.OrderGuid = item.OrderGuid;
            this.dataProvider.Numbers.AddOrUpdate(item.Number);
            this.dataProvider.SaveChanges();
        }

        public void AddRange(IEnumerable<Order> items)
        {
            this.loadedOrders.AddRange(items);
        }

        public void Remove(Order item)
        {
            this.loadedOrders.Remove(item);
        }

        public void RemoveById(int id)
        {
            this.loadedOrders.Remove(this.loadedOrders.FirstOrDefault(order => order.OrderId == id));
        }

        public void SaveChanges(Order item)
        {
            if (item.OrderPositions != null)
            {
                if (item.OrderPositions.FirstOrDefault() is SparePartsOrderPosition)
                {
                    foreach (SparePartsOrderPosition pos in item.OrderPositions)
                    {
                        this.dataProvider.SparePartsOrderPosition.AddOrUpdate(pos);
                    }

                }
                else if (item.OrderPositions.FirstOrDefault() is SupplyOrderPosition)
                {
                    foreach (SupplyOrderPosition pos in item.OrderPositions)
                    {
                        this.dataProvider.SupplyOrderPosition.AddOrUpdate(pos);
                    }
                }
            }
            if (item is SparePartsOrder)
            {
                this.dataProvider.SparePartsOrder.AddOrUpdate(item as SparePartsOrder);
            }
            else
            {
                this.dataProvider.SupplyOrder.AddOrUpdate(item as SupplyOrder);
            }
            this.dataProvider.SaveChanges();            
            this.Update();

        }

        public IEnumerable<Order> GetAll(string vessel)
        {
            return this.loadedOrders.Where(order => order.Vessel == vessel);
        }

        public IEnumerable<Order> GetByOrderId(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
