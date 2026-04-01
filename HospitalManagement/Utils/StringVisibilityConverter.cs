using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace HospitalManagement.Utils
{
    public class StringVisibilityConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string selected = value as string;
            string target = parameter as string;

            if (string.IsNullOrEmpty(selected) || string.IsNullOrEmpty(target))
                return Visibility.Collapsed;

            return target.Contains(selected) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
