using GlobalLIB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace DoMesCenterLIB
{
  public class SensorTagCC2650VM : VMBase
  {
    private SensorTagCC2650 _modele = null;
    private async void _modele_SurTMP007_NouvelleValeur(TMP007Mesure nouvelleValeur)
    {
      if (nouvelleValeur != null)
      {
        TMP007_TemperatureAmbiante = nouvelleValeur.TemperatureAmbiante;
        TMP007_TemperatureIR = nouvelleValeur.TemperatureIR;
        await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        {
          OnPropertyChanged("TMP007_TemperatureAmbiante");
          OnPropertyChanged("TMP007_TemperatureIR");
        });
      }
      TMP007_DerniereLectureReussie = (nouvelleValeur != null);
      await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
      {
        OnPropertyChanged("TMP007_DerniereLectureReussie");
      });
    }

    public double TMP007_TemperatureAmbiante   { get; private set; }
    public double TMP007_TemperatureIR         { get; private set; }
    public bool TMP007_DerniereLectureReussie  { get; private set; }
    public SensorTagCC2650VM(SensorTagCC2650 modele)
    {
      _modele = modele;
      TMP007_TemperatureAmbiante = double.NaN;
      TMP007_TemperatureIR = double.NaN;
      TMP007_DerniereLectureReussie = false;
      _modele.SurTMP007_NouvelleValeur += _modele_SurTMP007_NouvelleValeur;
    }    
  }
}
