namespace FreightAlliance.HomeService
{
    using System;
    using System.ServiceProcess;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNet.SignalR.Client;
    using Microsoft.Owin.Hosting;

    using NLog;
    using System.Linq;
    using System.Collections.Generic;
    using Base.Models;
    public partial class HomeService : ServiceBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private HubConnection connection;
        private IHubProxy hubProxy;
        private Task saveTask;

        public static void Main(string[] args)
        {
            try
            {
                var service = new HomeService();
                if (System.Environment.UserInteractive)
                {
                    service.OnStart(args);
                    System.Console.WriteLine("Press any key to stop program");
                    System.Console.Read();
                    service.OnStop();
                }
                else
                {
                    ServiceBase.Run(service);
                    Logger.Info("end service?");
                }
            }
            catch (Exception ex)
            {

                Logger.Info(ex.Message + "||" + ex.InnerException.Message);
            }
            
            Logger.Info("end program");

        }
        public HomeService()
        {
            this.InitializeComponent();
            this.ServiceName = "TestService";
        }


        protected override void OnStart(string[] args)
        {
            Logger.Info("Service started.");
            try
            {
                DataAccess.connection.Start();
            }
            catch (Exception ex)
            {
                Logger.Info(ex.Message + "||"+ ex.InnerException.Message);
            }
        }

        //private void Connect()
        //{
        //    string serverAddress = FreightAlliance.Service.Properties.Settings.Default.ServiceConnection;

        //    Logger.Info($"Connection to {serverAddress} started.");

        //    this.connection = new HubConnection(serverAddress);

        //    this.connection.Error += this.Error;
        //    this.connection.Closed += this.ConnectionClosed;

        //    this.hubProxy = this.connection.CreateHubProxy("FreightAllianceHub");

        //    this.hubProxy.On<int>("TestClient", this.TestClient);
        //    this.connection.StateChanged += this.Connection;
        //    this.connection.Start();
        //}

        //private void TestClient(int obj)
        //{
        //    throw new NotImplementedException();
        //}

        //private void ConnectionClosed()
        //{
        //    throw new NotImplementedException();
        //}

        //private void Error(Exception ex)
        //{
        //    Logger.Log(LogLevel.Error, ex);
        //    this.Connect();
        //}

        //private void Connection(StateChange state)
        //{
        //    if (state.NewState == ConnectionState.Connected)
        //    {
        //        Logger.Info("Connected to service.");

        //        this.TestServer();
        //    }
        //    else if (state.NewState == ConnectionState.Disconnected)
        //    {
        //        Logger.Info("Disconnected from service.");

        //        Thread.Sleep(new TimeSpan(0, 0, 5));
        //        this.Connect();
        //    }
        //}

        //private void TestServer()
        //{
        //    Logger.Info("TestServer called.");

        //    List<Order> newOrders = DataAccess.DataProvider.SparePartsOrder.Where(o => o.Status == Common.Enums.StatusEnum.SentToTheOffice).Select(o => o as Order).ToList();
        //    newOrders.AddRange(DataAccess.DataProvider.SupplyOrder.Where(o => o.Status == Common.Enums.StatusEnum.SentToTheOffice).Select(o => o as Order).ToList());
        //    if (!newOrders.Any()) return;

        //    foreach (var order in newOrders)
        //    {
        //        try
        //        {
        //            this.hubProxy.Invoke("TestServer", order);
        //            order.Status = Common.Enums.StatusEnum.ReceivedAtOffice;
        //            order.ReceivedAtOfficeDate = DateTime.Now;
        //        }
        //        catch
        //        {
        //            if (this.connection.State != ConnectionState.Connected) break;
        //        }
        //    }
        //    DataAccess.DataProvider.SaveChanges();
        //}          

        protected override void OnStop()
        {
            Logger.Info("Service sopping.");
        }
    }
}
