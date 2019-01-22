using Telerik.Windows.Controls.Data.DataForm;

namespace FreightAlliance.Suppliers.Views
{
    using System.Windows;
    using System.Windows.Data;

    using FreightAlliance.Suppliers.ViewModels;

    using Telerik.Windows.Controls;

    using CommonResources = Common.Properties.Resources;

    public partial class SuppliersView
    {
        public SuppliersView()
        {
            this.InitializeComponent();
        }


        private void RadDataForm1_EditEnded(object sender, EditEndedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            var supplierViewModel = frameworkElement?.DataContext as SupplierViewModel;
            supplierViewModel?.Save();
        }
    }
}