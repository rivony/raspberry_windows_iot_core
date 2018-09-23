using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace EncodeursLIB
{
  public delegate void ChangementPosition(byte nouvellePosition);
  public class EncodeurAbsolu
  {
    private static readonly byte[] _pinValuesToPosition = new byte[256] { 0xFF,   56,   40,   55,   24, 0xFF,   39,   52,    8,   57, 0xFF, 0xFF,   23, 0xFF,   36,   13,  120, 0xFF,   41,   54, 0xFF, 0xFF, 0xFF,   53,    7, 0xFF, 0xFF, 0xFF,   20,   19,  125,   18,  104,  105, 0xFF, 0xFF,   25,  106,   38, 0xFF, 0xFF,   58, 0xFF, 0xFF, 0xFF, 0xFF,   37,   14,  119,  118, 0xFF, 0xFF, 0xFF,  107, 0xFF, 0xFF,    4, 0xFF,    3, 0xFF, 109,  108,    2,    1,   88, 0xFF,   89, 0xFF, 0xFF, 0xFF, 0xFF,   51,    9,   10,   90, 0xFF,   22,   11, 0xFF,   12, 0xFF, 0xFF,   42,   43, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,   21, 0xFF,  126,  127,  103, 0xFF,  102, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,   91, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,  116,  117, 0xFF, 0xFF,  115, 0xFF, 0xFF, 0xFF,  93,   94,   92, 0xFF,  114,   95,  113,    0,   72,   71, 0xFF,   68,   73, 0xFF, 0xFF,   29, 0xFF,   70, 0xFF,   69, 0xFF, 0xFF,   35,   34,  121, 0xFF,  122, 0xFF,   74, 0xFF, 0xFF,   30,    6, 0xFF,  123, 0xFF, 0xFF, 0xFF,  124,   17, 0xFF, 0xFF, 0xFF,   67,   26, 0xFF,   27,   28, 0xFF,   59, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,   15, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,    5, 0xFF, 0xFF, 0xFF,  110, 0xFF,  111,   16,   87,   84, 0xFF,   45,   86,   85, 0xFF,   50, 0xFF, 0xFF, 0xFF,   46, 0xFF, 0xFF, 0xFF,   33, 0xFF,   83, 0xFF,   44,   75, 0xFF, 0xFF,   31, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,   32,  100,   61,  101,   66, 0xFF,   62, 0xFF,   49,   99,   60, 0xFF,   47, 0xFF, 0xFF, 0xFF,   48,  77,   82,   78,   65,   76,   63, 0xFF,   64,   98,   81,  79,   80,   97,   96,  112, 0xFF };
    private GpioPin[] _pins = new GpioPin[8];
    private GpioPinValue _etatActif;
    private void EncodeurAbsolu_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
    {
      SurChangementPosition?.Invoke(LirePosition());
    }
    private byte GetValeurPin(GpioPin pin)
    {
      return (pin.Read() == _etatActif) ? (byte)1 : (byte)0;
    }

    public EncodeurAbsolu(GpioPin[] pins, bool cToGND)
    {
      for (int i = 0; i < 8; i++)
      {
        _pins[i] = pins[i];
        _pins[i].SetDriveMode(cToGND ? GpioPinDriveMode.InputPullUp : GpioPinDriveMode.InputPullDown);
        _pins[i].ValueChanged += EncodeurAbsolu_ValueChanged; ;
      }
      _etatActif = cToGND ? GpioPinValue.High : GpioPinValue.Low;
    }
    public byte LirePosition()
    {
      byte valeurBrute = (byte)(GetValeurPin(_pins[7]) << 7 |
                                GetValeurPin(_pins[6]) << 6 |
                                GetValeurPin(_pins[5]) << 5 |
                                GetValeurPin(_pins[4]) << 4 |
                                GetValeurPin(_pins[3]) << 3 |
                                GetValeurPin(_pins[2]) << 2 |
                                GetValeurPin(_pins[1]) << 1 |
                                GetValeurPin(_pins[0]));
      return _pinValuesToPosition[valeurBrute];
    }
    public event ChangementPosition SurChangementPosition;
  }
}
