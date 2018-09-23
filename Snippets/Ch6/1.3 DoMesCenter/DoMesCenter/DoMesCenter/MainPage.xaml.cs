using DoMesCenterLIB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace DoMesCenter
{
  /// <summary>
  /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
  /// </summary>
  public sealed partial class MainPage : Page
  {
    private Horloge _horloge = null;
    private void InitHorloge()
    {
      _horloge = new Horloge();
      DateHeureTBK.DataContext = new HorlogeVM(_horloge);
    }
    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
      InitHorloge();
    }

    public MainPage()
    {
      this.InitializeComponent();
    }   
  }
}
