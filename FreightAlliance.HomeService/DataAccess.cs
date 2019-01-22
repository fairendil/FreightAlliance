

namespace FreightAlliance.HomeService
{
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Diagnostics;
    using System.Linq;
    using FreightAlliance.Base.Models;
    using Microsoft.AspNet.SignalR.Client;
    using NLog;
    using FreightAlliance.Base.Providers;


    static class DataAccess
    {
        public static HubConnection connection;
        public static IHubProxy hubProxy;
        public static bool isConnected;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static CancellationTokenSource tokenSource = new CancellationTokenSource();
        private static CancellationToken cancellationToken = tokenSource.Token;
        private static Object thisLock = new Object();

        static DataAccess()
        {
            DataAccess.DataProvider = new DataProvider("Alliance");
            DataAccess.InDataProvider = new DataProvider("Alliance");

            initialize();
        }

        public static DataProvider InDataProvider { get; set; }

        private static void initialize()
        {
            string serverAddress = FreightAlliance.HomeService.Properties.Settings.Default.ServiceConnection;
            connection = new HubConnection(serverAddress);
            hubProxy = connection.CreateHubProxy("FreightAllianceHub");
            hubProxy.On<SparePartsOrder>("SendToVesselSparePartsOrder", SendToVesselSparePartsOrder);
            hubProxy.On<SupplyOrder>("SendToVesselSupplyOrder", SendToVesselSupplyOrder);
            hubProxy.On<List<SparePartsOrderPosition>>("CheckRows", CheckRows);
            connection.Error += ConnectionOnError;
            connection.StateChanged += ConnectionOnStateChanged;
        }

        private static void ConnectionOnStateChanged(StateChange stateChange)
        {
            if (stateChange.NewState == ConnectionState.Disconnected)
            {
                tokenSource.Cancel();
                Logger.Info("Disconect");
                Thread.Sleep(5000);
                connection.Dispose();
                initialize();
                connection.Start();
            }

            if (stateChange.NewState == ConnectionState.Connected)
            {
                tokenSource = new CancellationTokenSource();
                cancellationToken = tokenSource.Token;
                hubProxy.Invoke("StartSend");
                Logger.Info("start sending to vessel");
                Task.Factory.StartNew(async () =>
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            while (await SendRows())
                            {
                                if (cancellationToken.IsCancellationRequested)
                                {
                                    break;
                                }

                                
                                Thread.Sleep(5000);
                            }
                            Logger.Info("Thread ended");
                        }, cancellationToken);
            }
        }

        private static void ConnectionOnError(Exception exception)
        {
            tokenSource.Cancel();
            Logger.Info(exception.Message);
            Thread.Sleep(5000);
            connection.Dispose();
            initialize();
            connection.Start();
        }


        private static async void SendToVesselSparePartsOrder(SparePartsOrder order)
        {
            Logger.Info("SparePartsOrder recived");
            lock (thisLock)
            {
                var old = DataAccess.InDataProvider.SparePartsOrder.FirstOrDefault(o => o.OrderGuid == order.OrderGuid);

                old.Status = order.Status;
                old.ReceivedAtVesselDate = order.ReceivedAtVesselDate;
                InDataProvider.SaveChanges();
            }
        }

        private static async void SendToVesselSupplyOrder(SupplyOrder order)
        {
            Logger.Info("SupplyOrder recived");
            lock (thisLock)
            {
                var old = DataAccess.InDataProvider.SupplyOrder.FirstOrDefault(o => o.OrderGuid == order.OrderGuid);
                old.Status = order.Status;
                old.ReceivedAtVesselDate = order.ReceivedAtVesselDate;
                InDataProvider.SaveChanges();
            }
        }

        private static async void CheckRows(List<SparePartsOrderPosition> positions)
        {
            InDataProvider.Configuration.ProxyCreationEnabled = false;
            InDataProvider.Configuration.LazyLoadingEnabled = false;
            InDataProvider.Configuration.ProxyCreationEnabled = false;
            var guidList = positions.Select(p => p.ShipPositionGuid);
            var updateList = await 
                InDataProvider.SparePartsOrderPosition.Where(p => p.Received && guidList.Contains(p.OrderPositionGuid))
                    .ToListAsync();
            foreach (var position in updateList)
            {
                var sparePartsOrderPosition = positions.FirstOrDefault(p => p.ShipPositionGuid == position.OrderPositionGuid);
                sparePartsOrderPosition.Received = true;
                sparePartsOrderPosition.StoragePlace = position.StoragePlace;
                await hubProxy.Invoke("SendSparePartsOrderPositionToServer", sparePartsOrderPosition);
            }
            Logger.Info("CheckRows ended");
        }

        public static DataProvider DataProvider { get; set; }

        public static async Task<bool> SendRows()
        {
            DataProvider.Configuration.ProxyCreationEnabled = false;
            DataProvider.Configuration.LazyLoadingEnabled = false;
            DataProvider.Configuration.ProxyCreationEnabled = false;
            if (connection.State != ConnectionState.Connected) return false;
            List<Order> newOrders =
                new List<Order>();
            lock (thisLock)
            {
                try
                {
                    var collection =
                        DataAccess.DataProvider.SparePartsOrder.Where(
                            o => o.Status == Common.Enums.StatusEnum.SentToTheOffice)
                            .Include(o => o.Code)
                            .Include(o => o.Number)
                            .ToList();
                    newOrders =
                        new List<Order>(collection);
                    newOrders.AddRange(
                        DataAccess.DataProvider.SupplyOrder.Where(
                            o => o.Status == Common.Enums.StatusEnum.SentToTheOffice)
                            .Include(o => o.Code)
                            .Include(o => o.Number)
                            .ToList());
                }
                catch (InvalidOperationException ex)
                {
                    return false;
                }
                catch (Exception ex)
                {
                    Logger.Info(ex.Message + "||" + ex.InnerException.Message);

                    Logger.Info(newOrders + "1");
                }
            }
            if (!newOrders.Any()) return true;
            try
            {
                foreach (var order in newOrders)
                {
                    try
                    {
                        if (connection.State == ConnectionState.Connected)
                        {
                            order.Status = Common.Enums.StatusEnum.ReceivedAtOffice;
                            order.ReceivedAtOfficeDate = DateTime.Now;
                            Logger.Info("Sending to vessel " + order.Number.ToString());
                            if (order is SupplyOrder)
                            {
                                Logger.Info("SupplyOrder " + order.Number.ToString());
                                var i =
                                    await
                                        hubProxy.Invoke<int>("SendSupplyOrderToServer", order, order.Code,
                                            order.Number);

                                foreach (
                                    var position in
                                        DataProvider.SupplyOrderPosition.Where(o => o.OrderGuid == order.OrderGuid))
                                {
                                    while (
                                        await hubProxy.Invoke<int>("SendSupplyOrderPositionToServer", position) == 0)
                                    {
                                        Thread.Sleep(100);
                                    }
                                }
                                DataAccess.DataProvider.SaveChanges();
                            }
                            else
                            {
                                Logger.Info("SparePartsOrder " + order.Number.ToString());
                                var i =
                                    await
                                        hubProxy.Invoke<int>("SendSparePartsOrderToServer", order, order.Code,
                                            order.Number);
                                if (i == 0)
                                {
                                    Logger.Info("end send row failed");
                                }
                                else
                                {


                                    foreach (
                                        var position in
                                            DataProvider.SparePartsOrderPosition.Where(
                                                o => o.OrderGuid == order.OrderGuid))
                                    {
                                        while (
                                            await
                                                hubProxy.Invoke<int>("SendSparePartsOrderPositionToServer", position) ==
                                            0)
                                        {
                                            Thread.Sleep(100);
                                        }

                                    }
                                    lock (thisLock)
                                    {
                                        DataAccess.DataProvider.SaveChanges();
                                    }
                                }
                            }
                            Logger.Info("end send row");
                        }
                    }
                    catch
                    {
                        Logger.Info("Disconected " + order.Number.ToString());
                        if (connection.State != ConnectionState.Connected) break;
                    }
                    Logger.Info("end try");
                }
            }
            catch (Exception ex)
            {

                Logger.Info(ex.Message + "||" + ex.InnerException.Message);
                Logger.Info(newOrders + "2");
            }

            Logger.Info("end send rows");
            return true;
        }

    }
}
