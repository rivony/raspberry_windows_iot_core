using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace EncodeursLIB
{
  public delegate void RotationEncodeur(bool sensHoraire);
  public abstract class EncodeurIncrementalBase
  {
    protected GpioPin _pinA = null;
    protected GpioPin _pinB = null;
    protected void DeclencherSurRotationEncodeur(bool sensHoraire)
    {
      SurRotationEncodeur?.Invoke(sensHoraire);
    }

    public EncodeurIncrementalBase(GpioPin pinA, GpioPin pinB)
    {
      _pinA = pinA;
      _pinA.SetDriveMode(GpioPinDriveMode.InputPullUp);
      _pinA.DebounceTimeout = new TimeSpan(20000);

      _pinB = pinB;
      _pinB.SetDriveMode(GpioPinDriveMode.InputPullUp);
      _pinB.DebounceTimeout = new TimeSpan(20000);
    }
    public event RotationEncodeur SurRotationEncodeur;
  }
}
