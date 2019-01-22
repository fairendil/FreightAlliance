using System;

namespace FreightAlliance.Common.Interfaces
{
    using System.Collections.Generic;

    public interface IDataProvider<T>
    {
        IEnumerable<T> GetAll();

        IEnumerable<T> Get(int count);

        T GetById(int id);
        T GetByGuid(Guid id);

        void Add(T item);

        void AddRange(IEnumerable<T> items);

        void Remove(T item);

        void RemoveById(int id);

        void SaveChanges(T item);

        IEnumerable<T> GetAll(string vessel);
        IEnumerable<T> GetByOrderId(Guid id);
    }
}
