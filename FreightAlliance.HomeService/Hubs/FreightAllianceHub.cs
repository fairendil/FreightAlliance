using FreightAlliance.Base.Models;

namespace FreightAlliance.HomeService.Hubs
{
    using System.Diagnostics;

    using Microsoft.AspNet.SignalR;
    using System.Threading.Tasks;
    using Microsoft.AspNet.SignalR.Client;
    using System;
    using System.Data.Entity.Migrations;
    public partial class FreightAllianceHub : Hub
    {
        private HubConnection connection;
        private IHubProxy hubProxy;

        public void GetOrder(Order order)
        {
            if (order is SparePartsOrder)
            {
                DataAccess.DataProvider.SparePartsOrder.AddOrUpdate((SparePartsOrder)order);
            }
            if (order is SupplyOrder)
            {
                DataAccess.DataProvider.SupplyOrder.AddOrUpdate((SupplyOrder)order);
            }
        }


        public void SendRows()
        {
            DataAccess.SendRows();
        }
        
        public static void TestClient(int i)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<FreightAllianceHub>();
            hubContext.Clients.All.TestClient(i);
        }
    }
}
