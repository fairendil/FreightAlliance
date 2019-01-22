using FreightAlliance.Orders.ViewModels;

namespace FreightAlliance.Orders.Reports
{
    using Orders.ViewModels;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for OrderReport.
    /// </summary>
    public partial class OrderReport : Telerik.Reporting.Report
    {
        public OrderReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

        }

        public OrderReport(SupplyOrderViewModel order)
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = order;
        }

        public OrderReport(SparePartsOrderViewModel order)
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = order;
        }
    }
}