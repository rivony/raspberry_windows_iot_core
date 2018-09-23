using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PremiersEssaisGpio
{
  /// <summary>
  /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
  /// </summary>
  public sealed partial class MainPage : Page
  {
    private GpioPin _pin20;
    private GpioController _gpc;
    private GpioPin _pin21;  
    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
      _gpc = GpioController.GetDefault();
      _pin21 = _gpc.OpenPin(21);
      EtatLedTBK.DataContext = new GpioPinVM(_pin21);
      _pin21.SetDriveMode(GpioPinDriveMode.Output);

      _pin20 = _gpc.OpenPin(20);
      _pin20.SetDriveMode(GpioPinDriveMode.InputPullUp);
      _pin20.DebounceTimeout = new TimeSpan(10000);
      _pin20.ValueChanged += _pin20_ValueChanged;
    }  
    private void OnBTN_Click(object sender, RoutedEventArgs e)
    {
      CommanderLed(true);
    }
    private void OffBTN_Click(object sender, RoutedEventArgs e)
    {
      CommanderLed(false);
    }
    private void _pin20_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
    {
      if (args.Edge == GpioPinEdge.FallingEdge)
      {
        CommanderLed(_pin21.Read() == GpioPinValue.Low);
        Debug.WriteLine(DateTime.Now.Ticks.ToString());
      }
    }
    private void CommanderLed(bool Allumee)
    {
      if (Allumee)
      {
        _pin21.Write(GpioPinValue.High);
      }
      else
      {
        _pin21.Write(GpioPinValue.Low);
      }
    }

    public MainPage()
    {
      this.InitializeComponent();
    }    
  }
}
