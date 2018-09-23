using GlobalLIB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace SerialLIB
{
  public class GpsNMEAVM : VMBase
  {
    private GpsNMEA _modele = null;
    private async void _modele_SurPhraseGGA(object sender, DonneesGGA donnees)
    {
      GGA = donnees;
      await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
      {
        OnPropertyChanged("GGA");
      });
    }
    private async void _modele_SurPhraseRMC(object sender, DonneesRMC donnees)
    {
      RMC = donnees;
      await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
      {
        OnPropertyChanged("RMC");
      });
    }

    public DonneesGGA GGA { get; private set; }
    public DonneesRMC RMC { get; private set; }
    public GpsNMEAVM(GpsNMEA modele)
    {
      _modele = modele;
      _modele.SurPhraseGGA += _modele_SurPhraseGGA;
      _modele.SurPhraseRMC += _modele_SurPhraseRMC;
    }
  }
}
