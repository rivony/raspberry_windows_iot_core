using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace SerialLIB
{
  public delegate void CodeBarresRecu(object sender, string codeBarres);
  public class LecteurCodeBarres : PeripheriqueSerie
  {
    protected override void DonnesRecues(DataReader donnees)
    {
      string codeLu = donnees.ReadString(donnees.UnconsumedBufferLength);
      Debug.WriteLine("Code-barres lu : " + codeLu);
      if (codeLu.Length >= 2)
      {
        string codeBarres = codeLu.Substring(0, codeLu.Length - 2);
        ulong codeBarresNumerique;
        if (ulong.TryParse(codeBarres, out codeBarresNumerique))
          Task.Run(() => { SurCodeBarresRecu?.Invoke(this, codeBarres); });
      }
    }

    public LecteurCodeBarres(ParametresPortSerie parametres, bool autoOuvrir, uint nbMaxOctetsALire) : base(parametres, autoOuvrir, nbMaxOctetsALire) { }
    public event CodeBarresRecu SurCodeBarresRecu;
  }
}
