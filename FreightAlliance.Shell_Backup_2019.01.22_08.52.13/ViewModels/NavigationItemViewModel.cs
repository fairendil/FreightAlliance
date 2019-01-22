namespace FreightAlliance.Shell.ViewModels
{
    using Caliburn.Micro;

    using FreightAlliance.Common.Interfaces;

    public class NavigationItemViewModel : INavigationItem
    {
        public BindableCollection<INavigationItem> RelatedItems { get; set; }

        public string Title { get; set; }

        public string ItemName { get; set; }

        public void NavigationViewModel()
        {
            this.RelatedItems = new BindableCollection<INavigationItem>();
        }
    }
}