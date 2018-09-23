using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Core;

namespace GlobalLIB
{
  public class GpioPinVM : VMBase
  {
    private GpioPin _modele = null;
    private async void _modele_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
    {
      SortieEtatHaut = (args.Edge == GpioPinEdge.RisingEdge);
      await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
      {
        OnPropertyChanged("SortieEtatHaut");
      });
    }

    public bool SortieEtatHaut { get; private set; }
    public GpioPinVM(GpioPin modele) : base()
    {
      _modele = modele;
      _modele.ValueChanged += _modele_ValueChanged;
      SortieEtatHaut = (_modele.Read() == GpioPinValue.High);
    }
  }
}
