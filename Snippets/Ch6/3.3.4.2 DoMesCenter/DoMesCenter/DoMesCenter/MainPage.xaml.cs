using DoMesCenterLIB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DoMesCenter
{
  /// <summary>
  /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
  /// </summary>
  public sealed partial class MainPage : Page
  {
    private Horloge _horloge = null;
    private DHT22 _dht22 = null;
    private bool _mesureEnCours = false;
    private object _mesureEnCoursLOCK = new object();
    private const ulong ADRESSE_BLUETOOTH_SENSORTAG = 0x247189bc1201;
    private SensorTagCC2650 _sensorTag = null;
    private void _horloge_SurToutesLesMinutes(object sender, DateTime instant)
    {
      LireValeurs();
    }
    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
      InitDHT22();
      InitHorloge();
      await InitSensorTag();

      LireValeurs();
    }
    private void InitHorloge()
    {
      _horloge = new Horloge();
      DateHeureTBK.DataContext = new HorlogeVM(_horloge);
      _horloge.SurToutesLesMinutes += _horloge_SurToutesLesMinutes;
    }
    private void InitDHT22()
    {
      GpioController gpc = GpioController.GetDefault();
      GpioPin dht22Pin = null;
      if (gpc == null)
        App.log.LogEvent("Contrôleur GPIO absent !", null, LoggingLevel.Warning);
      else
      {
        GpioOpenStatus status;
        if (!gpc.TryOpenPin(17, GpioSharingMode.Exclusive, out dht22Pin, out status))
          App.log.LogEvent("L'accès à la broche GPIO 17 est impossible : " + status.ToString(), null, LoggingLevel.Warning);
      }
      _dht22 = new DHT22(dht22Pin);
      DHT22SPL.DataContext = new DHT22VM(_dht22);
    }
    private async Task InitSensorTag()
    {
      _sensorTag = new SensorTagCC2650(ADRESSE_BLUETOOTH_SENSORTAG);
      SensorTagCC2650SPL.DataContext = new SensorTagCC2650VM(_sensorTag);
      if (await _sensorTag.Ouvrir())
      {
        _sensorTag.Connecter_TMP007();
        _sensorTag.Connecter_HDC1000();
        _sensorTag.Connecter_BMP280();
        _sensorTag.Connecter_OPT3001();
        _sensorTag.Connecter_ServiceBatterie();
      }
    }
    private async void LireValeurs()
    {
      lock (_mesureEnCoursLOCK)
      {
        if (_mesureEnCours)
          return;
        _mesureEnCours = true;
      }
      try
      {
        DHT22Mesure DHT22Result = _dht22.Lire(30);

        bool? TMP007Actif = await _sensorTag.Activer_TMP007(true);
        bool? HDC1000Actif = await _sensorTag.Activer_HDC1000(true);
        bool? BMP280Actif = await _sensorTag.Activer_BMP280(true);
        bool? OPT3001Actif = await _sensorTag.Activer_OPT3001(true);

        await _sensorTag.LireNiveauBatterie();

        await Task.Delay(500);

        TMP007Mesure TMP007res = null;
        if ((TMP007Actif.HasValue) && (TMP007Actif.Value))
          TMP007res = await _sensorTag.LireTMP007();
        HDC1000Mesure HDC1000res = null;
        if ((HDC1000Actif.HasValue) && (HDC1000Actif.Value))
          HDC1000res = await _sensorTag.LireHDC1000();
        BMP280Mesure BMP280res = null;
        if ((BMP280Actif.HasValue) && (BMP280Actif.Value))
          BMP280res = await _sensorTag.LireBMP280();
        OPT3001Mesure OPT3001res = null;
        if ((OPT3001Actif.HasValue) && (OPT3001Actif.Value))
          OPT3001res = await _sensorTag.LireOPT3001();

        await _sensorTag.Activer_TMP007(false);
        await _sensorTag.Activer_HDC1000(false);
        await _sensorTag.Activer_BMP280(false);
        await _sensorTag.Activer_OPT3001(false);
      }
      finally
      {
        lock (_mesureEnCoursLOCK)
          _mesureEnCours = false;
      }
    }

    public MainPage()
    {
      this.InitializeComponent();
    }   
  }
}
