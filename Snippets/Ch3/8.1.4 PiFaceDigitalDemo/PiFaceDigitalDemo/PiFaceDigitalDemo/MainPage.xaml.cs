using PiFaceLIB;
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

namespace PiFaceDigitalDemo
{
  /// <summary>
  /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
  /// </summary>
  public sealed partial class MainPage : Page
  {   
    private PiFaceDigital _piFaceDigital = null;
    private ChenillardPi _chenillard = null;
    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
      _piFaceDigital = new PiFaceDigital(0, 0);
      if (await _piFaceDigital.Initialiser(GpioController.GetDefault()))
      {
        _chenillard = new ChenillardPi(_piFaceDigital);
        PiFaceDigitalCCT.DataContext = new PiFaceDigitalVM(_piFaceDigital);
      }
    }
    private void ChenillardTSW_Toggled(object sender, RoutedEventArgs e)
    {
      if (ChenillardTSW.IsOn)
        _chenillard.Demarrer(100);
      else
        _chenillard.Arrêter();
    }

    public MainPage()
    {
      this.InitializeComponent();
    }
  }
}
