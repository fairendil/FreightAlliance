using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FreightAlliance.Service.Hubs
{
    using System.Threading.Tasks;

    using NLog;

    public partial class FreightAllianceHub
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static CancellationTokenSource tokenSource = new CancellationTokenSource();
        private static CancellationToken cancellationToken = tokenSource.Token;
        public override Task OnConnected()
        {
            Logger.Info($"Client {this.Context.ConnectionId} connected!");
            
            tokenSource = new CancellationTokenSource();
            cancellationToken = tokenSource.Token;
            if (!ClientsList.Any())
            {
                ClientsList.Add(this.Context.ConnectionId);
                Logger.Info($"Client first!");
                First = true;
            }
            else
            {
                return Task;
            }
            
            return base.OnConnected();
        }

        

        public static List<string> ClientsList = new List<string>();

        public override Task OnDisconnected(bool stopCalled)
        {
            DataAccess.isConnected = false;
            
            tokenSource.Cancel();
            Logger.Info($"Client {this.Context.ConnectionId} disconnected!");
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            DataAccess.isConnected = true;
            
            this.SendRows();
            Logger.Info($"Client {this.Context.ConnectionId} reconnected!");
            return base.OnReconnected();
        }
    }
}
