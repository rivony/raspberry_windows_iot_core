using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DoMesCenterLIB
{
  public class DoMesCenterCsv
  {
    private const string NOM_FICHIER_APPLICATION = "DoMesCenter";
    private const string CSV_SEPARATOR = ";";
    private static StorageFolder _dossierApplication = null;
    private async static Task<StorageFolder> GetDossierApplication()
    {
      if (_dossierApplication == null)
        _dossierApplication = await ApplicationData.Current.LocalFolder.CreateFolderAsync(NOM_FICHIER_APPLICATION, CreationCollisionOption.OpenIfExists);
      return _dossierApplication;
    }
    private string ValeurBool(bool? val)
    {
      if (val.HasValue)
        return val.Value ? "O" : "N";
      return "";
    }

    public async void AjouterMesures(DateTime instant, double? tempInt, double? tempExtAmbiante, double? tempExtIR, double? humiditeInt, double? humiditeExt, double? pression, double? luminosite, ulong? indexHP_BASE, ulong? indexHC, bool? periodeTarifEnCoursHC, bool? DepCourantSurvenu)
    {
      StorageFolder dossierApplicaton = await GetDossierApplication();
      string nomFichier = instant.ToString("yyyyMMdd") + "_" + NOM_FICHIER_APPLICATION + ".csv";
      IStorageItem elementSurDisque = await dossierApplicaton.TryGetItemAsync(nomFichier);
      StorageFile fichier;
      List<string> lignes = new List<string>();
      if (elementSurDisque == null)
      {
        fichier = await dossierApplicaton.CreateFileAsync(nomFichier);
        lignes.Add("HEURE" + CSV_SEPARATOR + "Temp int (°C)" + CSV_SEPARATOR + "Temp ext Amb (°C)" + CSV_SEPARATOR + "Temp ext Ir (°C)" + CSV_SEPARATOR + "Humid int (%rH)" + CSV_SEPARATOR + "Humid ext (%rH)" + CSV_SEPARATOR + "Press (hPa)" + CSV_SEPARATOR + "Lum (Lux)" + CSV_SEPARATOR + "Index HP/BASE (Wh)" + CSV_SEPARATOR + "Index HC (Wh)" + CSV_SEPARATOR + "HC (O/N)" + CSV_SEPARATOR + "Dep. courant (O/N)");
      }
      else
        fichier = (StorageFile)elementSurDisque;
      lignes.Add(instant.ToString("HH:mm:ss") + CSV_SEPARATOR + tempInt.ToString() + CSV_SEPARATOR + tempExtAmbiante.ToString() + CSV_SEPARATOR + tempExtIR.ToString() + CSV_SEPARATOR + humiditeInt.ToString() + CSV_SEPARATOR + humiditeExt.ToString() + CSV_SEPARATOR + pression.ToString() + CSV_SEPARATOR + luminosite.ToString() + CSV_SEPARATOR + indexHP_BASE.ToString() + CSV_SEPARATOR + indexHC.ToString() + CSV_SEPARATOR + ValeurBool(periodeTarifEnCoursHC) + CSV_SEPARATOR + ValeurBool(DepCourantSurvenu));
      await FileIO.AppendLinesAsync(fichier, lignes);
    }
  }
}
