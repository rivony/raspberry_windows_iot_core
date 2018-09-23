using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.Spi;

namespace PiFaceLIB
{
  public delegate void ChangementValeur(byte nouvellesvaleurs);
  public class PiFaceDigital
  {
    private const byte ICON = 0x0A;

    private const byte IODIRA = 0x00;
    private const byte GPIOA = 0x09;

    private const byte IODIRB = 0x10;
    private const byte IPOLB = 0x11;
    private const byte GPINTENB = 0x12;
    private const byte INTCONB = 0x14;
    private const byte GPPUB = 0x16;
    private const byte INTCAPB = 0x18;
    private const byte GPIOB = 0x19;

    private SpiConnectionSettings _spiParams = null;
    private byte _adresse = 0;
    private SpiDevice _spi = null;
    private GpioPin _intPin = null;
    private void _intPin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
    {
      if (args.Edge == GpioPinEdge.FallingEdge)
        SurChangementEntrees?.Invoke(LireRegistre(INTCAPB));
    }
    private void Ecrire(byte registre, byte valeur)
    {
      _spi.Write(new byte[3] { _adresse, registre, valeur });
    }
    private byte LireRegistre(byte registre)
    {
      byte[] octetsRecus = new byte[3];
      _spi.TransferFullDuplex(new byte[3] { (byte)(_adresse | 1), registre, 0 }, octetsRecus);
      return octetsRecus[2];
    }
    private void Set8BitsMode()
    {
      _spi.Write(new byte[4] { _adresse, ICON, 0x8C, 0x8C });
    }

    public PiFaceDigital(byte adresse, int csLine)
    {
      _spiParams = new SpiConnectionSettings(csLine);
      _spiParams.ClockFrequency = 1000000;
      _spiParams.Mode = SpiMode.Mode0;
      _adresse = (byte)((byte)0x40 | (adresse << 1));
    }
    public async Task<bool> Initialiser(GpioController gpc)
    {
      string spi0aqs = SpiDevice.GetDeviceSelector("SPI0");
      DeviceInformationCollection busSPI = await DeviceInformation.FindAllAsync(spi0aqs);
      if (busSPI.Count > 0)
      {
        _spi = await SpiDevice.FromIdAsync(busSPI[0].Id, _spiParams);
        if (_spi != null)
        {
          _intPin = gpc.OpenPin(25);
          _intPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
          _intPin.ValueChanged += _intPin_ValueChanged; ;

          Set8BitsMode();

          Ecrire(IODIRA, 0x00);
          Ecrire(GPIOA, 0x00);

          Ecrire(IODIRB, 0xFF);
          Ecrire(GPPUB, 0xFF);
          Ecrire(GPINTENB, 0xFF);
          Ecrire(INTCONB, 0x00);
          Ecrire(IPOLB, 0xFF);
          LireRegistre(GPIOB);
          return true;
        }
      }
      return false;
    }
    public byte LireEntrees()
    {
      return LireRegistre(GPIOB);
    }
    public void EcrireSorties(byte valeursSorties)
    {
      Ecrire(GPIOA, valeursSorties);
      SurChangementSorties?.Invoke(valeursSorties);
    }
    public event ChangementValeur SurChangementEntrees;
    public event ChangementValeur SurChangementSorties;
  }
}
