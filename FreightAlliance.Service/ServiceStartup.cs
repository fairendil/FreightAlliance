using Microsoft.Owin;

[assembly: OwinStartup(typeof(FreightAlliance.Service.ServiceStartup))]

namespace FreightAlliance.Service
{
    using Microsoft.Owin.Cors;

    using Owin;

    public class ServiceStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }

    }
}
