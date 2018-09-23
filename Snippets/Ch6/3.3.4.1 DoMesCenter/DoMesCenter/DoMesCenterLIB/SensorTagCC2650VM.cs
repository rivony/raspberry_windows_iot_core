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
    private async void _modele_SurHDC1000_NouvelleValeur(HDC1000Mesure nouvelleValeur)
    {
      if (nouvelleValeur != null)
      {
        HDC1000_Temperature = nouvelleValeur.Temperature;
        HDC1000_Humidite = nouvelleValeur.Humidite;
        await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        {
          OnPropertyChanged("HDC1000_Temperature");
          OnPropertyChanged("HDC1000_Humidite");
        });
      }
      HDC1000_DerniereLectureReussie = (nouvelleValeur != null);
      await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
      {
        OnPropertyChanged("HDC1000_DerniereLectureReussie");
      });
    }
    private async void _modele_SurBMP280_NouvelleValeur(BMP280Mesure nouvelleValeur)
    {
      if (nouvelleValeur != null)
      {
        BMP280_Temperature = nouvelleValeur.Temperature;
        BMP280_Pression = nouvelleValeur.Pression;
        await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        {
          OnPropertyChanged("BMP280_Temperature");
          OnPropertyChanged("BMP280_Pression");
        });
      }
      BMP280_DerniereLectureReussie = (nouvelleValeur != null);
      await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
      {
        OnPropertyChanged("BMP280_DerniereLectureReussie");
      });
    }
    private async void _modele_SurOPT3001_NouvelleValeur(OPT3001Mesure nouvelleValeur)
    {
      if (nouvelleValeur != null)
      {
        OPT3001_Luminosite = nouvelleValeur.Luminosite;       
        await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        {
          OnPropertyChanged("OPT3001_Luminosite");
        });
      }
      OPT3001_DerniereLectureReussie = (nouvelleValeur != null);
      await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
      {
        OnPropertyChanged("OPT3001_DerniereLectureReussie");
      });
    }

    public double TMP007_TemperatureAmbiante   { get; private set; }
    public double TMP007_TemperatureIR         { get; private set; }
    public bool TMP007_DerniereLectureReussie  { get; private set; }
    public double HDC1000_Temperature          { get; private set; }
    public double HDC1000_Humidite             { get; private set; }
    public bool HDC1000_DerniereLectureReussie { get; private set; }
    public double BMP280_Temperature           { get; private set; }
    public double BMP280_Pression              { get; private set; }
    public bool BMP280_DerniereLectureReussie  { get; private set; }
    public double OPT3001_Luminosite           { get; private set; }
    public bool OPT3001_DerniereLectureReussie { get; private set; }

    public SensorTagCC2650VM(SensorTagCC2650 modele)
    {
      _modele = modele;
      TMP007_TemperatureAmbiante = double.NaN;
      TMP007_TemperatureIR = double.NaN;
      TMP007_DerniereLectureReussie = false;
      _modele.SurTMP007_NouvelleValeur += _modele_SurTMP007_NouvelleValeur;

      HDC1000_Temperature = double.NaN;
      HDC1000_Humidite = double.NaN;
      HDC1000_DerniereLectureReussie = false;
      _modele.SurHDC1000_NouvelleValeur += _modele_SurHDC1000_NouvelleValeur;

      BMP280_Temperature = double.NaN;
      BMP280_Pression = double.NaN;
      BMP280_DerniereLectureReussie = false;
      _modele.SurBMP280_NouvelleValeur += _modele_SurBMP280_NouvelleValeur;

      OPT3001_Luminosite = double.NaN;
      OPT3001_DerniereLectureReussie = false;
      _modele.SurOPT3001_NouvelleValeur += _modele_SurOPT3001_NouvelleValeur;
    }
  }
}
