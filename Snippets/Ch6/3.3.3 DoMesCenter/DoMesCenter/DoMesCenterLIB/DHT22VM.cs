using GlobalLIB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoMesCenterLIB
{
  public class DHT22VM : VMBase
  {
    private DHT22 _modele = null;
    private void _modele_SurNouvelleMesure(object sender, DHT22Mesure nouvelleMesure)
    {
      DerniereLectureReussie = (nouvelleMesure != null);
      OnPropertyChanged("DerniereLectureReussie");
      if (nouvelleMesure != null)
      {
        Temperature = nouvelleMesure.Temperature;
        Humidite = nouvelleMesure.Humidite;
        OnPropertyChanged("Temperature");
        OnPropertyChanged("Humidite");
      }
    }

    public double Temperature           { get; private set; }
    public double Humidite              { get; private set; }
    public bool DerniereLectureReussie  { get; private set; }
    public DHT22VM(DHT22 modele)
    {
      _modele = modele;
      Temperature = double.NaN;
      Humidite = double.NaN;
      DerniereLectureReussie = false;
      _modele.SurNouvelleMesure += _modele_SurNouvelleMesure;
    }    
  }
}
