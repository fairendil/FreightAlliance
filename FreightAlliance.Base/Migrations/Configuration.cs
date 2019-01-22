using System.Data.Entity.Infrastructure;

namespace FreightAlliance.Base.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FreightAlliance.Base.Providers.DataProvider>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "FreightAlliance.Base.Providers.DataProvider";
            
            
        }


        protected override void Seed(FreightAlliance.Base.Providers.DataProvider context)
        {
            

        }
    }
}
