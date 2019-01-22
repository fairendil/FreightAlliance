using System;

namespace FreightAlliance.Common.Interfaces
{
    using System.Collections.Generic;

    public interface IOrderPostionsProvider<T> : IDataProvider<T>
    {
        IEnumerable<T> GetByOrderId(Guid id);
    }
}
