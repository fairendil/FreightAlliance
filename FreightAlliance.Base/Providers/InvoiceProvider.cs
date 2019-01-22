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

    [Export(nameof(InvoiceProvider), typeof(IDataProvider<Invoice>))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class InvoiceProvider : IDataProvider<Invoice>
    {
        [ImportingConstructor]
        public InvoiceProvider([Import("DataProvider")] IBaseProvider provider)
        {
            this.dataProvider = (DataProvider)provider;
            Update();
        }

        private void Update()
        {
            this.oldOrders = new List<Invoice>();
            this.oldOrders.AddRange(this.dataProvider.Invoices.ToList());
            this.loadedInvoices = new List<Invoice>(this.oldOrders.ToList());
        }


        private List<Invoice> loadedInvoices;

        private DataProvider dataProvider;
        private List<Invoice> oldOrders;

        public IEnumerable<Invoice> GetAll()
        {
            return this.loadedInvoices;
        }

        public IEnumerable<Invoice> Get(int count)
        {
            if (this.loadedInvoices.Count <= count)
            {
                return this.loadedInvoices;
            }

            return this.loadedInvoices.OrderByDescending(i => i.Date).Take(count);
        }

        public Invoice GetById(int id)
        {
            return this.loadedInvoices.FirstOrDefault(i => i.InvoiceId == id);
        }

        public Invoice GetByGuid(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Add(Invoice item)
        {
            
            this.loadedInvoices.Add(item);

            this.dataProvider.Invoices.AddOrUpdate(item);

            this.dataProvider.SaveChanges();
        }

        public void AddOrUdate(Invoice item)
        {
            var oldInvoice = this.loadedInvoices.FirstOrDefault(i => i.InvoiceId == item.InvoiceId);
            if (oldInvoice != null)
            {
                this.loadedInvoices.Remove(oldInvoice);
                this.loadedInvoices.Add(item);
            }
            else
            {
                this.loadedInvoices.Add(item);
            }

            this.dataProvider.Invoices.AddOrUpdate(item);
            this.dataProvider.SaveChanges();
        }


        public void AddRange(IEnumerable<Invoice> items)
        {
            this.loadedInvoices.AddRange(items);
            this.dataProvider.Invoices.AddRange(items);
            this.dataProvider.SaveChangesAsync();
        }

        public void Remove(Invoice item)
        {
            this.loadedInvoices.Remove(item);
            this.dataProvider.Invoices.Remove(item);
            this.dataProvider.SaveChangesAsync();
        }

        public void RemoveById(int id)
        {
            this.loadedInvoices.Remove(this.loadedInvoices.FirstOrDefault(order => order.InvoiceId == id));
        }

        public void SaveChanges(Invoice item)
        {
            var oldInvoice = this.loadedInvoices.FirstOrDefault(i => i.InvoiceId == item.InvoiceId);
            if (oldInvoice != null)
            {
                this.loadedInvoices.Remove(oldInvoice);
                this.loadedInvoices.Add(item);
            }
            else
            {
                this.loadedInvoices.Add(item);
            }

            this.dataProvider.Invoices.AddOrUpdate(item);
            this.dataProvider.SaveChanges();
        }

        public IEnumerable<Invoice> GetAll(string vessel)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Invoice> GetByOrderId(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}

