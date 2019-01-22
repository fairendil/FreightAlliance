using FreightAlliance.Base.Models;
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
    public partial class OrderReportCopy : Telerik.Reporting.Report
    {
        public OrderReportCopy()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

        }

        public OrderReportCopy(SupplyOrderViewModel order)
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = order;
        }

        public OrderReportCopy(SparePartsOrder order)
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            this.DataSource = order;
        }
    }
}