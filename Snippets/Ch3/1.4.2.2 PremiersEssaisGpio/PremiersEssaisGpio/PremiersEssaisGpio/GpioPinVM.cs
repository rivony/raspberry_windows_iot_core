using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace PremiersEssaisGpio
{
  public class GpioPinVM : INotifyPropertyChanged
  {
    private static CoreDispatcher _dispatcher = null;
    private GpioPin _model = null;
    private void OnPropertyChanged(string propertyName = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    private async void _model_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
    {
      SortieEtatHaut = (args.Edge == GpioPinEdge.RisingEdge);
      await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
      {
        OnPropertyChanged("SortieEtatHaut");
      });
    }

    public bool SortieEtatHaut { get; private set; }
    public GpioPinVM(GpioPin model)
    {
      if (_dispatcher == null)
        _dispatcher = Window.Current.Dispatcher;
      _model = model;
      _model.ValueChanged += _model_ValueChanged;
    }
    public event PropertyChangedEventHandler PropertyChanged;
  }
}
