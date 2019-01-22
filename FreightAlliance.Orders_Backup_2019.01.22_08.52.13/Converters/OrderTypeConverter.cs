namespace FreightAlliance.Orders.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Markup;

    using FreightAlliance.Orders.ViewModels;

    public class OrderTypeConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SparePartsOrderViewModel)
            {
                return value as SparePartsOrderViewModel;
            }
            if (value is SupplyOrderViewModel)
            {
                return value as SupplyOrderViewModel;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

    }
}
