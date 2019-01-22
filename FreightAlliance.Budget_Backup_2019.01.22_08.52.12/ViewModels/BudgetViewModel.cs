using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using Caliburn.Micro;
using FreightAlliance.Base;
using FreightAlliance.Base.Models;
using FreightAlliance.Base.Providers;
using FreightAlliance.Common.Annotations;
using FreightAlliance.Common.Attributes;
using FreightAlliance.Common.Helpers;
using FreightAlliance.Common.Interfaces;
using Telerik.Windows.Controls;

namespace FreightAlliance.Budget.ViewModels
{
    [Export("Budget", typeof(IScreen))]
    [Export(typeof(IScreen))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class BudgetViewModel : Screen , INavigationScreen, ICanRibbon, INotifyPropertyChanged
    {
        private DataProvider dataProvider;
        private User user;
        private BindableCollection<BudgetPositionViewModel> budgets;
        private List<CodeType> codeTypes;
        private DateTime budgetYear;

        public string NavigationTitle => Properties.Resources.BudgetText;

        public string NavigationItemName => "Budget";

        public string NavigationGroup
        {
            get
            {
                var title = this.user.Role == RoleEnum.Manager ? Properties.Resources.MainText : string.Empty;
                return title;
            }
        }

        public List<RibbonTab> Tabs
        {
            get
            {

                var tab = new RibbonTab();
                var group = new RibbonGroup();
                return new List<RibbonTab> { tab };
            }
        }

        public ICollectionView Budgets { get; private set; }

        [ImportingConstructor]
        public BudgetViewModel([Import("DataProvider")] IBaseProvider dataProvider,
                                 [Import("User")] IUser user)
        {
            this.dataProvider = (DataProvider)dataProvider;
            this.user = (User)user;
            this.budgetYear = DateTime.Today;
            this.budgets = new BindableCollection<BudgetPositionViewModel>();
            this.Budgets = CollectionViewSource.GetDefaultView(this.budgets);
            this.codeTypes = this.dataProvider.CodeTypes.ToList();
            foreach (var code in this.dataProvider.Codes)
            {
                this.budgets.Add(new BudgetPositionViewModel(code,this.codeTypes.FirstOrDefault(ct => ct.Type == code.CodeTypeId)));
            }
            Refresh();
        }

        private void Refresh()
        {
            var orders = this.dataProvider.SparePartsOrder.Where(c => c.CreationDate.Year == this.BudgetYear.Year).ToList();
            
            foreach (var order in orders)
            {
                var budget = this.budgets.FirstOrDefault(c => order.Code != null && c.code.Number == order.Code.Number);

                var positions = this.dataProvider.SparePartsOrderPosition.Where(p => p.OrderGuid == order.OrderGuid);
                foreach (var position in positions)
                {
                    budget.Plan += position.Quantity*position.Price;
                }
                var inv = this.dataProvider.Invoices.FirstOrDefault(i => i.OrderGuid == order.OrderGuid);
                if (inv != null)
                {
                    budget.Fact += inv.PaidAmount;
                }
            }
            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(Budgets)));
        }


        public DateTime BudgetYear
        {
            get { return this.budgetYear; }
            set
            {
                this.Refresh();
                this.budgetYear = value;
            }
        }
    }
}