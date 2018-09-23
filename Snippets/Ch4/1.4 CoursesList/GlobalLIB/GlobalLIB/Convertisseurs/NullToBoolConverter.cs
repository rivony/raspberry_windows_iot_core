using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace GlobalLIB.Convertisseurs
{
  public class NullToBoolConverter : IValueConverter
  {
    public bool Invert { get; set; }
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      if (Invert)
        return (value == null);
      else
        return (value != null);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}
