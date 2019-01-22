using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using FreightAlliance.Base.Models;
using FreightAlliance.Common.Enums;

namespace FreightAlliance.Orders.Converters
{
    public class DateToColorConveter
         : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var Status = (StatusEnum)value;
            var greenYellow = Colors.GreenYellow;
            switch (Status)
            {
                case StatusEnum.New:
                    return new SolidColorBrush(Colors.Aqua);
                case StatusEnum.Confirmed:
                    return new SolidColorBrush(Colors.Aquamarine);
                case StatusEnum.ReadyToBeSent:
                    return new SolidColorBrush(Colors.BlueViolet);
                case StatusEnum.SentToTheOffice:
                    return new SolidColorBrush(Colors.Red);
                case StatusEnum.ReceivedAtOffice:
                    return new SolidColorBrush(Colors.OrangeRed);
                case StatusEnum.SentForQuotation:
                    return new SolidColorBrush(Colors.Coral);
                case StatusEnum.Received:
                    return new SolidColorBrush(Colors.Orange);
                case StatusEnum.Capitalized:
                    return new SolidColorBrush(Colors.White);
                case StatusEnum.Closed:
                    return new SolidColorBrush(Colors.White);
                case StatusEnum.Canceled:
                    return new SolidColorBrush(Colors.Gray);
                default:
                    return new SolidColorBrush(Colors.Gray);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}