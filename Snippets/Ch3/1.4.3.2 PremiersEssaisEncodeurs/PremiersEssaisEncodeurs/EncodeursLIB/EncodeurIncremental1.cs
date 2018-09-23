using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace EncodeursLIB
{
  public class EncodeurIncremental1 : EncodeurIncrementalBase
  {
    private void _pinA_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
    {
      if (args.Edge == GpioPinEdge.RisingEdge)
        DeclencherSurRotationEncodeur(_pinB.Read() == GpioPinValue.Low);
    }

    public EncodeurIncremental1(GpioPin pinA, GpioPin pinB) : base(pinA, pinB)
    {
      _pinA.ValueChanged += _pinA_ValueChanged; ;
    }
  }
}
