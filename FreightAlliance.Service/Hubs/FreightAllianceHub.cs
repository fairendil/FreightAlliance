using System.Data.Entity;
using System.Threading;
using FreightAlliance.Base.Models;
using FreightAlliance.Common.Enums;

namespace FreightAlliance.Service.Hubs
{
    using System.Diagnostics;

    using Microsoft.AspNet.SignalR;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNet.SignalR.Client;
    using System;
    using System.Data.Entity.Migrations;
    public partial class FreightAllianceHub : Hub
    {
        private HubConnection connection;
        private IHubProxy hubProxy;
        private static Object thisLock = new Object();

        public Task Task { get; private set; }

        public async Task<int> SendSparePartsOrderToServer(SparePartsOrder order, Code code, Number num )
        {
            Logger.Info($"recive order {order.OrderId} | {code} | {num}");
            lock (thisLock)
            {
                if (DataAccess.InDataProvider.Database.Connection.State != System.Data.ConnectionState.Open)
                {
                    DataAccess.InDataProvider.Database.Connection.Open();
                }
                if (code != null)
                {
                    order.Code = DataAccess.InDataProvider.Codes.FirstOrDefault(c => c.CodeId == code.CodeId);
                }
                else
                {
                    order.Code = DataAccess.InDataProvider.Codes.FirstOrDefault();
                }
                if (num != null)
                {
                    order.Number = num;
                }
                try
                {
                    DataAccess.InDataProvider.SparePartsOrder.Add(order);
                    var i = DataAccess.InDataProvider.SaveChanges();
                    return i;
                }
                catch (Exception ex)
                {
                    Logger.Info(ex.Message + "||" + ex.InnerException.Message);
                    tokenSource.Cancel();
                }
            }
            return 0;
        }

        public async Task<int> SendSparePartsOrderPositionToServer(SparePartsOrderPosition position)
        {
            lock (thisLock)
            {
                try
                {
                    if (DataAccess.InDataProvider.Database.Connection.State != System.Data.ConnectionState.Open)
                    {
                        DataAccess.InDataProvider.Database.Connection.Open();
                    }
                    Logger.Info($"position {position.SparePartsOrderPositionId}");
                    if (
                        !DataAccess.InDataProvider.SparePartsOrderPosition.Any(
                            p => p.OrderPositionGuid == position.OrderPositionGuid))
                    {
                        if (position.ShipPositionGuid == Guid.Empty)
                        {
                            position.ShipPositionGuid = position.OrderPositionGuid;
                        }
                        DataAccess.InDataProvider.SparePartsOrderPosition.Add(position);
                        return DataAccess.InDataProvider.SaveChanges();
                    }
                    else
                    {
                        var updatePosition =
                            DataAccess.InDataProvider.SparePartsOrderPosition.FirstOrDefault(
                                p => p.OrderPositionGuid == position.OrderPositionGuid);
                        updatePosition.Received = position.Received;
                        updatePosition.StoragePlace = position.StoragePlace;
                        return DataAccess.InDataProvider.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Info(ex.Message + "||" + ex.InnerException.Message);
                    tokenSource.Cancel();
                }
            }
            return 0;
        }

        public async Task<int> SendSupplyOrderPositionToServer(SupplyOrderPosition position)
        {
            lock (thisLock)
            {
                try
                {
                    if (DataAccess.InDataProvider.Database.Connection.State != System.Data.ConnectionState.Open)
                    {
                         DataAccess.InDataProvider.Database.Connection.Open();
                    }
                    Logger.Info($"position {position.SupplyOrderPositionId}");
                    if (
                        !DataAccess.InDataProvider.SupplyOrderPosition.Any(
                            p => p.OrderPositionGuid == position.OrderPositionGuid))
                    {
                        if (position.ShipPositionGuid == Guid.Empty)
                        {
                            position.ShipPositionGuid = position.OrderPositionGuid;
                        }
                        DataAccess.InDataProvider.SupplyOrderPosition.Add(position);
                        return DataAccess.InDataProvider.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Info(ex.Message + "||" + ex.InnerException.Message);
                    tokenSource.Cancel();
                }
            }
            return 0;
        }

        public void StartSend()
        {
            try
            {
                Logger.Info("Task start");
                    Task.Factory.StartNew(() =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        while (true)
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                break;
                            }
                            
                            this.SendRows();
                            Thread.Sleep(2500);
                            this.UpdatePositions();
                            Thread.Sleep(2500);
                        }
                    }
                        , cancellationToken);
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            Logger.Info($"Sucsess {First}");
        }

        public bool First { get; set; }

        public async Task<int> SendSupplyOrderToServer(SupplyOrder order, Code code, Number num)
        {
            lock (thisLock)
            {
                try
                {
                    if (DataAccess.InDataProvider.Database.Connection.State != System.Data.ConnectionState.Open)
                    {
                        DataAccess.InDataProvider.Database.Connection.Open();
                    }
                    Logger.Info($"recive order {order.OrderId} | {code} | {num}");
                    if (code != null)
                    {
                        order.Code = code;
                    }
                    else
                    {
                        order.Code = DataAccess.InDataProvider.Codes.FirstOrDefault();
                    }
                    if (num != null)
                    {
                        order.Number = num;
                    }

                    DataAccess.InDataProvider.SupplyOrder.AddOrUpdate(order);
                    return DataAccess.InDataProvider.SaveChanges();
                }
                catch (Exception ex)
                {
                    Logger.Info(ex.Message + "||" + ex.InnerException.Message);
                    tokenSource.Cancel();
                }
            }
            return 0;
        }

        public async void SendSupplyOrderPositionsToServer(List<SparePartsOrderPosition> positions)
        {
            lock (thisLock)
            {
                if (DataAccess.InDataProvider.Database.Connection.State != System.Data.ConnectionState.Open)
                {
                    DataAccess.InDataProvider.Database.Connection.Open();
                }
                foreach (var position in positions)
                {
                    var current =
                        DataAccess.InDataProvider.SparePartsOrderPosition.FirstOrDefault(
                            p => p.OrderPositionGuid == position.OrderPositionGuid);
                    current.Received = position.Received;
                    current.StoragePlace = position.StoragePlace;
                    DataAccess.InDataProvider.SparePartsOrderPosition.AddOrUpdate(current);
                }
                DataAccess.InDataProvider.SaveChanges();
            }
        }

        
        public void SendRows()
        {
            lock (thisLock)
            {
                try
                {
                    if (DataAccess.DataProvider.Database.Connection.State != System.Data.ConnectionState.Open)
                    {
                        DataAccess.DataProvider.Database.Connection.OpenAsync();
                    }
                    Logger.Info("try find rows to send");
                    List<Order> newOrders =
                        new List<Order>(
                            DataAccess.DataProvider.SparePartsOrder.Where(
                                o => o.Status == Common.Enums.StatusEnum.Confirmed)
                                .ToList());
                    newOrders.AddRange(
                        DataAccess.DataProvider.SupplyOrder.Where(o => o.Status == Common.Enums.StatusEnum.Confirmed)
                            .ToList());
                    if (!newOrders.Any()) return;
                    Logger.Info("try to send");
                    foreach (var order in newOrders)
                    {
                        try
                        {
                            if (DataAccess.isConnected)
                            {
                                order.Status = Common.Enums.StatusEnum.Received;
                                order.ReceivedAtVesselDate = DateTime.Now;
                                if (order is SupplyOrder)
                                {
                                    SendToVesselSupplyOrder((SupplyOrder) order);
                                }
                                else
                                {
                                    SendToVesselSparePartsOrder((SparePartsOrder) order);
                                }

                                DataAccess.DataProvider.SaveChangesAsync();
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch
                        {
                            if (connection.State != ConnectionState.Connected)
                            {
                                Logger.Info("not connected");
                                tokenSource.Cancel();
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Info(ex.Message + "||" + ex.InnerException.Message);
                    tokenSource.Cancel();
                }
            }
        }

        public void UpdatePositions()
        {
            lock (thisLock)
            {
                var newOrdersList =
                    DataAccess.DataProvider.SparePartsOrder.Where(o => o.Status >= StatusEnum.Received)
                        .Select(c => c.OrderGuid);
                var sparePartsOrderPositions =
                    DataAccess.DataProvider.SparePartsOrderPosition.Where(
                        p => !p.Received && newOrdersList.Contains(p.OrderGuid))
                        .ToList();
                if (sparePartsOrderPositions != null && sparePartsOrderPositions.Any())
                {
                    CheckRows(sparePartsOrderPositions);
                }
            }
        }

        private static void CheckRows(List<SparePartsOrderPosition> sparePartsOrderPositions)
        {
            
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<FreightAllianceHub>();
            hubContext.Clients.All.CheckRows(sparePartsOrderPositions);
        }

        public static void SendToVesselSupplyOrder(SupplyOrder order)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<FreightAllianceHub>();
            hubContext.Clients.All.SendToVesselSupplyOrder(order);
        }
        public static void SendToVesselSparePartsOrder(SparePartsOrder order)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<FreightAllianceHub>();
            hubContext.Clients.All.SendToVesselSparePartsOrder(order);
        }

        private T UnProxy<T>(DbContext context, T proxyObject) where T : class
        {
            var proxyCreationEnabled = context.Configuration.ProxyCreationEnabled;
            try
            {
                context.Configuration.ProxyCreationEnabled = false;
                T poco = context.Entry(proxyObject).CurrentValues.ToObject() as T;
                return poco;
            }
            finally
            {
                context.Configuration.ProxyCreationEnabled = proxyCreationEnabled;
            }
        }
    }
}
