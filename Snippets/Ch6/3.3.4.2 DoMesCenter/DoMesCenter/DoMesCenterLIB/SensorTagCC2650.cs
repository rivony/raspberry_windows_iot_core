using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace DoMesCenterLIB
{
  public delegate void TMP007_NouvelleValeur(TMP007Mesure nouvelleValeur);
  public delegate void HDC1000_NouvelleValeur(HDC1000Mesure nouvelleValeur);
  public delegate void BMP280_NouvelleValeur(BMP280Mesure nouvelleValeur);
  public delegate void OPT3001_NouvelleValeur(OPT3001Mesure nouvelleValeur);
  public delegate void NiveauBatterie_NouvelleValeur(byte? nouvelleValeur);
  public class SensorTagCC2650
  {
    private ulong _adresseBluetooth;
    private BluetoothLEDevice _sensorTagBle = null;
    private TMP007_SensorTagService _TMP007_Service = null;
    private HDC1000_SensorTagService _HDC1000_Service = null;
    private BMP280_SensorTagService _BMP280_Service = null;
    private OPT3001_SensorTagService _OPT3001_Service = null;
    private GattDeviceService _Batterie_Service = null;
    private GattCharacteristic _BatterieNiveauCaracteristique = null;
    private static readonly Guid _BatterieServiceGuid = new Guid("0000180f-0000-1000-8000-00805f9b34fb");
    private static readonly Guid _BatterieNiveauCaracteristiqueGuid = new Guid("00002a19-0000-1000-8000-00805f9b34fb");
    private async Task<DataReader> LireCaracteristique(GattCharacteristic gattCaracteristique)
    {
      GattReadResult res = await gattCaracteristique.ReadValueAsync(BluetoothCacheMode.Uncached);
      if (res.Status == GattCommunicationStatus.Success)
        return DataReader.FromBuffer(res.Value);
      else
        return null;
    }
    private void _TMP007_Service_SurNouvelleValeur(object sender, TMP007Mesure e)
    {
      SurTMP007_NouvelleValeur?.Invoke(e);
    }
    private void _HDC1000_Service_SurNouvelleValeur(object sender, HDC1000Mesure e)
    {
      SurHDC1000_NouvelleValeur?.Invoke(e);
    }
    private void _BMP280_Service_SurNouvelleValeur(object sender, BMP280Mesure e)
    {
      SurBMP280_NouvelleValeur?.Invoke(e);
    }
    private void _OPT3001_Service_SurNouvelleValeur(object sender, OPT3001Mesure e)
    {
      SurOPT3001_NouvelleValeur?.Invoke(e);
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
    public bool Connecter_HDC1000()
    {
      if (_HDC1000_Service == null)
        _HDC1000_Service = HDC1000_SensorTagService.CreerSensorTagService(_sensorTagBle);
      if ((_HDC1000_Service != null) && (_HDC1000_Service.Initialiser()))
      {
        _HDC1000_Service.SurNouvelleValeur += _HDC1000_Service_SurNouvelleValeur;
        return true;
      }
      else
      {
        _HDC1000_Service = null;
        return false;
      }
    }
    public bool Connecter_BMP280()
    {
      if (_BMP280_Service == null)
        _BMP280_Service = BMP280_SensorTagService.CreerSensorTagService(_sensorTagBle);
      if ((_BMP280_Service != null) && (_BMP280_Service.Initialiser()))
      {
        _BMP280_Service.SurNouvelleValeur += _BMP280_Service_SurNouvelleValeur;
        return true;
      }
      else
      {
        _BMP280_Service = null;
        return false;
      }
    }
    public bool Connecter_OPT3001()
    {
      if (_OPT3001_Service == null)
        _OPT3001_Service = OPT3001_SensorTagService.CreerSensorTagService(_sensorTagBle);
      if ((_OPT3001_Service != null) && (_OPT3001_Service.Initialiser()))
      {
        _OPT3001_Service.SurNouvelleValeur += _OPT3001_Service_SurNouvelleValeur;
        return true;
      }
      else
      {
        _OPT3001_Service = null;
        return false;
      }
    }
    public bool Connecter_ServiceBatterie()
    {
      if (_Batterie_Service == null)
        try
        {
          _Batterie_Service = _sensorTagBle.GetGattService(_BatterieServiceGuid);
        }
        catch
        {
          _Batterie_Service = null;
        }
      if (_Batterie_Service != null)
      {
        _BatterieNiveauCaracteristique = _Batterie_Service.GetCharacteristics(_BatterieNiveauCaracteristiqueGuid).FirstOrDefault();
        if (_BatterieNiveauCaracteristique != null)
          return true;
      }
      _Batterie_Service = null;
      return false;
    }
    public async Task<bool?> Activer_TMP007(bool active)
    {
      return await _TMP007_Service?.ActiverCapteur(active);
    }
    public async Task<bool?> Activer_HDC1000(bool active)
    {
      if (_HDC1000_Service != null)
        return await _HDC1000_Service.ActiverCapteur(active);
      else
        return null;
    }
    public async Task<bool?> Activer_BMP280(bool active)
    {
      if (_BMP280_Service != null)
        return await _BMP280_Service.ActiverCapteur(active);
      else
        return null;
    }
    public async Task<bool?> Activer_OPT3001(bool active)
    {
      if (_OPT3001_Service != null)
        return await _OPT3001_Service.ActiverCapteur(active);
      else
        return null;
    }
    public async Task<TMP007Mesure> LireTMP007()
    {
      return await _TMP007_Service?.LireCapteur();
    }
    public async Task<HDC1000Mesure> LireHDC1000()
    {
      if (_HDC1000_Service != null)
        return await _HDC1000_Service.LireCapteur();
      else
        return null;
    }
    public async Task<BMP280Mesure> LireBMP280()
    {
      if (_BMP280_Service != null)
        return await _BMP280_Service.LireCapteur();
      else
        return null;
    }
    public async Task<OPT3001Mesure> LireOPT3001()
    {
      if (_OPT3001_Service != null)
        return await _OPT3001_Service.LireCapteur();
      else
        return null;
    }
    public async Task<byte?> LireNiveauBatterie()
    {
      byte? res = null;
      if (_BatterieNiveauCaracteristique != null)
      {
        DataReader dr = await LireCaracteristique(_BatterieNiveauCaracteristique);
        if (dr != null)
          res = dr.ReadByte();
        SurNiveauBatterie_NouvelleValeur?.Invoke(res);
      }
      return res;
    }
    public event TMP007_NouvelleValeur SurTMP007_NouvelleValeur;
    public event HDC1000_NouvelleValeur SurHDC1000_NouvelleValeur;
    public event BMP280_NouvelleValeur SurBMP280_NouvelleValeur;
    public event OPT3001_NouvelleValeur SurOPT3001_NouvelleValeur;
    public event NiveauBatterie_NouvelleValeur SurNiveauBatterie_NouvelleValeur;
  }
}
