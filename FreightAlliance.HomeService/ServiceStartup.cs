using Microsoft.Owin;

[assembly: OwinStartup(typeof(FreightAlliance.HomeService.ServiceStartup))]

namespace FreightAlliance.HomeService
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
