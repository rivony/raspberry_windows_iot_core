using CoursesListLIB;
using SerialLIB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.SerialCommunication;
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

namespace CoursesList
{
  /// <summary>
  /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
  /// </summary>
  public sealed partial class MainPage : Page
  {
    private CoreDispatcher _dispatcher = Window.Current.Dispatcher;
    private async void Lecteur_SurCodeBarresRecu(object sender, string codeBarres)
    {
      await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
      {
        ListeProduits.instance.AjouterCodeBarres(codeBarres);
      });
    }
    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
      ParametresPortSerie parametresLecteur = new ParametresPortSerie("SCAN_ROUGE", @"\\?\FTDIBUS#VID_0403+PID_6001+COM5A#", 9600, SerialParity.None, SerialStopBitCount.One, 8, SerialHandshake.None, 100, 0);
      LecteurCodeBarres lecteur = new LecteurCodeBarres(parametresLecteur, true, 32);
      LecteurCodeBarresAPB.DataContext = new PeripheriqueSerieVM(lecteur);
      lecteur.SurCodeBarresRecu += Lecteur_SurCodeBarresRecu;

      ListeProduitsLVW.DataContext = new ListeProduitsVM(ListeProduits.instance);
      FiltreeAPB.DataContext = ListeProduitsLVW.DataContext;

      await ListeProduits.instance.Charger();

      PeripheriqueSerie.PeripheriquesSerie.DemarrerSurveillance();
    }
    private void ListeProduitsLVW_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ListeProduitsLVW.ScrollIntoView(e.AddedItems.FirstOrDefault());
    }
    private void ViderListeAPB_Click(object sender, RoutedEventArgs e)
    {
      ListeProduits.instance.Vider();
    }

    public MainPage()
    {
      this.InitializeComponent();
    }
  }
}
