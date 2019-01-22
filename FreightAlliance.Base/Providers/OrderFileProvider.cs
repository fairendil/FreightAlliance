using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    [Export(nameof(OrderFilesProvider), typeof(IOrderPostionsProvider<OrderFilePosition>))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class OrderFilesProvider : IOrderPostionsProvider<OrderFilePosition>
    {
        private readonly List<OrderFilePosition> loadedOrderPositions;
        private DataProvider dataProvider;
        [ImportingConstructor]
        public OrderFilesProvider([Import("DataProvider")]IBaseProvider provider)
        {
            this.dataProvider = (DataProvider)provider;
            List<OrderFilePosition> oldPositions = new List<OrderFilePosition>();
            oldPositions.AddRange(this.dataProvider.OrderFilePositions.ToList());
            this.loadedOrderPositions = oldPositions.ToList();
        }
        public IEnumerable<OrderFilePosition> GetAll()
        {
            return this.loadedOrderPositions;
        }

        public IEnumerable<OrderFilePosition> Get(int count)
        {

            return this.loadedOrderPositions.OrderByDescending(o => o.OrderFilePositionId).Take(count).ToList();
        }

        public OrderFilePosition GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public OrderFilePosition GetByGuid(Guid id)
        {
            throw new NotImplementedException();
        }

        public OrderFilePosition GetById(int id)
        {
            return this.loadedOrderPositions.FirstOrDefault(order => order.OrderFilePositionId == id);
        }

        public IEnumerable<OrderFilePosition> GetByOrderId(Guid id)
        {
            return this.loadedOrderPositions.Where(order => order.OrderGuid == id);
        }

        public void Add(OrderFilePosition item)
        {
            this.loadedOrderPositions.Add(item);
        }

        public void AddRange(IEnumerable<OrderFilePosition> orderPositions)
        {
            foreach (var orderPosition in orderPositions)
            {
                this.loadedOrderPositions.Add(orderPosition);
            }
        }

        public void Remove(OrderFilePosition item)
        {


            this.dataProvider.OrderFilePositions.Remove(item);

            this.loadedOrderPositions.Remove(item);
        }

        public void RemoveById(int id)
        {

        }

        public void SaveChanges(OrderFilePosition item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OrderFilePosition> GetAll(string vessel)
        {
            throw new NotImplementedException();
        }
    }
}
