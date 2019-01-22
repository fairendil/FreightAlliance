namespace FreightAlliance.Common.Interfaces
{
    using Caliburn.Micro;

    public interface INavigationItem
    {
        BindableCollection<INavigationItem> RelatedItems { get; set; }

        string Title { get; set; }

        string ItemName { get; set; }
    }
}