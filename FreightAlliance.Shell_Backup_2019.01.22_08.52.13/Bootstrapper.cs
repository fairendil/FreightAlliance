using System.Globalization;
using System.Threading;

namespace FreightAlliance.Shell
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows;

    using Caliburn.Micro;

    using Common.Interfaces;

    using FreightAlliance.Shell.Interfaces;
    using FreightAlliance.Shell.ViewModels;

    using Microsoft.AspNet.SignalR.Client;

    public class Bootstrapper : BootstrapperBase
    {
        private CompositionContainer container;

        private HubConnection connection;

        private IHubProxy hubProxy;

        public Bootstrapper()
        {
            this.Initialize();
        }

        protected override void Configure()
        {
            this.container =
                new CompositionContainer(
                    new AggregateCatalog(
                        AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>()));


            var batch = new CompositionBatch();

            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            
            batch.AddExportedValue(this.container);

            this.container.Compose(batch);
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = this.container.GetExportedValues<object>(contract).ToList();

            if (exports.Any())
            {
                return exports.First();
            }

            throw new Exception(string.Format("Could not locate any instances of contract {0}.",contract));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            var exportedValues = this.container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
            return exportedValues;
        }

        protected override void BuildUp(object instance)
        {
            this.container.SatisfyImportsOnce(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            this.DisplayRootViewFor<IShell>();
     
            var windowManager = IoC.Get<IWindowManager>();
            var loginViewModel = IoC.Get<ILogin>() as LoginViewModel;
            windowManager.ShowDialog(loginViewModel);
            if (!loginViewModel.Confirmed)
            {
                this.Application.MainWindow.Close();
            }
            var batch = new CompositionBatch();
            batch.AddExportedValue<IUser>("User", loginViewModel.SelectedUser);
            

            this.container.Compose(batch);

            foreach (var supportInitialize in IoC.GetAll<ISupportInitialize>())
            {
                if (supportInitialize != null)
                {
                    supportInitialize.BeginInit();
                 }
            }            
        }

        private void Connect()
        {            
            string serverAddress = ConfigurationManager.AppSettings["ServerAddress"];
            this.connection = new HubConnection(serverAddress);

            this.connection.Error += exception => { MessageBox.Show(exception.Message); };
            this.connection.Closed += this.ConnectionClosed;
            
            this.hubProxy = this.connection.CreateHubProxy("FreightAllianceHub");

            this.hubProxy.On<int>("TestClient", this.TestClient);
            this.connection.StateChanged += this.Connection;            
            this.connection.Start();
            
        }

        private void Error(Exception obj)
        {
            throw new NotImplementedException();
        }

        private void Connection(StateChange obj)
        {
           if (obj.NewState == ConnectionState.Connected)
            {
                this.TestServer();
            }
        }

        private void TestClient(int i)
        {
            MessageBox.Show(i.ToString());
        }

        public void TestServer()
        {
            
            this.hubProxy.Invoke("TestServer", new Base.Models.SparePartsOrder() { Comment = "fuck eha"} );
        }

        private void ConnectionClosed()
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            var assemblies = base.SelectAssemblies();
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            var files = directory.GetFiles("*.dll", SearchOption.TopDirectoryOnly);

            var modules =
                files.Where(f => f.Name.Contains("FreightAlliance") && !f.Name.Contains("FreightAlliance.Common"))
                    .Select(f => Assembly.LoadFile(f.FullName));
            return assemblies.Concat(modules);
        }
    }
}