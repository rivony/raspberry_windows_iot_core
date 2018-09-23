using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace PremiersEssaisGpio
{
  public class BoolToCouleurConverter : IValueConverter
  {
    public Brush CouleurTrue { get; set; }
    public Brush CouleurFalse { get; set; }
    public Brush CouleurNull { get; set; }
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      if (value is bool)
        return (bool)value ? CouleurTrue : CouleurFalse;
      else
        return CouleurNull;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}
