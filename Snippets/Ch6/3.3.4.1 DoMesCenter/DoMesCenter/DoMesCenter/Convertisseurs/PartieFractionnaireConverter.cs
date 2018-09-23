using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace DoMesCenter.Convertisseurs
{
  public class PartieFractionnaireConverter : IValueConverter
  {
    private static NumberFormatInfo _nfi = new CultureInfo("fr-FR").NumberFormat;
    static PartieFractionnaireConverter()
    {
      _nfi.NumberDecimalSeparator = ".";
    }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
      if (value is double)
      {
        double val = (double)value;
        if (!double.IsNaN(val))
        {
          string valStr = val.ToString("F1", _nfi);
          return valStr.Substring(valStr.Length - 2);
        }
      }
      return ".-";
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}
