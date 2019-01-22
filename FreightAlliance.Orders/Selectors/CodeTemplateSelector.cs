using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FreightAlliance.Orders.Selectors
{
    public class CodeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var t = item as string;
            return base.SelectTemplate(item, container);
        }
    }
}
