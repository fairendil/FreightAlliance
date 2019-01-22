// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="">
// </copyright>
// <summary>
//   Interaction logic for App.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using System.Threading;
using System.Windows.Markup;
using Telerik.Windows.Controls;

namespace FreightAlliance.Shell
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
            StyleManager.ApplicationTheme = new VisualStudio2013Theme();
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e) 
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo((string)FreightAlliance.Common.Properties.Settings.Default["Lang"]);
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo((string)FreightAlliance.Common.Properties.Settings.Default["Lang"]);
            base.OnStartup(e);
        }
        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = string.Format("An unhandled exception occurred: {0}", e.Exception.Message);
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}