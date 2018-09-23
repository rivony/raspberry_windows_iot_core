using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Media.Capture;

namespace GpsCamLIB
{
  public class AppareilPhoto
  {
    private GpioPin _ledOccupe = null;
    private MediaCapture _mediaCapture = null;
    private async Task<bool> InitialiserSourceImages(uint largeur, uint hauteur, string sousTypeVideo)
    {
      return true;
    }

    public AppareilPhoto(GpioPin occupe)
    {
      _ledOccupe = occupe;
      _ledOccupe?.SetDriveMode(GpioPinDriveMode.Output);
    }
    public async Task<bool> Initialiser(uint largeur, uint hauteur, string sousTypeVideo)
    {
      bool res = false;
      _ledOccupe?.Write(GpioPinValue.High);
      _mediaCapture = new MediaCapture();
      await _mediaCapture.InitializeAsync(new MediaCaptureInitializationSettings()  { SharingMode = MediaCaptureSharingMode.ExclusiveControl,
                                                                                      MemoryPreference = MediaCaptureMemoryPreference.Cpu,
                                                                                      StreamingCaptureMode = StreamingCaptureMode.Video
                                                                                    });
      res = await InitialiserSourceImages(largeur, hauteur, sousTypeVideo);
      _ledOccupe?.Write(GpioPinValue.Low);
      return res;
    }

    
  }
}
