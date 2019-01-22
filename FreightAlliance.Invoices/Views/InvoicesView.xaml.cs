using Telerik.Windows.Controls.Data.DataForm;

namespace FreightAlliance.Invoices.Views
{
    using System.Windows;
    using System.Windows.Data;

    using FreightAlliance.Invoices.ViewModels;

    using Telerik.Windows.Controls;

    using CommonResources = Common.Properties.Resources;
    using System.Windows.Input;

    public partial class InvoicesView
    {
        public InvoicesView()
        {
            this.InitializeComponent();
        }

        private void RadDataForm_AutoGeneratingField(object sender, Telerik.Windows.Controls.Data.DataForm.AutoGeneratingFieldEventArgs e)
        {

        }

        private void RadDataForm1_EditEnded(object sender, EditEndedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            var invoiceViewModel = frameworkElement?.DataContext as InvoiceViewModel;
            if (invoiceViewModel != null)
            {
                invoiceViewModel.Save();
            }
        }

        private void ExternalPresenter_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}