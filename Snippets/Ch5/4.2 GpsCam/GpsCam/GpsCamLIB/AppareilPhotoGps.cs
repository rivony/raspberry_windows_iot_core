using SerialLIB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Graphics.Imaging;

namespace GpsCamLIB
{
  public class AppareilPhotoGps : AppareilPhoto
  {
    private static readonly TimeSpan DUREE_VALIDITE_POSITION_GPS = new TimeSpan(0, 0, 15);
    private const long DENOMINATEUR_SECONDES = 10000;
    private GpsNMEA _gps = null;
    private DonneesGGA _derniereGGARecue = null;
    private object _derniereGGARecueLOCK = new object();
    private DateTime _instantReceptionDerniereGGA = DateTime.MinValue;
    private void _gps_SurPhraseGGA(object sender, DonneesGGA donnees)
    {
      if (donnees.TypePositionnement != DonneesGGA.TypesPositionnement.tpInvalide)
        lock (_derniereGGARecueLOCK)
        {
          _derniereGGARecue = donnees;
          _instantReceptionDerniereGGA = DateTime.Now;
        }
    }
    private BasicGeoposition? GetDernierePositionGPSValide()
    {
      lock (_derniereGGARecueLOCK)
      {
        if (_derniereGGARecue != null)
          if (DateTime.Now - _instantReceptionDerniereGGA <= DUREE_VALIDITE_POSITION_GPS)
            return _derniereGGARecue.GetPositionGPS();
      }
      return null;
    }
    private Dictionary<string, BitmapTypedValue> CreerDonneesExifGps(BasicGeoposition positionGps)
    {
      Dictionary<string, BitmapTypedValue> res = new Dictionary<string, BitmapTypedValue>();

      res.Add("System.GPS.LatitudeRef", new BitmapTypedValue((positionGps.Latitude >= 0) ? "N" : "S", PropertyType.String));
      res.Add("System.GPS.LatitudeNumerator", CreerCoordonneeNumerateur(positionGps.Latitude));
      res.Add("System.GPS.LatitudeDenominator", CreerCoordonneeDenominateur());

      res.Add("System.GPS.LongitudeRef", new BitmapTypedValue((positionGps.Longitude >= 0) ? "E" : "W", PropertyType.String));
      res.Add("System.GPS.LongitudeNumerator", CreerCoordonneeNumerateur(positionGps.Longitude));
      res.Add("System.GPS.LongitudeDenominator", CreerCoordonneeDenominateur());

      res.Add("System.GPS.AltitudeRef", new BitmapTypedValue(0, PropertyType.UInt8));
      res.Add("System.GPS.AltitudeNumerator", new BitmapTypedValue(positionGps.Altitude * 100, PropertyType.UInt32));
      res.Add("System.GPS.AltitudeDenominator", new BitmapTypedValue(100, PropertyType.UInt32));

      return res;
    }
    private BitmapTypedValue CreerCoordonneeNumerateur(double coord)
    {
      long[] coordNumerateur = new long[3];

      double lat = Math.Abs(coord);
      double latDegres = Math.Truncate(lat);
      coordNumerateur[0] = (long)latDegres;

      double latMinutes = (lat - latDegres) * 60;
      double latMinutesTrunc = Math.Truncate(latMinutes);
      coordNumerateur[1] = (long)latMinutesTrunc;

      double latSecondes = (latMinutes - latMinutesTrunc) * 60 * DENOMINATEUR_SECONDES;
      coordNumerateur[2] = (long)Math.Truncate(latSecondes);

      return new BitmapTypedValue(coordNumerateur, PropertyType.UInt32Array);
    }
    private BitmapTypedValue CreerCoordonneeDenominateur()
    {
      return new BitmapTypedValue(new long[3] { 1, 1, DENOMINATEUR_SECONDES }, PropertyType.UInt32Array);
    }

    protected override async Task AjouterDonneesExif(BitmapEncoder encodeur)
    {
      BasicGeoposition? positionGPS = GetDernierePositionGPSValide();
      if (positionGPS.HasValue)
        await encodeur.BitmapProperties.SetPropertiesAsync(CreerDonneesExifGps(positionGPS.Value));
    }

    public AppareilPhotoGps(GpioPin occupe, GpsNMEA gps) : base(occupe)
    {
      _gps = gps;
      _gps.SurPhraseGGA += _gps_SurPhraseGGA;
    }

    
  }
}
