using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FreightAlliance.Base.Models;
using Microsoft.AspNet.SignalR.Client;
using NLog;

namespace FreightAlliance.Service
{
    using FreightAlliance.Base.Providers;


    static class DataAccess
    {
        public static HubConnection connection;
        public static bool isConnected;

        static DataAccess()
        {
            DataProvider = new DataProvider("Alliance");
            InDataProvider = DataProvider; //new DataProvider("Alliance");
        }

        public static DataProvider InDataProvider { get; set; }

        public static DataProvider DataProvider { get; set; }

        
    }
}
