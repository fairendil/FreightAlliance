using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

namespace FreightAlliance.Base.Providers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Common.Extensions;

    using FreightAlliance.Base.Models;
    using FreightAlliance.Common.Interfaces;

    [Export(nameof(OrderPositionsProvider), typeof(IOrderPostionsProvider<OrderPosition>))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class OrderPositionsProvider : IOrderPostionsProvider<OrderPosition>
    {
        private List<OrderPosition> loadedOrderPositions;

        private object @lock = new object();
        private DataProvider dataProvider;
        [ImportingConstructor]
        public OrderPositionsProvider([Import("DataProvider")]IBaseProvider provider)
        {
            this.dataProvider = (DataProvider)provider;
            List<OrderPosition> oldPositions = new List<OrderPosition>();
            oldPositions.AddRange(this.dataProvider.SupplyOrderPosition.ToList());
            oldPositions.AddRange(this.dataProvider.SparePartsOrderPosition.ToList());
            this.loadedOrderPositions = oldPositions.ToList();
        }

        private void Update()
        {
            this.dataProvider.Configuration.ProxyCreationEnabled = false;
            this.dataProvider.Configuration.LazyLoadingEnabled = false;
            this.dataProvider.Configuration.ProxyCreationEnabled = false;
            this.dataProvider.Configuration.AutoDetectChangesEnabled = true;
            

            var oldOrders = new List<OrderPosition>();
            var objectContext = ((IObjectContextAdapter)this.dataProvider).ObjectContext;
            objectContext.Refresh(RefreshMode.StoreWins,dataProvider.SparePartsOrderPosition );
            oldOrders.AddRange(this.dataProvider.SparePartsOrderPosition.ToList());
            oldOrders.AddRange(this.dataProvider.SupplyOrderPosition.ToList());
            
            this.loadedOrderPositions = oldOrders;
        }
        public IEnumerable<OrderPosition> GetAll()
        {
            Update();
            return this.loadedOrderPositions;
        }

        public IEnumerable<OrderPosition> Get(int count)
        {
            Update();
            return this.loadedOrderPositions.OrderByDescending(o => o.OrderPositionId).Take(count).ToList();
        }

        public OrderPosition GetById(int id)
        {
            throw new NotImplementedException();
        }

        public OrderPosition GetByGuid(Guid id)
        {
            Update();
            return this.loadedOrderPositions.FirstOrDefault(order => order.OrderPositionGuid == id);
        }

        public IEnumerable<OrderPosition> GetByOrderId(Guid id)
        {
            Update();
            return this.loadedOrderPositions.Where(order => order.OrderGuid == id);
        }

        public void Add(OrderPosition item)
        {
            this.loadedOrderPositions.Add(item);
        }

        public void AddRange(IEnumerable<OrderPosition> orderPositions)
        {
            foreach (var orderPosition in orderPositions)
            {
                this.loadedOrderPositions.Add(orderPosition);
            }
        }

        public void Remove(OrderPosition item)
        {
            if (item is SparePartsOrderPosition)
            {

                    this.dataProvider.SparePartsOrderPosition.Remove((SparePartsOrderPosition) item);
               
            }
            else if (item is SupplyOrderPosition)
            {

                    this.dataProvider.SupplyOrderPosition.Remove((SupplyOrderPosition) item);

            }

            this.loadedOrderPositions.Remove(item);
        }

        public void RemoveById(int id)
        {

        }

        public void SaveChanges(OrderPosition item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OrderPosition> GetAll(string vessel)
        {
            throw new NotImplementedException();
        }
    }
}
