namespace FreightAlliance.Common.Interfaces
{
    using Caliburn.Micro;
    using System.ComponentModel.Composition;

    [InheritedExport]
    public interface INavigationScreen : IScreen
    {
        string NavigationTitle { get; }

        string NavigationItemName { get; }
        string NavigationGroup { get; }
    }
}