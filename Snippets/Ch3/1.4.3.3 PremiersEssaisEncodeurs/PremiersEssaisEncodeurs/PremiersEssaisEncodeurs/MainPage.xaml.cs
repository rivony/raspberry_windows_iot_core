using EncodeursLIB;
using GlobalLIB;
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

namespace PremiersEssaisEncodeurs
{
  /// <summary>
  /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
  /// </summary>
  public sealed partial class MainPage : Page
  {
    public MainPage()
    {
      this.InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
      GpioController gpc = GpioController.GetDefault();

      GpioPin[] pins = new GpioPin[8];
      pins[0] = gpc.OpenPin(21);
      pins[1] = gpc.OpenPin(20);
      pins[2] = gpc.OpenPin(19);
      pins[3] = gpc.OpenPin(26);
      pins[4] = gpc.OpenPin(13);
      pins[5] = gpc.OpenPin(6);
      pins[6] = gpc.OpenPin(12);
      pins[7] = gpc.OpenPin(16);
      EncodeurAbsolu encodeurAbsolu = new EncodeurAbsolu(pins, true);
      EncodeurAbsoluSTP.DataContext = new EncodeurAbsoluVM(encodeurAbsolu);

      GpioPin pinA = gpc.OpenPin(23);
      GpioPin pinB = gpc.OpenPin(24);
      EncodeurIncrementalBase encodeurIncremental = new EncodeurIncremental2(pinA, pinB);
      EncodeurIncrementalSTP.DataContext = new EncodeurIncrementalVM(encodeurIncremental);
      PinABRD.DataContext = new GpioPinVM(pinA);
      PinBBRD.DataContext = new GpioPinVM(pinB);
    }
  }
}
