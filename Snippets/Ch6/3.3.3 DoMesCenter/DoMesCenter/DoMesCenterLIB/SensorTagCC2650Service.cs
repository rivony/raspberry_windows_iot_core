using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace DoMesCenterLIB
{
  public abstract class SensorTagService<T> where T : ResultatDeMesure
  {
    private GattDeviceService _gattService = null;
    private GattCharacteristic _gattCaracteristiqueConfig = null;
    private GattCharacteristic _gattCaracteristiqueData = null;
    private async Task<bool?> GetRegistreConfig()
    {
      GattReadResult res = await _gattCaracteristiqueConfig.ReadValueAsync(BluetoothCacheMode.Uncached);
      if (res.Status == GattCommunicationStatus.Unreachable)
        return null;
      DataReader dataR = DataReader.FromBuffer(res.Value);
      return (dataR.ReadByte() == 0x01);
    }
    private async Task<bool> SetRegistreConfig(bool active)
    {
      DataWriter dataW = new DataWriter();
      if (active)
        dataW.WriteByte(0x01);
      else
        dataW.WriteByte(0x00);
      return (await
     _gattCaracteristiqueConfig.WriteValueAsync(dataW.DetachBuffer()) == GattCommunicationStatus.Success);
    }
    private async Task<DataReader> GetRegistreData()
    {
      GattReadResult res = await _gattCaracteristiqueData.ReadValueAsync(BluetoothCacheMode.Uncached);
      if (res.Status == GattCommunicationStatus.Success)
        return DataReader.FromBuffer(res.Value);
      else
        return null;
    }

    protected Guid _ConfCaracGuid { get; set; }
    protected Guid _DataCaracGuid { get; set; }
    protected SensorTagService(GattDeviceService gattService)
    {
      _gattService = gattService;
    }
    protected abstract T CreerResultatDeMesure(DataReader dataR);

    public bool Initialiser()
    {
      try
      {
        _gattCaracteristiqueConfig = _gattService.GetCharacteristics(_ConfCaracGuid).First();
        _gattCaracteristiqueData = _gattService.GetCharacteristics(_DataCaracGuid).First();
        return true;
      }
      catch
      {
        _gattCaracteristiqueConfig = null;
        _gattCaracteristiqueData = null;
      }
      return false;
    }
    public async Task<bool> ActiverCapteur(bool active)
    {
      bool? capteurActive = await GetRegistreConfig();
      if (!capteurActive.HasValue)
        return false;
      if (capteurActive.Value == active)
        return true;
      return await SetRegistreConfig(active);
    }
    public async Task<T> LireCapteur()
    {
      T res = null;
      DataReader dataR = await GetRegistreData();
      if (dataR != null)
        res = CreerResultatDeMesure(dataR);
      SurNouvelleValeur?.Invoke(this, res);
      return res;
    }
    public event EventHandler<T> SurNouvelleValeur;
  }

  public abstract class ResultatDeMesure
  {
    public ResultatDeMesure(DataReader donnees) { }
  }
  public class TMP007Mesure : ResultatDeMesure
  {
    public double TemperatureAmbiante { get; private set; }
    public double TemperatureIR       { get; private set; }
    public TMP007Mesure(DataReader donnees) : base(donnees)
    {
      donnees.ByteOrder = ByteOrder.LittleEndian;
      short IRt = donnees.ReadInt16();
      short Amt = donnees.ReadInt16();
      TemperatureIR = (IRt >> 2) * 0.03125;
      TemperatureAmbiante = (Amt >> 2) * 0.03125;
      Debug.WriteLine("TMP007 Mesure : Temp IR = " + TemperatureIR.ToString() + "°C ;  Temp Amb = " + TemperatureAmbiante.ToString() + "°C");
    }
  }
  public class TMP007_SensorTagService : SensorTagService<TMP007Mesure>
  {
    private static readonly Guid ServiceGuid = new Guid("f000aa00-0451-4000-b000-000000000000");
    private TMP007_SensorTagService(GattDeviceService gattService) : base(gattService)
    {
      _ConfCaracGuid = new Guid("f000aa02-0451-4000-b000-000000000000");
      _DataCaracGuid = new Guid("f000aa01-0451-4000-b000-000000000000");
    }

    protected override TMP007Mesure CreerResultatDeMesure(DataReader dataR)
    {
      return new TMP007Mesure(dataR);
    }

    public static TMP007_SensorTagService CreerSensorTagService(BluetoothLEDevice sensorTagBle)
    {
      TMP007_SensorTagService res = null;
      try
      {
        GattDeviceService service = sensorTagBle.GetGattService(ServiceGuid);
        if (service != null)
          res = new TMP007_SensorTagService(service);
      }
      catch (Exception)
      {
      }
      return res;
    }
  }
}
