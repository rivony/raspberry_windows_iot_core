using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace DoMesCenter.Convertisseurs
{
  public class PartieEntiereConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      if (value is double)
      {
        double val = (double)value;
        if (!double.IsNaN(val))
        {
          string valStr = val.ToString("F1");
          return valStr.Substring(0, valStr.Length - 2);
        }
      }
      else if (value is uint)
      {
        uint val = (uint)value;
        if (val != uint.MaxValue)
          return val.ToString();
      }
      else if (value is byte)
        return ((byte)value).ToString();
      else if (value is ulong)
      {
        ulong val = (ulong)value;
        if (val != ulong.MaxValue)
          return val.ToString();
      }
      if (parameter is int)
      {
        int nbTirets = (int)parameter;
        return new string('-', nbTirets);
      }
      return "-";
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}
