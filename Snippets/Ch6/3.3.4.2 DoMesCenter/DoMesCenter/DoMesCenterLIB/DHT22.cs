using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace DoMesCenterLIB
{
  public delegate void DHT22NouvelleMesure(object sender, DHT22Mesure nouvelleMesure);
  public class DHT22
  {
    private GpioPin _pin = null;
    private static DHT22Mesure ConvertirValeurs(DHT22SignalBus signalDHT)
    {
      uint humidMSB = LireOctet(signalDHT, 0);
      uint humidLSB = LireOctet(signalDHT, 1);
      uint TempMSB  = LireOctet(signalDHT, 2);
      uint TempLSB  = LireOctet(signalDHT, 3);
      uint chk      = LireOctet(signalDHT, 4);

      if (((humidMSB + humidLSB + TempLSB + TempMSB) & 0xff) != chk)
        return null;
      else
        return new DHT22Mesure(humidMSB, humidLSB, TempMSB, TempLSB);
    }
    private static byte LireOctet(DHT22SignalBus signalDHT, byte noOctet)
    {
      byte res = 0;
      for (int i = noOctet * 8; i < (noOctet + 1) * 8; i++)
      {
        res <<= 1;
        res += LireBit(signalDHT, i);
      }
      return res;
    }
    private static byte LireBit(DHT22SignalBus signalDHT, int noBit)
    {
      return ((signalDHT.tFe[noBit + 1] - signalDHT.tFe[noBit]) > DHT22SignalBus.SEUIL_BIT) ? (byte)1 : (byte)0;
    }

    public DHT22(GpioPin pin)
    {
      _pin = pin;
    }
    public DHT22Mesure Lire(int nbMaxEssais)
    {
      DHT22Mesure res = null;
      if (_pin != null)
      {
        int i = 0;
        while ((i < nbMaxEssais) && (res == null))
        {
          DHT22SignalBus SignalDHT22 = DHT22SignalBus.LireSignal(_pin);
          if (SignalDHT22.iFe >= DHT22SignalBus.NB_MAX_FRONTS_DESCENDANTS)
            res = ConvertirValeurs(SignalDHT22);
          i++;
        }
      }
      SurNouvelleMesure?.Invoke(this, res);
      return res;
    }
    public event DHT22NouvelleMesure SurNouvelleMesure;
  }

  public class DHT22Mesure
  {
    public double Temperature { get; private set; }
    public double Humidite { get; private set; }
    public DHT22Mesure(uint humidMSB, uint humidLSB, uint TempMSB, uint TempLSB)
    {
      Temperature = (((TempMSB & 0x7f) << 8) + TempLSB) / 10.0;
      if ((TempMSB & 0x80) != 0)
        Temperature = -Temperature;
      Humidite = ((humidMSB << 8) + humidLSB) / 10.0;
    }
  }

  public class DHT22SignalBus
  {
    protected long _t1max = 0;
    protected long _t2max = 0;
    protected long _t3max = 0;
    protected static readonly long _l1max = MicrosSecVersTicks(20000);
    protected static readonly long _l2max = MicrosSecVersTicks(1000);
    protected static readonly long _l3max = MicrosSecVersTicks(10000);
    protected static long MicrosSecVersTicks(double microSecs)
    {
      return Convert.ToInt64(microSecs * Stopwatch.Frequency / 1e6);
    }

    public static readonly int NB_MAX_FRONTS_DESCENDANTS = 41;
    public static readonly long SEUIL_BIT = MicrosSecVersTicks(110);
    public long[] tFe = new long[NB_MAX_FRONTS_DESCENDANTS];
    public int iFe = 0;
    public static DHT22SignalBus LireSignal(GpioPin pin)
    {
      DHT22SignalBus res = new DHT22SignalBus();
      GpioPinValue etatCourant;
      GpioPinValue etatPrecedent;

      //******** Commande *********
      pin.Write(GpioPinValue.Low);
      pin.SetDriveMode(GpioPinDriveMode.Output);
      res._t1max = Stopwatch.GetTimestamp() + _l1max;
      while (Stopwatch.GetTimestamp() < res._t1max)
      { }
      pin.SetDriveMode(GpioPinDriveMode.Input);

      //***** Attente retour ******
      etatPrecedent = pin.Read();
      res._t2max = Stopwatch.GetTimestamp() + _l2max;
      do
      {
        etatCourant = pin.Read();
        if ((etatCourant != etatPrecedent) && (etatCourant == GpioPinValue.High))
          break;
        if (Stopwatch.GetTimestamp() > res._t2max)
          return res;
      } while (true);

      //****** Acquisitions *******
      res._t3max = Stopwatch.GetTimestamp() + _l3max;
      do
      {
        etatCourant = pin.Read();
        if ((etatPrecedent == GpioPinValue.High) && (etatCourant == GpioPinValue.Low))
          res.tFe[res.iFe++] = Stopwatch.GetTimestamp();
        etatPrecedent = etatCourant;
      } while ((res.iFe < NB_MAX_FRONTS_DESCENDANTS) && (Stopwatch.GetTimestamp() < res._t3max));
      return res;
    }
  }
}
