using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace EncodeursLIB
{
  public class EncodeurIncremental2 : EncodeurIncrementalBase
  {
    private static readonly TimeSpan RESPIRATION = new TimeSpan(5000);
    private async void PollingProcess()
    {
      GpioPinValue dernierEtatA = _pinA.Read();
      GpioPinValue etatCourantA = dernierEtatA;
      bool sensHoraire;
      Task p;
      while (true)
      {
        etatCourantA = _pinA.Read();
        if ((etatCourantA == GpioPinValue.High) &&
            (dernierEtatA == GpioPinValue.Low))
        {
          sensHoraire = (_pinB.Read() == GpioPinValue.Low);
          p = Task.Run(() => { DeclencherSurRotationEncodeur(sensHoraire); });
        }
        await Task.Delay(RESPIRATION);
        dernierEtatA = etatCourantA;
      }
    }

    public EncodeurIncremental2(GpioPin pinA, GpioPin pinB) : base(pinA, pinB)
    {
      Task.Run(() => { PollingProcess(); });
    }
  }
}
