using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.Devices;
using Windows.Storage;
using Windows.Storage.Streams;

namespace GpsCamLIB
{
  public class AppareilPhoto
  {
    private GpioPin _ledOccupe = null;
    private MediaCapture _mediaCapture = null;
    private MediaFrameReader _lecteurMedia = null;
    private int _photoAPrendre = 0;
    private object _photoAPrendreLOCK = new object();
    private const double QUALITE_ENCODAGE_IMAGE = 0.8d;
    private BitmapPropertySet _qualiteEncodageImage = new BitmapPropertySet();
    private async Task<bool> InitialiserSourceImages(uint largeur, uint hauteur, string sousTypeVideo)
    {
      if (_mediaCapture.FrameSources.Count > 0)
      {
        MediaFrameSource sourceImages = _mediaCapture.FrameSources.First().Value;
        ReglerControleurVideo(sourceImages);

        MediaFrameFormat formatVideo = sourceImages.SupportedFormats.FirstOrDefault(f => f.VideoFormat.Width == largeur && f.VideoFormat.Height == hauteur && f.Subtype == sousTypeVideo);
        if (formatVideo != null)
        {
          await sourceImages.SetFormatAsync(formatVideo);

          _lecteurMedia = await _mediaCapture.CreateFrameReaderAsync(sourceImages);
          _lecteurMedia.FrameArrived += _lecteurMedia_FrameArrived;
          await _lecteurMedia.StartAsync();
          return true;
        }
        else
        {
          Debug.WriteLine("Le format demandé " + largeur.ToString() + "x" + hauteur.ToString() + " en " + sousTypeVideo + " n'est pas supporté par cette caméra !");
          Debug.WriteLine("Voici les formats supportés :");
          foreach (MediaFrameFormat formatImages in sourceImages.SupportedFormats)
            Debug.WriteLine(formatImages.MajorType + " " + formatImages.VideoFormat.Width.ToString() + "x" + formatImages.VideoFormat.Height.ToString() + " " + formatImages.Subtype);
          return false;
        }
      }
      else
      {
        Debug.WriteLine("Aucun périphérique de capture détecté !");
        return false;
      }
    }
    private async void _lecteurMedia_FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
    {
      lock (_photoAPrendreLOCK)
      {
        if (_photoAPrendre != 1) return;
        _photoAPrendre++;
      }

      _ledOccupe?.Write(GpioPinValue.High);
      MediaFrameReference imageCourante = sender.TryAcquireLatestFrame();
      if (!(imageCourante == null || imageCourante.VideoMediaFrame == null || imageCourante.VideoMediaFrame.SoftwareBitmap == null))
      {
        await EnregistrerImage(imageCourante);
        lock (_photoAPrendreLOCK) { _photoAPrendre = 0; }
      }
      _ledOccupe?.Write(GpioPinValue.Low);
    }
    private static void ReglerControleurVideo(MediaFrameSource sourceImages)
    {
      VideoDeviceController controleurVideo = sourceImages.Controller.VideoDeviceController;
      controleurVideo.DesiredOptimization = MediaCaptureOptimization.Quality;
      controleurVideo.PrimaryUse = CaptureUse.Video;
      if (controleurVideo.Exposure.Capabilities.Supported && controleurVideo.Exposure.Capabilities.AutoModeSupported)
        controleurVideo.Exposure.TrySetAuto(true);
    }
    private async Task EnregistrerImage(MediaFrameReference image)
    {
      using (InMemoryRandomAccessStream imageStream = new InMemoryRandomAccessStream())
      {
        await EncoderImage(image, imageStream);
        StorageFile fichierPhoto = await KnownFolders.PicturesLibrary.CreateFileAsync("GpsCam.jpg", CreationCollisionOption.GenerateUniqueName);
        using (IRandomAccessStream photoFileStream = await fichierPhoto.OpenAsync(FileAccessMode.ReadWrite))
          await RandomAccessStream.CopyAndCloseAsync(imageStream.GetInputStreamAt(0), photoFileStream.GetOutputStreamAt(0));
      }
    }
    private async Task EncoderImage(MediaFrameReference image, IRandomAccessStream imageStream)
    {
      using (SoftwareBitmap bitmap = SoftwareBitmap.Convert(image.VideoMediaFrame.SoftwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore))
      {
        BitmapEncoder encodeur = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, imageStream, _qualiteEncodageImage);
        encodeur.SetSoftwareBitmap(bitmap);

        await encodeur.FlushAsync();
      }
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
      _qualiteEncodageImage.Add("ImageQuality", new BitmapTypedValue(QUALITE_ENCODAGE_IMAGE, PropertyType.Single));
      _mediaCapture = new MediaCapture();
      await _mediaCapture.InitializeAsync(new MediaCaptureInitializationSettings()  { SharingMode = MediaCaptureSharingMode.ExclusiveControl,
                                                                                      MemoryPreference = MediaCaptureMemoryPreference.Cpu,
                                                                                      StreamingCaptureMode = StreamingCaptureMode.Video
                                                                                    });
      res = await InitialiserSourceImages(largeur, hauteur, sousTypeVideo);
      _ledOccupe?.Write(GpioPinValue.Low);
      return res;
    }
    public void PrendrePhoto()
    {
      lock (_photoAPrendreLOCK)
      {
        if (_photoAPrendre == 0)
          _photoAPrendre = 1;
      }
    }

  }
}
