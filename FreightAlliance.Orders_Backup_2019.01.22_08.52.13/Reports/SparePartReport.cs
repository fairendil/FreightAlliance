namespace FreightAlliance.Orders.Reports
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;
    using FreightAlliance.Orders.ViewModels;
    /// <summary>
    /// Summary description for SparePartReport.
    /// </summary>
    public partial class SparePartReport : Telerik.Reporting.Report
    {

        public SparePartReport(SparePartsOrderViewModel context)
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.dataSource1.DataSource = context;
            this.dataSource2.DataSource = context;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}