namespace FreightAlliance.Common.Helpers
{
    using Telerik.Windows.Controls;

    public class RibbonButton
    {
        public string Title { get; set; }

        public string ImgPath { get; set; }

        public DelegateCommand Command { get; set; }
    }
}
