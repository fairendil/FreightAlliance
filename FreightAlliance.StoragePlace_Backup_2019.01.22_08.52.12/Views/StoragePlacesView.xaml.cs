using FreightAlliance.StoragePlaces.ViewModels;
using Telerik.Windows.Controls.Data.DataForm;

namespace FreightAlliance.StoragePlaces.Views
{
    using System.Windows;
    using System.Windows.Data;

    using FreightAlliance.StoragePlaces.ViewModels;

    public partial class StoragePlacesView
    {
        public StoragePlacesView()
        {
            this.InitializeComponent();
        }


        private void RadDataForm1_EditEnded(object sender, EditEndedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            var supplierViewModel = frameworkElement?.DataContext as StoragePlaceViewModel;
            supplierViewModel?.Save();
        }
    }
}