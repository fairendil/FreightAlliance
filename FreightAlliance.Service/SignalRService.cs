using System;
using System.Linq;
using FreightAlliance.Base.Models;
using FreightAlliance.Base.Providers;
using NLog;

namespace FreightAlliance.Service
{
    using System.ServiceProcess;
    using Microsoft.Owin.Hosting;

    public partial class ServerService : ServiceBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static void Main(string[] args)
        {
            try
            {
                var service = new ServerService();
                if (System.Environment.UserInteractive)
                {
                    service.OnStart(args);
                    DataAccess.isConnected = true;
                    System.Console.WriteLine("Press any key to stop program");
                    System.Console.Read();
                    service.OnStop();
                }
                else
                {
                    ServiceBase.Run(service);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }
        public ServerService()
        {
            this.InitializeComponent();
            this.ServiceName = "TestService";            
        }


        protected override void OnStart(string[] args)
        {
            
            string url = FreightAlliance.Service.Properties.Settings.Default.ServiceConnection;
            WebApp.Start(url);

        }

        protected override void OnStop()
        {
        }
    }
}
