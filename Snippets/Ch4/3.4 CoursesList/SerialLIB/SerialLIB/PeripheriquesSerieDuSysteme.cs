using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;

namespace SerialLIB
{
  public class PeripheriquesSerieDuSysteme
  {
    protected DeviceWatcher _scrutateur = null;

    public PeripheriquesSerieDuSysteme()
    {
      _scrutateur = DeviceInformation.CreateWatcher(SerialDevice.GetDeviceSelector());
    }
    public void DemarrerSurveillance()
    {
      _scrutateur.Start();
    }
    public void AbonnerPeripherique(PeripheriqueSerie peripherique)
    {
      _scrutateur.Added += peripherique.PeripheriqueSerieAjoute;
      _scrutateur.Removed += peripherique.PeripheriqueSerieSupprime;
    }
  }
}
