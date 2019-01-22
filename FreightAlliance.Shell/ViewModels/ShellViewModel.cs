using System.Threading;
using System.Threading.Tasks;

namespace FreightAlliance.Shell.ViewModels
{
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;

    using Caliburn.Micro;

    using FreightAlliance.Common.Interfaces;
    using FreightAlliance.Shell.Interfaces;

    [Export(typeof(IShell))]
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IShell, IHandle<INavigationItem>
    {
        private readonly CompositionContainer container;

        [ImportingConstructor]
        public ShellViewModel(IEventAggregator eventAggregator, CompositionContainer container)
        {
            this.container = container;
            eventAggregator.Subscribe(this);
        }

        public ShellViewModel()
        {
        }

        [Import]
        public NavigationViewModel Navigation { get; set; }

        [Import]
        public RibbonViewModel Ribbon { get; set; }

        public void Handle(INavigationItem navigationItem)
        {
            var screen = this.container.GetExports<IScreen>(navigationItem.ItemName).FirstOrDefault();
            if (screen != null)
            {
                this.ActivateItem(screen.Value);
            }
        }

        public Task HandleAsync(INavigationItem message, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}