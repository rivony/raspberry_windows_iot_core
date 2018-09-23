using GpsCamLIB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GpsCam
{
  /// <summary>
  /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
  /// </summary>
  public sealed partial class MainPage : Page
  {
    private AppareilPhoto _appareilPhoto = null;
    private GpioPin _poussoirDecl = null;
    private GpioPin _ledEnMarche = null;
    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
      GpioController gpc = GpioController.GetDefault();
      GpioPin ledOccupe = gpc?.OpenPin(24);

      _appareilPhoto = new AppareilPhoto(ledOccupe);
      if (await _appareilPhoto.Initialiser(1280, 960, "MJPG"))
      {
        PrendrePhotoBTN.Visibility = Visibility.Visible;
        if (gpc != null)
        {
          _poussoirDecl = gpc.OpenPin(18);
          _poussoirDecl.SetDriveMode(GpioPinDriveMode.InputPullUp);
          _poussoirDecl.DebounceTimeout = new TimeSpan(0, 0, 0, 0, 5);

          _ledEnMarche = gpc.OpenPin(23);
          _ledEnMarche.SetDriveMode(GpioPinDriveMode.Output);
          _ledEnMarche.Write(GpioPinValue.High);
        }
      }
    }

    public MainPage()
    {
      this.InitializeComponent();
    }


  }
}
