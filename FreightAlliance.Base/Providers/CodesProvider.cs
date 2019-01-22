using System.Data.Entity.Migrations.Model;

namespace FreightAlliance.Base.Providers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using FreightAlliance.Base.Models;
    using FreightAlliance.Common.Extensions;
    using FreightAlliance.Common.Interfaces;

    [Export(nameof(CodesProvider), typeof(IDataProvider<Code>))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class CodesProvider : IDataProvider<Code>
    {
        private IEnumerable<Code> codes;

        public DataProvider DataProvider { get; private set; }

        [ImportingConstructor]
        public CodesProvider([Import("DataProvider")] IBaseProvider provider)
        {
            this.DataProvider = (DataProvider) provider;
            Update();
        }

        private void Update()
        {
            this.codes = this.DataProvider.Codes.ToList();
        }

        public IEnumerable<Code> GetAll()
        {
            return this.codes;
        }

        public IEnumerable<Code> Get(int count)
        {
            throw new System.NotImplementedException();
        }

        public Code GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Code GetByGuid(Guid id)
        {
            throw new NotImplementedException();
        }

        public Code GetById(int id)
        {
            throw new System.NotImplementedException();
        }

        public void Add(Code item)
        {
            throw new System.NotImplementedException();
        }

        public void AddRange(IEnumerable<Code> items)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(Code item)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveById(int id)
        {
            throw new System.NotImplementedException();
        }

        public void SaveChanges(Code item)
        {
            throw new System.NotImplementedException();
        }


        public IEnumerable<Code> GetAll(string vessel)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<Code> GetByOrderId(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
