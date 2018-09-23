using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;

namespace DoMesCenterLIB
{
  public delegate void TMP007_NouvelleValeur(TMP007Mesure nouvelleValeur);
  public class SensorTagCC2650
  {
    private ulong _adresseBluetooth;
    private BluetoothLEDevice _sensorTagBle = null;
    private TMP007_SensorTagService _TMP007_Service = null;
    private void _TMP007_Service_SurNouvelleValeur(object sender, TMP007Mesure e)
    {
      SurTMP007_NouvelleValeur?.Invoke(e);
    }

    public SensorTagCC2650(ulong bluetoothAddress)
    {
      _adresseBluetooth = bluetoothAddress;
    }
    public async Task<bool> Ouvrir()
    {
      if (_sensorTagBle == null)
        try
        {
          _sensorTagBle = await BluetoothLEDevice.FromBluetoothAddressAsync(_adresseBluetooth);
          return (_sensorTagBle != null);
        }
        catch
        {
          _sensorTagBle = null;
        }
      return (_sensorTagBle != null);
    }
    public bool Connecter_TMP007()
    {
      if (_TMP007_Service == null)
        _TMP007_Service = TMP007_SensorTagService.CreerSensorTagService(_sensorTagBle);
      if ((_TMP007_Service != null) && (_TMP007_Service.Initialiser()))
      {
        _TMP007_Service.SurNouvelleValeur += _TMP007_Service_SurNouvelleValeur;
        return true;
      }
      else
      {
        _TMP007_Service = null;
        return false;
      }
    }
    public async Task<bool?> Activer_TMP007(bool active)
    {
      return await _TMP007_Service?.ActiverCapteur(active);
    }
    public async Task<TMP007Mesure> LireTMP007()
    {
      return await _TMP007_Service?.LireCapteur();
    }
    public event TMP007_NouvelleValeur SurTMP007_NouvelleValeur;
  }
}
