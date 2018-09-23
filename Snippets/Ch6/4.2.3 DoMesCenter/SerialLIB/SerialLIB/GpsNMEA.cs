using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;

namespace SerialLIB
{
  public delegate void PhraseGGA(object sender, DonneesGGA donnees);
  public delegate void PhraseRMC(object sender, DonneesRMC donnees);
  public class GpsNMEA : PeripheriqueSerie
  {
    private object _donneesRecuesLOCK = new object();
    private string _donneesRecuesBuffer = "";
    private string GetPhrasesCompletes(string dernieresDonneesRecues)
    {
      string dernieresPhrasesCompletes = "";
      _donneesRecuesBuffer += dernieresDonneesRecues;
      int idxPremierDollar = _donneesRecuesBuffer.IndexOf('$');
      if (idxPremierDollar >= 0)
      {
        int idxDernierCR = _donneesRecuesBuffer.LastIndexOf('\r');
        if (idxDernierCR >= 0)
          dernieresPhrasesCompletes = _donneesRecuesBuffer.Substring(idxPremierDollar, idxDernierCR - idxPremierDollar);
        _donneesRecuesBuffer = _donneesRecuesBuffer.Substring(idxDernierCR + 2);
      }
      return dernieresPhrasesCompletes;
    }
    private string GetDernierePhrase(string idPhrase, string phrases)
    {
      int idxIdPhrase = phrases.LastIndexOf("$" + idPhrase);
      if (idxIdPhrase >= 0)
      {
        int idxCR = phrases.IndexOf("\r\n", idxIdPhrase);
        if (idxCR >= 0)
          return phrases.Substring(idxIdPhrase, idxCR - idxIdPhrase);
      }
      return "";
    }

    protected override void DonnesRecues(DataReader donnees)
    {
      string dernierePhraseGGA;
      string dernierePhraseRMC;
      lock (_donneesRecuesLOCK)
      {
        byte[] dRecues = new byte[donnees.UnconsumedBufferLength];
        donnees.ReadBytes(dRecues);
        string dernieresPhrasesCompletes = GetPhrasesCompletes(Encoding.ASCII.GetString(dRecues));
        Debug.WriteLine(dernieresPhrasesCompletes);
        dernierePhraseGGA = GetDernierePhrase("GPGGA", dernieresPhrasesCompletes);
        dernierePhraseRMC = GetDernierePhrase("GPRMC", dernieresPhrasesCompletes);
      }
      if (dernierePhraseGGA != "")
        SurPhraseGGA?.Invoke(this, new DonneesGGA(dernierePhraseGGA));
      if (dernierePhraseRMC != "")
        SurPhraseRMC?.Invoke(this, new DonneesRMC(dernierePhraseRMC));
    }

    public GpsNMEA(ParametresPortSerie parametres, bool autoOuvrir, uint nbMaxOctetsALire) : base(parametres, autoOuvrir, nbMaxOctetsALire)
    {

    }
    public event PhraseGGA SurPhraseGGA;
    public event PhraseRMC SurPhraseRMC;
  }

  public abstract class DonneesPhraseNMEA
  {
    protected static readonly NumberFormatInfo _convertisseurVersDouble = new NumberFormatInfo() { NumberDecimalSeparator = ".", NumberDecimalDigits = 4 };
    protected static string ChaineVideEstZero(string value)
    {
      return string.IsNullOrEmpty(value) ? "0" : value;
    }
    protected double GetCoordonnee(string valeur, string direction, int nbCarsDegres)
    {
      if (valeur.Length < nbCarsDegres) return 0;
      double res = Convert.ToDouble(valeur.Substring(0, nbCarsDegres));
      res += Convert.ToDouble(valeur.Substring(nbCarsDegres), _convertisseurVersDouble) / 60;
      if ((direction == "S") || (direction == "W"))
        res = -res;
      return res;
    }
  }
  public class DonneesGGA : DonneesPhraseNMEA
  {
    private TimeSpan _instantUTC = TimeSpan.Zero;

    public enum TypesPositionnement { tpInvalide, tpGPS, tpDGPS, tpPPS, tpKinematic, tpRTK, tpEstime, tpManuel, tpSimule }
    public TimeSpan InstantUTC                    { get { return _instantUTC; } }
    public TypesPositionnement TypePositionnement { get; private set; }
    public double Altitude                        { get; private set; }
    public double Latitude                        { get; private set; }
    public double Longitude                       { get; private set; }
    public byte NbSatellitesUtilises              { get; private set; }
    public double PrecisionHorizontale            { get; private set; }
    public DonneesGGA(string phraseGGA)
    {
      string[] donneesPhrase = phraseGGA.Split(',');
      TimeSpan.TryParseExact(ChaineVideEstZero(donneesPhrase[1]), "hhmmss\\.ff", null, out _instantUTC);
      TypePositionnement = (TypesPositionnement)Convert.ToByte(donneesPhrase[6]);
      Latitude = GetCoordonnee(ChaineVideEstZero(donneesPhrase[2]), donneesPhrase[3], 2);
      Longitude = GetCoordonnee(ChaineVideEstZero(donneesPhrase[4]), donneesPhrase[5], 3);
      Altitude = Convert.ToDouble(ChaineVideEstZero(donneesPhrase[9]), _convertisseurVersDouble);
      NbSatellitesUtilises = Convert.ToByte(ChaineVideEstZero(donneesPhrase[7]));
      PrecisionHorizontale = Convert.ToDouble(ChaineVideEstZero(donneesPhrase[8]), _convertisseurVersDouble);
    }
    public BasicGeoposition GetPositionGPS()
    {
      return new BasicGeoposition()  { Altitude = Altitude, Latitude = Latitude, Longitude = Longitude };
    }
  }
  public class DonneesRMC : DonneesPhraseNMEA
  {
    private DateTime _instantUTC;

    public DateTime InstantUTC { get { return _instantUTC; } }
    public bool Valide { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public double VitesseNoeuds { get; private set; }
    public double RouteDegres { get; private set; }
    public DonneesRMC(string phraseRMC)
    {
      string[] donneesPhrase = phraseRMC.Split(',');
      DateTime.TryParseExact(ChaineVideEstZero(donneesPhrase[9]) + ChaineVideEstZero(donneesPhrase[1]), "ddMMyyHHmmss\\.ff", null, DateTimeStyles.AssumeUniversal, out _instantUTC);
      Valide = (donneesPhrase[2] == "A");
      Latitude = GetCoordonnee(ChaineVideEstZero(donneesPhrase[3]), donneesPhrase[4], 2);
      Longitude = GetCoordonnee(ChaineVideEstZero(donneesPhrase[5]), donneesPhrase[6], 3);
      VitesseNoeuds = Convert.ToDouble(ChaineVideEstZero(donneesPhrase[7]), _convertisseurVersDouble);
      RouteDegres = Convert.ToDouble(ChaineVideEstZero(donneesPhrase[8]), _convertisseurVersDouble);
    }
  }
}
