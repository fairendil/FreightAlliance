namespace FreightAlliance.HomeService.Hubs
{
    using System.Threading.Tasks;

    using NLog;

    public partial class FreightAllianceHub
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public override Task OnConnected()
        {
            DataAccess.isConnected = true;

            Logger.Info($"Client {this.Context.ConnectionId} connected!");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            DataAccess.isConnected = false;

            Logger.Info($"Client {this.Context.ConnectionId} disconnected!");
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            DataAccess.isConnected = true;

            Logger.Info($"Client {this.Context.ConnectionId} reconnected!");
            return base.OnReconnected();
        }
    }
}
