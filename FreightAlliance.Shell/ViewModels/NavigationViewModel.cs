namespace FreightAlliance.Shell.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Caliburn.Micro;

    using FreightAlliance.Common.Interfaces;


    [Export(typeof(ISupportInitialize))]
    [Export(typeof(NavigationViewModel))]
    public class NavigationViewModel : ISupportInitialize
    {
        private readonly IEventAggregator eventAggregator;

        private NavigationItemViewModel selectedNavigationItem;

        [ImportingConstructor]
        public NavigationViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.NavigationItems = new BindableCollection<INavigationItem>();
        }

        public NavigationViewModel()
        {
        }

        public BindableCollection<INavigationItem> NavigationItems { get; set; }

        public NavigationItemViewModel SelectedNavigationItem
        {
            get
            {
                return this.selectedNavigationItem;
            }

            set
            {
                if (this.selectedNavigationItem == value)
                {
                    return;
                }

                this.selectedNavigationItem = value;
                this.eventAggregator.PublishOnUIThreadAsync(this.selectedNavigationItem);
            }
        }

        public void BeginInit()
        {
            foreach (var navigationItem in IoC.GetAll<INavigationScreen>())
            {
                if (navigationItem.NavigationGroup == string.Empty) continue;
                    this.NavigationItems.Add(
                        new NavigationItemViewModel
                        {
                            Title = navigationItem.NavigationTitle,
                            ItemName = navigationItem.NavigationItemName
                        });
                
            }
            
        }

        public void EndInit()
        {
        }
    }
}