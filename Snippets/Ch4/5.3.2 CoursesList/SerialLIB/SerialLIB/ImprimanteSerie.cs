using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace SerialLIB
{
  public class ImprimanteSerie : PeripheriqueSerie
  {
    protected override void DonnesRecues(DataReader donnees) { }

    public ImprimanteSerie(ParametresPortSerie parametres, bool autoOuvrir, uint nbMaxOctetsALire) : base(parametres, autoOuvrir, nbMaxOctetsALire)
    {
    }

    public virtual async Task ImprimerTexte(string texte, int codepage = 0)
    {
      await EnvoyerDonnees(Encoding.GetEncoding(codepage).GetBytes(texte));
    }
  }
}
